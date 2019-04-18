using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Models;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using MvvmCross.Navigation;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using System.Collections.ObjectModel;
using ProspectManagement.Core.Extensions;

namespace ProspectManagement.Core.ViewModels
{
    public class ActivitiesViewModel : BaseViewModel, IMvxViewModel<Prospect>
    {
        public event EventHandler LoadingDataFromBackendStarted;
        public event EventHandler LoadingDataFromBackendCompleted;

        private Prospect _prospect;
        private User _user;
        private bool _assigned;
        private Activity _activity;

        private ICommand _showDetailTab;
        private ICommand _showTrafficCardTab;
        private ICommand _showCobuyerTab;

        private ICommand _selectionChangedCommand;
        private ICommand _completeApptCommand;
        private ICommand _addNoteCommand;
        private ICommand _addVisitCommand;
        private ICommand _refreshCommand;


        private readonly IUserService _userService;
        private readonly IActivityService _activitiesService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;

        private MvxInteraction _clearDetailsInteraction = new MvxInteraction();
        public IMvxInteraction ClearDetailsInteraction => _clearDetailsInteraction;

        private MvxInteraction _activityAddedInteraction = new MvxInteraction();
        public IMvxInteraction ActivityAddedInteraction => _activityAddedInteraction;

        private ObservableCollection<Activity> _activities;

        public ICommand ShowDetailTab
        {            
            get   
            {
                return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<SplitDetailViewModel, KeyValuePair<Prospect, User>>(new KeyValuePair<Prospect, User>(_prospect, _user))));
            }
        }

        public ICommand ShowTrafficCardTab
        {
            get
            {
                return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<TrafficCardViewModel, Prospect>(_prospect)));
            }
        }

        public ICommand ShowCobuyerTab
        {
            get
            {
                return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<CobuyerViewModel, Prospect>(_prospect)));
            }
        }

        public bool AssignedWithoutAppointment
        {
            get { return AssignedProspect || (IsLead && !Prospect.ProspectCommunity.AppointmentStatus.Equals("Pending")); }
        }

        public bool AssignedProspect
        {
            get { return Assigned && !IsLead; }
        }

        public bool Assigned
        {
            get { return _assigned; }
            set
            {
                _assigned = value;
                RaisePropertyChanged(() => AssignedWithoutAppointment);
                RaisePropertyChanged(() => AssignedProspect);
                RaisePropertyChanged(() => Assigned);
            }
        }

        public bool IsLead
        {
            get { return Prospect.ProspectCommunity.AddressType.Equals("Lead"); }
        }

        public bool IsLeadWithAppointment
        {
            get { return IsLead && Prospect.ProspectCommunity.AppointmentStatus.Equals("Pending"); }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new MvxCommand(async () =>
                {
                    //Messenger.Publish(new RefreshMessage(this));
                    var list = await _activitiesService.GetActivitiesForProspectAsync(_prospect.ProspectAddressNumber);
                    ActivitiesList = list.ToObservableCollection();
                    OnLoadingDataFromBackendCompleted();
                }));
            }
        }

        public ICommand CompleteApptCommand
        {
            get
            {
                return _completeApptCommand ?? (_completeApptCommand = new MvxCommand(() =>
                {
                    var activity = new Activity
                    {
                        ActivityType = "APPOINTMENT",
                        ContactMethod = "In-Person",
                        DateCompleted = DateTime.UtcNow,
                        ProspectAddressNumber = Prospect.ProspectAddressNumber,
                        SalespersonAddressNumber = Prospect.ProspectCommunity.SalespersonAddressNumber,
                        ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
                        ProspectCommunity = Prospect.ProspectCommunity
                    };
                    _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
                }));
            }
        }

        public ICommand AddNoteCommand
        {
            get
            {
                return _addNoteCommand ?? (_addNoteCommand = new MvxCommand(() =>
                {
                    var activity = new Activity
                    {
                        ActivityType = "COMMENT",
                        DateCompleted = DateTime.UtcNow,
                        ProspectAddressNumber = Prospect.ProspectAddressNumber,
                        SalespersonAddressNumber = Prospect.ProspectCommunity.SalespersonAddressNumber,
                        ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
                        ProspectCommunity = Prospect.ProspectCommunity
                    };
                    _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
                }));
            }
        }

        public ICommand AddVisitCommand
        {
            get
            {
                return _addVisitCommand ?? (_addVisitCommand = new MvxCommand(() =>
                {
                    var activity = new Activity
                    {
                        ActivityType = "VISIT",
                        ContactMethod = "In-Person",
                        DateCompleted = DateTime.UtcNow,
                        ProspectAddressNumber = Prospect.ProspectAddressNumber,
                        SalespersonAddressNumber = Prospect.ProspectCommunity.SalespersonAddressNumber,
                        ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
                        ProspectCommunity = Prospect.ProspectCommunity
                    };
                    _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
                }));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Activity>(async (activity) =>
                 {

                     activity = await _activitiesService.GetActivityForProspectAsync(_prospect.ProspectAddressNumber, activity.InstanceID);

                     if (activity.AdditionalNotesExist)
                     {
                         Analytics.TrackEvent("Contact History Detail Viewed", new Dictionary<string, string>
                        {
                            {"Community", Prospect.ProspectCommunity.CommunityNumber + " " + Prospect.ProspectCommunity.Community.Description},
                            {"SalesAssociate", Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Prospect.ProspectCommunity.SalespersonName},
                            {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                        });
                         _navigationService.Navigate<ActivityDetailViewModel, KeyValuePair<Prospect, Activity>>(new KeyValuePair<Prospect, Activity>(_prospect, activity));

                     }
                 }));
            }
        }

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                Assigned = _prospect.ProspectCommunity.SalespersonAddressNumber > 0;
                RaisePropertyChanged(() => Prospect);
            }
        }

        public ObservableCollection<Activity> ActivitiesList
        {
            get { return _activities; }
            set
            {
                _activities = value;
                RaisePropertyChanged(() => ActivitiesList);
            }
        }

        public ActivitiesViewModel(IMvxMessenger messenger, IActivityService activitiesService, IMvxNavigationService navigationService, IUserService userService)
        {
            Messenger = messenger;
            _activitiesService = activitiesService;
            _navigationService = navigationService;
            _userService = userService;

            Messenger.Subscribe<RefreshMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<UserLogoutMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<ActivityAddedMessage>(message => ActivityAdded(message.AddedActivity), MvxReference.Strong);
        }

        public void ActivityAdded(Activity activity)
        {
            if (activity.ProspectAddressNumber == _prospect.ProspectAddressNumber)
            { 
                RaisePropertyChanged(() => IsLead);
                RaisePropertyChanged(() => AssignedProspect);
                RaisePropertyChanged(() => IsLeadWithAppointment);
                RaisePropertyChanged(() => AssignedWithoutAppointment);
                ActivitiesList.Add(activity);
                _activityAddedInteraction.Raise();
            }
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();
            Analytics.TrackEvent("Contact History Viewed", new Dictionary<string, string>
            {
                {"Community", Prospect.ProspectCommunity.CommunityNumber + " " + Prospect.ProspectCommunity.Community.Description},
                {"SalesAssociate", Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Prospect.ProspectCommunity.SalespersonName},
                {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
            });

            OnLoadingDataFromBackendStarted();
            var list = await _activitiesService.GetActivitiesForProspectAsync(_prospect.ProspectAddressNumber);
            ActivitiesList = list.ToObservableCollection();
            OnLoadingDataFromBackendCompleted();
        }

        public void Prepare(Prospect prospect)
        {
            Prospect = prospect;
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
