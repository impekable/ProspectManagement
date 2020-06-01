using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.InfiniteScroll;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SMSInboxViewModel : BaseViewModel, IMvxViewModel<User>
    {
        public event EventHandler LoadingDataFromBackendStarted;
        public event EventHandler LoadingDataFromBackendCompleted;
        private event EventHandler<int> _incrementalLoadFromBackendCompleted;

        private readonly IAuthenticator _authService;
        private readonly IActivityService _activityService;
        private readonly IProspectService _prospectService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IIncrementalCollectionFactory _collectionFactory;
        private ObservableCollection<SmsActivity> _smsActivities;

        private IMvxCommand _homeCommand;
        private IMvxCommand _refreshCommand;
        private IMvxCommand _selectionChangedCommand;

        private int _selectedSegment;
        private int _page = 0;
        private int _pageSize = 10;
        private User _user;

        private MvxInteraction<TableRow> _updateRowInteraction = new MvxInteraction<TableRow>();
        public IMvxInteraction<TableRow> UpdateRowInteraction => _updateRowInteraction;


        public IMvxCommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<SmsActivity>(async (smsActivity) =>
                {
                    await _navigationService.Navigate<SMSInboxDetailViewModel, KeyValuePair<SmsActivity, User>>(new KeyValuePair<SmsActivity, User>(smsActivity, User));

                    if (smsActivity.UnreadCount > 0)
                    {
                        var success = await _prospectService.UpdateProspectSMSActivityAsync(smsActivity.Prospect.ProspectAddressNumber);
                        if (success)
                        {
                            smsActivity.UnreadCount = 0;
                            var request = new TableRow { TableRowToUpdate = SMSActivities.IndexOf(smsActivity) };
                            _updateRowInteraction.Raise(request);
                        }
                    }

                    Analytics.TrackEvent("SMS Activity Selected", new Dictionary<string, string>
                    {
                        {"Community", smsActivity.Prospect.ProspectCommunity.CommunityNumber + " " + smsActivity.Prospect.ProspectCommunity.Community.Description},
                        {"SalesAssociate", smsActivity.Prospect.ProspectCommunity.SalespersonAddressNumber + " " + smsActivity.Prospect.ProspectCommunity.SalespersonName},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                    });
                }));
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

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public IMvxCommand HomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new MvxCommand(() =>
                {
                    _navigationService.Navigate<LandingViewModel, User>(User);
                }));
            }
        }

        public IMvxCommand RefreshCommand
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

        public ObservableCollection<SmsActivity> SMSActivities
        {
            get
            {
                {
                    if (_smsActivities == null)
                    {
                        _incrementalLoadFromBackendCompleted += (sender, e) =>
                        {
                            OnLoadingDataFromBackendCompleted();
                        };
                        _smsActivities = _collectionFactory.GetCollection(async (count, pageSize) =>
                        {
                            var authResult = await _authService.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                            var newActivities = new ObservableCollection<SmsActivity>();
                            if (authResult != null && !String.IsNullOrEmpty(authResult.AccessToken))
                            {
                                await Task.Run(async () =>
                                {
                                    Page++;
                                    var newOnly = SelectedSegment == 1;
                                    var activityList = await _activityService.GetSmsActivitiesAsync(authResult.AccessToken, User.AddressNumber, newOnly, Page, pageSize);
                                    newActivities = activityList.ToObservableCollection();
                                });
                            }
                            return newActivities;
                        }, _incrementalLoadFromBackendCompleted, _pageSize);
                    }
                }
                return _smsActivities;
            }
            set
            {
                _smsActivities = value;
                RaisePropertyChanged(() => SMSActivities);
            }
        }

        public void RemoveTask(SmsActivity activity)
        {
            var viewModel = SMSActivities.First(r => r.ActivityId == activity.ActivityId);

            SMSActivities.Remove(viewModel);
        }

        public void OnLoadingDataFromBackendStarted()
        {
            LoadingDataFromBackendStarted?.Invoke(null, EventArgs.Empty);
        }

        public void OnLoadingDataFromBackendCompleted()
        {
            Analytics.TrackEvent("SMS Inbox Viewed", new Dictionary<string, string>
            {
                {"User", User.AddressNumber + " " + User.AddressBook.Name},
                {"Unread Filter", SelectedSegment == 0 ? "All" : "Unread" },
            });
            LoadingDataFromBackendCompleted?.Invoke(null, EventArgs.Empty);
        }

        public SMSInboxViewModel(IMvxMessenger messenger, IAuthenticator authService, IProspectService prospectService, IActivityService activityService, IIncrementalCollectionFactory collectionFactory, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _authService = authService;
            _activityService = activityService;
            _collectionFactory = collectionFactory;
            _navigationService = navigationService;
            _prospectService = prospectService;
        }

        public void Prepare(User parameter)
        {
            User = parameter;
        }
    }
}