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
using ProspectManagement.Core.Interfaces.InfiniteScroll;

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

		private readonly IIncrementalCollectionFactory _collectionFactory;
		private ObservableCollection<Prospect> _prospects;

		private ICommand _selectionChangedCommand;
		private ICommand _logoutCommand;
		private ICommand _filterCommand;
		private ICommand _refreshCommand;

        private string _scopeFilter;
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

        public string ScopeFilter
        {
            get { return _scopeFilter; }
            set
            {
                _scopeFilter = value;
                RaisePropertyChanged(() => ScopeFilter);
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
									var prospectList = await _prospectService.GetProspectsAsync(authResult.AccessToken, _communities, salespersonId, searchType, Page, pageSize, SearchTerm, ScopeFilter);
									newProspects = prospectList.ToObservableCollection();
									OnLoadingDataFromBackendCompleted();
								});
							}
							return newProspects;
						}, _pageSize);
					}
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
                    var result = await _dialogService.ShowAlertAsync("Confirm", "Logout?", "Yes", "No");
                    if (result == 0)
                    {
                        _authService.Logout();
                        _navigationService.Navigate<RootViewModel, bool>(false);
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

		public SplitMasterViewModel(IMvxMessenger messenger, IDialogService dialogService, IUserService userService, IAuthenticator authService, ICommunityService communityService, IProspectService prospectService, IIncrementalCollectionFactory collectionFactory, IMvxNavigationService navigationService)
		{
			Messenger = messenger;
			_dialogService = dialogService;
			_userService = userService;
			_authService = authService;
			_communityService = communityService;
			_prospectService = prospectService;
			_collectionFactory = collectionFactory;
			_navigationService = navigationService;

			Messenger.Subscribe<ProspectChangedMessage>(message => ProspectUpdated(message.UpdatedProspect), MvxReference.Strong);
			Messenger.Subscribe<ProspectAssignedMessage>(message => ProspectAssigned(message.AssignedProspect), MvxReference.Strong);
			Messenger.Subscribe<ActivityAddedMessage>(message => ActivityAdded(message.AddedActivity), MvxReference.Strong);

		}

		public void ActivityAdded(Activity activity)
		{
			try
			{
				if (activity.ActivityType.Equals("VISIT") || activity.ActivityType.Equals("APPOINTMENT"))
				{
					var prospect = Prospects.FirstOrDefault(p => p.ProspectAddressNumber == activity.ProspectAddressNumber);
					if (prospect != null)
					{
						prospect.ProspectCommunity.EndDate = (DateTime)activity.DateCompleted;
						var request = new TableRow { TableRowToUpdate = Prospects.IndexOf(prospect) };
						_updateRowInteraction.Raise(request);
					}
				}
			}
			catch (Exception ex)
			{

			}
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
            ScopeFilter = "All";
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
