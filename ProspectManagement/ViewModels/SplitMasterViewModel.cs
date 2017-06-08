using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using Sequence.Plugins.InfiniteScroll;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitMasterViewModel : BaseViewModel
    {
		public event EventHandler LoadingDataFromBackendStarted;
		public event EventHandler LoadingDataFromBackendCompleted;

        private readonly IUserService _userService;
        private readonly IProspectService _prospectService;
        private readonly IIncrementalCollectionFactory _collectionFactory;
        private ObservableCollection<Prospect> _prospects;
        private ICommand _selectionChangedCommand;
        private int _selectedSegment;
        private int _page = 0;
        private int _pageSize = 10;
        private string _searchTerm;
        private User _user;

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

                                        await Task.Run(async () =>
                                        {
                                            Page++;                                            
                                            var prospectList = await _prospectService.GetProspectsAsync(User.AddressNumber, SelectedSegment == 1, Page, pageSize, SearchTerm);
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

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Prospect>((prospect) => ShowViewModel<SplitDetailViewModel>(prospect)));
            }
        }

        public SplitMasterViewModel(IUserService userService, IProspectService prospectService, IIncrementalCollectionFactory collectionFactory)
        {
            _prospectService = prospectService;
            _collectionFactory = collectionFactory;
            _userService = userService;
        }

        public override async void Start()
        {
            base.Start();
            await ReloadDataAsync();
        }

        protected override async Task InitializeAsync()
        {
            while (User == null || User.AddressNumber == 0)
            {
                User = await _userService.GetLoggedInUser();
            }
        }

		public void OnLoadingDataFromBackendStarted()
		{
			LoadingDataFromBackendStarted?.Invoke(null, EventArgs.Empty);
		}

		public void OnLoadingDataFromBackendCompleted()
		{
			LoadingDataFromBackendCompleted?.Invoke(null, EventArgs.Empty);
		}
    }
}
