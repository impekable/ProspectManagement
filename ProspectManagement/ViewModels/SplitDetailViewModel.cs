using System;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;
using MvvmCross.Commands;
using MvvmCross.Plugin.PhoneCall;
using MvvmCross.Plugin.Email;

namespace ProspectManagement.Core.ViewModels
{
	public class SplitDetailViewModel : BaseViewModel, IMvxViewModel<KeyValuePair<Prospect, User>>
    {
        private Prospect _prospect;
		private User _user;
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
        private ICommand _showContactHistoryTab;
        private readonly IProspectService _prospectService;
        protected IMvxMessenger Messenger;
		private readonly IDialogService _dialogService;
        private readonly IMvxNavigationService _navigationService;
		private readonly IMvxPhoneCallTask _phoneCallTask;
		private readonly IMvxComposeEmailTaskEx _emailTask;

		public IMvxCommand MobilePhoneCallCommand => new MvxCommand(CallProspectMobile);
		public IMvxCommand WorkPhoneCallCommand => new MvxCommand(CallProspectWork);
		public IMvxCommand HomePhoneCallCommand => new MvxCommand(CallProspectHome);
		public IMvxCommand ComposeEmailCommand => new MvxCommand(ComposeEmailToProspect);

		private Activity createAdHocActivity(string contactMethod, string subject)
		{
			return new Activity
			{
				ActivityType = "ADHOC",
				ContactMethod = contactMethod,
				Subject = subject,
				TimeDateStart = DateTime.UtcNow,
				TimeDateEnd = DateTime.UtcNow,
				DateCompleted = DateTime.UtcNow,
				ProspectAddressNumber = Prospect.ProspectAddressNumber,
				SalespersonAddressNumber = Prospect.ProspectCommunity.SalespersonAddressNumber,
				ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
				Community = Prospect.ProspectCommunity.CommunityNumber
			};
		}

		private void ComposeEmailToProspect()
        {
			if (_emailTask.CanSendEmail)
			{
				var activity = createAdHocActivity("Email", "Emailed from App");
				_navigationService.Navigate<AddActivityViewModel, Activity>(activity);
				_emailTask.ComposeEmail(_prospect.Email.EmailAddress);
			}
			else
			{
				_dialogService.ShowAlertAsync("Please configure email on this device and then try again.", "Email Not Configured", "Close");
			}
        }

		private void CallProspectMobile()
		{
			var activity = createAdHocActivity("Phone", "Called from App");
            _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
			_phoneCallTask.MakePhoneCall(_prospect.Name, _prospect.MobilePhoneNumber.Phone);
		}

		private void CallProspectWork()
        {
			var activity = createAdHocActivity("Phone", "Called from App");
            _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
			_phoneCallTask.MakePhoneCall(_prospect.Name, _prospect.WorkPhoneNumber.Phone);
        }

		private void CallProspectHome()
        {
			var activity = createAdHocActivity("Phone", "Called from App");
            _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
			_phoneCallTask.MakePhoneCall(_prospect.Name, _prospect.HomePhoneNumber.Phone);
        }

		public bool AllowCalling
        {
			get { return Prospect.FollowUpSettings.ConsentToPhone && Assigned; }
        }

		public bool AllowEmailing
        {
			get { return Prospect.FollowUpSettings.ConsentToEmail && Assigned; }
        }

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
        
		public SplitDetailViewModel(IDialogService dialogService, IMvxComposeEmailTaskEx emailTask, IMvxPhoneCallTask phoneCallTask, IMvxMessenger messenger, IProspectService prospectService, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _prospectService = prospectService;
            _navigationService = navigationService;
			_phoneCallTask = phoneCallTask;
			_emailTask = emailTask;
			_dialogService = dialogService;

            Messenger.Subscribe<RefreshMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
			Messenger.Subscribe<ProspectChangedMessage>(message => Prepare(new KeyValuePair<Prospect, User>(message.UpdatedProspect, User)), MvxReference.Strong);
            Messenger.Subscribe<UserLogoutMessage>(message => UserLogout(), MvxReference.Strong);
            Messenger.Subscribe<ActivityAddedMessage>(message => ActivityAdded(message.AddedActivity), MvxReference.Strong);
			Messenger.Subscribe<ProspectChangedMessage>(message => 
            			{   RaisePropertyChanged(() => EmailEntered); 
            				RaisePropertyChanged(() => StreetAddressEntered);
            				RaisePropertyChanged(() => MobilePhoneEntered);
            				RaisePropertyChanged(() => HomePhoneEntered);
            				RaisePropertyChanged(() => WorkPhoneEntered);
            			}, 
                        MvxReference.Strong);

		}

        public void UserLogout()
        {
            _clearDetailsInteraction.Raise();
        }

        public void ActivityAdded(Activity activity)
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
						RaisePropertyChanged(() => AllowCalling);
						RaisePropertyChanged(() => AllowEmailing);
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
        public ICommand ShowContactHistoryTab
        {
            get
            {
                return _showContactHistoryTab ?? (_showContactHistoryTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<ActivitiesViewModel, Prospect>(_prospect)));
            }
        }


		public User User
		{
			get { return _user; }
			set { _user = value; }
		}

		public string AssignText
        {
			get { return "Assign Prospect To Me (" + _user?.AddressBook?.Name + ")"; }
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

		public void Prepare(KeyValuePair<Prospect, User> param)
        {
            Prospect = param.Key;
			User = param.Value;
        }
    }


}
