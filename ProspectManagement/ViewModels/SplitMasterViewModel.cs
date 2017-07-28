using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using Sequence.Plugins.InfiniteScroll;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitMasterViewModel : BaseViewModel
    {
        public event EventHandler LoadingDataFromBackendStarted;
        public event EventHandler LoadingDataFromBackendCompleted;
        public event EventHandler LoginCompleted;
        public event EventHandler LogoutCompleted;

        private readonly IDialogService _dialogService;
        private readonly IAuthenticator _authService;
        private readonly ICommunityService _communityService;
        private readonly IUserService _userService;
        private readonly IProspectService _prospectService;
        private readonly IProspectCache _prospectCache;
        private readonly IIncrementalCollectionFactory _collectionFactory;
        private ObservableCollection<Prospect> _prospects;
        private ICommand _selectionChangedCommand;
        private ICommand _logoutCommand;
        private int _selectedSegment;
        private int _page = 0;
        private int _pageSize = 10;
        private string _searchTerm;
        private User _user;
        private List<Community> _communities;

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public int Page
        {
            get { return _page; }
            set
            {
                _page = value;
                OnLoadingDataFromBackendStarted();
                RaisePropertyChanged(() => Page);
            }
        }

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _page = 0;
                _searchTerm = value;
                OnLoadingDataFromBackendStarted();
                RaisePropertyChanged(() => SearchTerm);
            }
        }

        public int SelectedSegment
        {
            get { return _selectedSegment; }
            set
            {
                _page = 0;
                _selectedSegment = value;
                OnLoadingDataFromBackendStarted();
                RaisePropertyChanged(() => SelectedSegment);
            }
        }

        public ObservableCollection<Prospect> Prospects
        {
            get
            {
                if (_prospects == null)
                {                    
                    _prospects = _collectionFactory.GetCollection(async (count, pageSize) =>
                                    {
                                        var newProspects = new ObservableCollection<Prospect>();
                                        if (_communities == null)
                                        {
                                            _communities = await _communityService.GetCommunitiesBySalesperson(User.AddressNumber);
                                        }
                                        await Task.Run(async () =>
                                        {
                                            Page++;
                                            var salespersonId = SelectedSegment == 0 ? (int?)null : SelectedSegment == 1 ? 0 : User.AddressNumber;
                                            var prospectList = await _prospectService.GetProspectsAsync(_communities, salespersonId , Page, pageSize, SearchTerm);
                                            newProspects = prospectList.ToObservableCollection();
                                            OnLoadingDataFromBackendCompleted();
                                        });
                                        return newProspects;
                                    }, _pageSize);

                }
                return _prospects;
            }
            set
            {
                _prospects = value;
                RaisePropertyChanged(() => Prospects);
            }
        }


        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new MvxCommand(async () => 
                {
                    if (User == null)
                    {
                        while (User == null)
                        {
                            User = await _userService.GetLoggedInUser();
                        }
                        OnLoginCompleted();
                    }
                    else
                    {
                        var result = await _dialogService.ShowAlertAsync("Confirm", "Logout?", "Yes", "No");
                        if (result == 0)
                        {
                            _communities = null;
                            _prospects = null;
                            _authService.Logout();
                            User = null;
                            OnLogoutCompleted();
                        }
                    }
                } ));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Prospect>((prospect) => { _prospectCache.SaveProspectToCache(prospect); ShowViewModel<SplitDetailViewModel>(prospect); }));
            }
        }

        public SplitMasterViewModel(IProspectCache prospectCache, IDialogService dialogService, IUserService userService, IAuthenticator authService, ICommunityService communityService, IProspectService prospectService, IIncrementalCollectionFactory collectionFactory)
        {
            _prospectCache = prospectCache;
            _dialogService = dialogService;
            _userService = userService;
            _authService = authService;
            _communityService = communityService;
            _prospectService = prospectService;
            _collectionFactory = collectionFactory;
        }

        public override async void Start()
        {
            base.Start();
            await ReloadDataAsync();
        }

        public async void Init(User parameter)
        {
            User = parameter;
        }

        public void OnLoadingDataFromBackendStarted()
        {
            LoadingDataFromBackendStarted?.Invoke(null, EventArgs.Empty);
        }

        public void OnLoadingDataFromBackendCompleted()
        {
            LoadingDataFromBackendCompleted?.Invoke(null, EventArgs.Empty);
        }

        public void OnLogoutCompleted()
        {
            LogoutCompleted?.Invoke(null, EventArgs.Empty);
        }

        public void OnLoginCompleted()
        {
            LoginCompleted?.Invoke(null, EventArgs.Empty);
        }
    }
}
