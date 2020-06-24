using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.InfiniteScroll;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class DailyToDoViewModel : BaseViewModel, IMvxViewModel<User>
    {
        public event EventHandler LoadingDataFromBackendStarted;
        public event EventHandler LoadingDataFromBackendCompleted;
        private event EventHandler<int> _incrementalLoadFromBackendCompleted;

        private readonly IAuthenticator _authService;
        private readonly IActivityService _activityService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserDefinedCodeService _userDefinedCodeService;
        private readonly IIncrementalCollectionFactory _collectionFactory;
        private ObservableCollection<DailyToDoTaskViewModel> _activities;

        private ICommand _homeCommand;
        private ICommand _filterCommand;
        private ICommand _refreshCommand;

        private int _selectedSegment;
        private int _page = 0;
        private int _pageSize = 10;
        private User _user;
        private ObservableCollection<UserDefinedCode> _rankings;

        private MvxSubscriptionToken dismissedMessengerToken;
        private MvxSubscriptionToken completedMessengerToken;
        //private MvxInteraction<TableRow> _updateRowInteraction = new MvxInteraction<TableRow>();
        //public IMvxInteraction<TableRow> UpdateRowInteraction => _updateRowInteraction;

        //private MvxInteraction<Filter> _filterInteraction = new MvxInteraction<Filter>();
        //public IMvxInteraction<Filter> FilterInteraction => _filterInteraction;

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

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public ICommand HomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new MvxCommand(() =>
                {
                    _navigationService.Navigate<LandingViewModel, User>(User);
                }));
            }
        }

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

        public ObservableCollection<DailyToDoTaskViewModel> Activities
        {
            get
            {
                {
                    if (_activities == null)
                    {
                        _incrementalLoadFromBackendCompleted += (sender, e) =>
                        {
                            OnLoadingDataFromBackendCompleted();
                        };
                        _activities = _collectionFactory.GetCollection(async (count, pageSize) =>
                        {
                            var authResult = await _authService.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                            var newActivities = new ObservableCollection<DailyToDoTaskViewModel>();
                            if (authResult != null && !String.IsNullOrEmpty(authResult.AccessToken))
                            {
                                if (_rankings == null)
                                {
                                    _rankings = (await _userDefinedCodeService.GetRankingUserDefinedCodes()).ToObservableCollection();
                                }
                                await Task.Run(async () =>
                                {
                                    Page++;
                                    var category = SelectedSegment == 0 ? null : SelectedSegment == 1 ? "3" : SelectedSegment == 2 ? "2" : "1";
                                    var activityList = await _activityService.GetDailyToDoActivitiesAsync(authResult.AccessToken, _rankings.ToList(), User.AddressNumber, category, DateTime.Now, Page, pageSize);
                                    foreach (var item in activityList)
                                    {
                                        newActivities.Add(new DailyToDoTaskViewModel(Messenger, _navigationService, _activityService) { Activity = item, User = User });
                                    }
                                });
                            }
                            return newActivities;
                        }, _incrementalLoadFromBackendCompleted, _pageSize);
                    }
                }
                return _activities;
            }
            set
            {
                _activities = value;
                RaisePropertyChanged(() => Activities);
            }
        }

        public void RemoveTask(Activity activity)
        {
            var viewModel = Activities.FirstOrDefault(r => r.Activity.ActivityID == activity.ActivityID);
            if (viewModel != null)
            {
                Activities.Remove(viewModel);
            }
        }

        public void OnLoadingDataFromBackendStarted()
        {
            LoadingDataFromBackendStarted?.Invoke(null, EventArgs.Empty);
        }

        public void OnLoadingDataFromBackendCompleted()
        {
            Analytics.TrackEvent("Daily To Do Searched", new Dictionary<string, string>
            {
                {"User", User.AddressNumber + " " + User.AddressBook.Name},
                {"Category Filter", SelectedSegment == 0 ? null : SelectedSegment == 1 ? "3" : SelectedSegment == 2 ? "2" : "1"},
            });
            LoadingDataFromBackendCompleted?.Invoke(null, EventArgs.Empty);
        }

        public DailyToDoViewModel(IMvxMessenger messenger, IAuthenticator authService, IUserDefinedCodeService userDefinedCodeService, IActivityService activityService, IIncrementalCollectionFactory collectionFactory, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _authService = authService;
            _activityService = activityService;
            _collectionFactory = collectionFactory;
            _navigationService = navigationService;
            _userDefinedCodeService = userDefinedCodeService;

            dismissedMessengerToken = Messenger.Subscribe<TaskDismissedMessage>(message => RemoveTask(message.Activity));
            completedMessengerToken = Messenger.Subscribe<TaskCompletedMessage>(message => RemoveTask(message.Activity));
        }

        public void Prepare(User parameter)
        {
            User = parameter;
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            if (dismissedMessengerToken != null)
            {
                Messenger.Unsubscribe<SMSReceivedMessage>(dismissedMessengerToken);
                dismissedMessengerToken = null;
            }
            if (completedMessengerToken != null)
            {
                Messenger.Unsubscribe<SMSReceivedMessage>(completedMessengerToken);
                completedMessengerToken = null;
            }
        }

    }
}
