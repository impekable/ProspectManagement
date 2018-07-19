using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;
using MvvmCross.Commands;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitMasterViewModel : BaseViewModel, IMvxViewModel<User>
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
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        
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

		public IMvxCommand LoadProspectsCommand => new MvxCommand(FetchProspects);
		public IMvxCommand FetchProspectsCommand => new MvxCommand(FetchProspects);

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new MvxCommand(() =>
                {
                    Messenger.Publish(new RefreshMessage(this));
                }));
            }
        }

        public ICommand FilterCommand
        {
            get
            {
                return _filterCommand ?? (_filterCommand = new MvxCommand(() =>
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
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Prospect>((prospect) =>
                {
					_navigationService.Navigate<SplitDetailViewModel, KeyValuePair<Prospect, User>>(new KeyValuePair<Prospect, User>(prospect, User));
                    Analytics.TrackEvent("Prospect Selected", new Dictionary<string, string>
                    {
                        {"Community", prospect.ProspectCommunity.CommunityNumber + " " + prospect.ProspectCommunity.Community.Description},
                        {"SalesAssociate", prospect.ProspectCommunity.SalespersonAddressNumber + " " + prospect.ProspectCommunity.SalespersonName},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                    });
                }));
            }
        }

		private async void FetchProspects()
		{
			if (_page == 0)
            {
                Prospects?.Clear();
            }
            var authResult = await _authService.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
            var newProspects = new ObservableCollection<Prospect>();
            if (authResult != null && !String.IsNullOrEmpty(authResult.AccessToken))
            {
                if (_communities == null)
                {
                    _communities = await _communityService.GetCommunitiesBySalesperson(User.AddressNumber);
                }
                Page++;
                var searchType = FilterActive ? "Lead" : null;
                var salespersonId = SelectedSegment == 0 ? (int?)null : SelectedSegment == 1 ? 0 : User.AddressNumber;
                var prospectList = await _prospectService.GetProspectsAsync(authResult.AccessToken, _communities, salespersonId, searchType, Page, _pageSize, SearchTerm);
                newProspects = prospectList?.ToObservableCollection();
                OnLoadingDataFromBackendCompleted();
                if (Prospects == null)
                    Prospects = new MvxObservableCollection<Prospect>();
				foreach (var prospect in newProspects)
				{
					Prospects.Add(prospect);
				}
            }
        }

        public SplitMasterViewModel(IMvxMessenger messenger, IDialogService dialogService, IUserService userService, IAuthenticator authService, ICommunityService communityService, IProspectService prospectService, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _dialogService = dialogService;
            _userService = userService;
            _authService = authService;
            _communityService = communityService;
            _prospectService = prospectService;
            _navigationService = navigationService;

            Messenger.Subscribe<ProspectChangedMessage>(message => ProspectUpdated(message.UpdatedProspect), MvxReference.Strong);
			Messenger.Subscribe<ProspectAssignedMessage>(message => ProspectAssigned(message.AssignedProspect), MvxReference.Strong);
        }

        public void ProspectUpdated(Prospect prospect)
        {
            Analytics.TrackEvent("Prospect Updated", new Dictionary<string, string>
            {
                {"Community", prospect.ProspectCommunity.CommunityNumber + " " + prospect.ProspectCommunity.Community.Description},
                {"SalesAssociate", prospect.ProspectCommunity.SalespersonAddressNumber + " " + prospect.ProspectCommunity.SalespersonName},
                {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
            });
            var request = new TableRow { TableRowToUpdate = Prospects.IndexOf(prospect) };
            _updateRowInteraction.Raise(request);
        }

        public void ProspectAssigned(Prospect prospect)
        {
            Analytics.TrackEvent("Prospect Assigned", new Dictionary<string, string>
            {
                {"Community", prospect.ProspectCommunity.CommunityNumber + " " + prospect.ProspectCommunity.Community.Description},
                {"SalesAssociate", prospect.ProspectCommunity.SalespersonAddressNumber + " " + prospect.ProspectCommunity.SalespersonName},
                {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
            });
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

        public void Prepare(User parameter)
        {
            User = parameter;
        }

		public void OnLoadingDataFromBackendStarted()
        {
            LoadingDataFromBackendStarted?.Invoke(null, EventArgs.Empty);
        }

        public void OnLoadingDataFromBackendCompleted()
        {
            Analytics.TrackEvent("Prospects Searched", new Dictionary<string, string>
            {
                {"User", User.AddressNumber + " " + User.AddressBook.Name},
                {"Leads Only", FilterActive.ToString()},
                {"Sales Associate Filter", SelectedSegment == 0 ? "All" : SelectedSegment == 1 ? "Unassigned" : "Mine"},
            });
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
