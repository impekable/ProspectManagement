using System;
using System.Windows.Input;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitDetailViewModel : BaseViewModel, IMvxViewModel<Prospect>
    {
        private Prospect _prospect;
        private bool _assigned;
        private string _workPhoneLabel;
        private string _homePhoneLabel;
        private string _mobilePhoneLabel;
        private ICommand _editProspectCommand;
        private ICommand _assignCommand;
        private ICommand _addNoteCommand;
        private ICommand _completeApptCommand;
        private ICommand _addVisitCommand;
        private ICommand _showCobuyerTab;
        private ICommand _showTrafficCardTab;
        private readonly IProspectService _prospectService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;

        public bool IsLead
        {
            get { return Prospect.ProspectCommunity.AddressType.Equals("Lead"); }
        }

        public bool IsLeadWithAppointment
        {
            get { return IsLead && Prospect.ProspectCommunity.AppointmentStatus.Equals("Pending"); }
        }

        public bool StreetAddressEntered
        {
            get { return Prospect.StreetAddress != null && !String.IsNullOrEmpty(Prospect.StreetAddress.AddressLine1); }
        }

        public bool EmailEntered
        {
            get { return Prospect.Email != null && !String.IsNullOrEmpty(Prospect.Email.EmailAddress); }
        }

        public bool WorkPhoneEntered
        {
            get { return Prospect.WorkPhoneNumber != null && !String.IsNullOrEmpty(Prospect.WorkPhoneNumber.Phone); }
        }

        public bool MobilePhoneEntered
        {
            get { return Prospect.MobilePhoneNumber != null && !String.IsNullOrEmpty(Prospect.MobilePhoneNumber.Phone); }
        }

        public bool HomePhoneEntered
        {
            get { return Prospect.HomePhoneNumber != null && !String.IsNullOrEmpty(Prospect.HomePhoneNumber.Phone); }
        }

        private MvxInteraction _showAlertInteraction = new MvxInteraction();
        public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private MvxInteraction _clearDetailsInteraction = new MvxInteraction();
        public IMvxInteraction ClearDetailsInteraction => _clearDetailsInteraction;

        private MvxInteraction _assignedProspectInteraction = new MvxInteraction();
        public IMvxInteraction AssignedProspectInteraction => _assignedProspectInteraction;

        public SplitDetailViewModel(IMvxMessenger messenger, IProspectService prospectService, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _prospectService = prospectService;
            _navigationService = navigationService;

            Messenger.Subscribe<RefreshMessage>(async message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<ProspectChangedMessage>(async message => Prepare(message.UpdatedProspect), MvxReference.Strong);
            Messenger.Subscribe<UserLogoutMessage>(async message => UserLogout(), MvxReference.Strong);
            Messenger.Subscribe<ActivityAddedMessage>(async message => ActivityAdded(message.AddedActivity), MvxReference.Strong);
        }

        public async void UserLogout()
        {
            _clearDetailsInteraction.Raise();
        }

        public async void ActivityAdded(Activity activity)
        {
            if (Prospect.ProspectCommunity.AddressType.Equals("Lead") && (activity.ActivityType.Equals("VISIT") || activity.ActivityType.Equals("APPOINTMENT")))
            {
                Prospect.ProspectCommunity.AddressType = "Prospect";
                RaisePropertyChanged(() => IsLead);
                RaisePropertyChanged(() => AssignedProspect);
                RaisePropertyChanged(() => IsLeadWithAppointment);
                RaisePropertyChanged(() => AssignedWithoutAppointment);
                _assignedProspectInteraction.Raise();
            }
        }

        public ICommand AssignCommand
        {
            get
            {
                return _assignCommand ?? (_assignCommand = new MvxCommand(async () =>
                {
                    _showAlertInteraction.Raise();
                    var _assignedTo = await _prospectService.AssignProspectToLoggedInUserAsync(_prospect.ProspectCommunity.CommunityNumber, _prospect.ProspectAddressNumber);
                    _hideAlertInteraction.Raise();

                    if (_assignedTo != null)
                    {
                        Assigned = true;
                        Prospect.ProspectCommunity.SalespersonAddressNumber = _assignedTo.AddressNumber;
                        Prospect.ProspectCommunity.SalespersonName = _assignedTo.Name;
                        Messenger.Publish(new ProspectAssignedMessage(this) { AssignedProspect = Prospect });
                        _assignedProspectInteraction.Raise();
                    }
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
                        ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId
                    };
                    _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
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
                        ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId
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
                        ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId
                    };
                    _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
                }));
            }
        }

        public ICommand EditProspectCommand
        {
            get
            {
                return _editProspectCommand ?? (_editProspectCommand = new MvxCommand(() => _navigationService.Navigate<EditProspectViewModel, Prospect>(_prospect)));
            }
        }

        public ICommand ShowCobuyerTab
        {
            get
            {
                return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<CobuyerViewModel, Prospect>(_prospect)));
            }
        }

        public ICommand ShowTrafficCardTab
        {
            get
            {
                return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<TrafficCardViewModel, Prospect>(_prospect)));
            }
        }

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                Assigned = _prospect.ProspectCommunity.SalespersonAddressNumber > 0;
                WorkPhoneLabel = _prospect.WorkPhoneNumber == null || string.IsNullOrEmpty(_prospect.WorkPhoneNumber.Phone) ? null : "Work";
                MobilePhoneLabel = _prospect.MobilePhoneNumber == null || string.IsNullOrEmpty(_prospect.MobilePhoneNumber.Phone) ? null : "Mobile";
                HomePhoneLabel = _prospect.HomePhoneNumber == null || string.IsNullOrEmpty(_prospect.HomePhoneNumber.Phone) ? null : "Home";
                RaisePropertyChanged(() => Prospect);
                RaisePropertyChanged(() => StreetAddressEntered);
            }
        }

        public bool AssignedProspect
        {
            get { return Assigned && !IsLead; }
        }

        public bool AssignedWithoutAppointment
        {
            get { return AssignedProspect ||(IsLead && !Prospect.ProspectCommunity.AppointmentStatus.Equals("Pending")); }
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

        public string WorkPhoneLabel
        {
            get { return _workPhoneLabel; }
            set
            {
                _workPhoneLabel = value;
                RaisePropertyChanged(() => WorkPhoneLabel);
            }
        }

        public string HomePhoneLabel
        {
            get { return _homePhoneLabel; }
            set
            {
                _homePhoneLabel = value;
                RaisePropertyChanged(() => HomePhoneLabel);
            }
        }

        public string MobilePhoneLabel
        {
            get { return _mobilePhoneLabel; }
            set
            {
                _mobilePhoneLabel = value;
                RaisePropertyChanged(() => MobilePhoneLabel);
            }
        }

        public override async void Start()
        {
            base.Start();
            await ReloadDataAsync();
        }

        public async void Prepare(Prospect prospect)
        {
            Prospect = prospect;
        }
    }


}
