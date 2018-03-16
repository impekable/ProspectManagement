using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
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
        protected IMvxMessenger Messenger;

        private readonly IIncrementalCollectionFactory _collectionFactory;
        private ObservableCollection<Prospect> _prospects;

        private ICommand _selectionChangedCommand;
        private ICommand _logoutCommand;
        private ICommand _filterCommand;
        private ICommand _refreshCommand;

        private int _selectedSegment;
        private int _page = 0;
        private int _pageSize = 10;
        private string _searchTerm;
        private bool _filterActive;
        private User _user;
        private List<Community> _communities;

        private MvxInteraction<TableRow> _updateRowInteraction = new MvxInteraction<TableRow>();
        public IMvxInteraction<TableRow> UpdateRowInteraction => _updateRowInteraction;

        private MvxInteraction<Filter> _filterInteraction = new MvxInteraction<Filter>();
        public IMvxInteraction<Filter> FilterInteraction => _filterInteraction;

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new MvxCommand(async () =>
                {
                    Messenger.Publish(new RefreshMessage(this));
                }));
            }
        }

        public ICommand FilterCommand
        {
            get
            {
                return _filterCommand ?? (_filterCommand = new MvxCommand(async () =>
                {
                    var request = new Filter { Active = !FilterActive };
                    _filterInteraction.Raise(request);
                }));
            }
        }

        public bool FilterActive
        {
            get { return _filterActive; }
            set
            {
                _filterActive = value;
                RaisePropertyChanged(() => FilterActive);
            }
        }

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
                                        var authResult = await _authService.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                                        var newProspects = new ObservableCollection<Prospect>();
                                        if (authResult != null && !String.IsNullOrEmpty(authResult.AccessToken))
                                        {
                                            if (_communities == null)
                                            {
                                                _communities = await _communityService.GetCommunitiesBySalesperson(User.AddressNumber);
                                            }
                                            await Task.Run(async () =>
                                            {
                                                Page++;
                                                var searchType = FilterActive ? "Lead" : null;
                                                var salespersonId = SelectedSegment == 0 ? (int?)null : SelectedSegment == 1 ? 0 : User.AddressNumber;
                                                var prospectList = await _prospectService.GetProspectsAsync(authResult.AccessToken, _communities, salespersonId, searchType, Page, pageSize, SearchTerm);
                                                newProspects = prospectList.ToObservableCollection();
                                                OnLoadingDataFromBackendCompleted();
                                            });
                                        }
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
                    if (User == null || User.AddressNumber == 0)
                    {
                        User = await _userService.GetLoggedInUser();
                        if (User != null && User.AddressNumber != 0)
                        {
                            OnLoginCompleted();
                        }
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
                            Messenger.Publish(new UserLogoutMessage(this));
                        }
                    }
                }));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Prospect>((prospect) => { _prospectCache.SaveProspectToCache(prospect); ShowViewModel<SplitDetailViewModel>(prospect); }));
            }
        }

        public SplitMasterViewModel(IMvxMessenger messenger, IProspectCache prospectCache, IDialogService dialogService, IUserService userService, IAuthenticator authService, ICommunityService communityService, IProspectService prospectService, IIncrementalCollectionFactory collectionFactory)
        {
            Messenger = messenger;
            _prospectCache = prospectCache;
            _dialogService = dialogService;
            _userService = userService;
            _authService = authService;
            _communityService = communityService;
            _prospectService = prospectService;
            _collectionFactory = collectionFactory;

            Messenger.Subscribe<ProspectChangedMessage>(async message => ProspectUpdated(message.UpdatedProspect), MvxReference.Strong);
            Messenger.Subscribe<ProspectAssignedMessage>(async message => ProspectAssigned(message.AssignedProspect), MvxReference.Strong);

        }

        public async void ProspectUpdated(Prospect prospect)
        {
            var request = new TableRow { TableRowToUpdate = Prospects.IndexOf(prospect) };
            _updateRowInteraction.Raise(request);
        }

        public async void ProspectAssigned(Prospect prospect)
        {
            var request = new TableRow { TableRowToUpdate = Prospects.IndexOf(prospect) };
            if (SelectedSegment == 1) //Unassigned
            {
                Prospects.Remove(prospect);
            }
            else
            {
                _updateRowInteraction.Raise(request);
            }

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
