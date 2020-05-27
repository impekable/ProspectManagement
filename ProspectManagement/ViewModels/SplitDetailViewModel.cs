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
using Microsoft.AppCenter.Analytics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using ProspectManagement.Core.Models.App;
using System.Collections.ObjectModel;

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

        private ICommand _showCobuyerTab;
        private ICommand _showTrafficCardTab;
        private ICommand _showContactHistoryTab;
        private readonly IProspectService _prospectService;
        private readonly IActivityService _activityService;

        protected IMvxMessenger Messenger;
        private readonly IDialogService _dialogService;
        private readonly IMvxNavigationService _navigationService;
        private readonly ITwilioService _twilioService;
        private readonly IMvxPhoneCallTask _phoneCallTask;
        private readonly IMvxComposeEmailTaskEx _emailTask;

        private IMvxCommand _smsCommand;
        private IMvxCommand<String> _mobilePhoneCallCommand;
        private IMvxCommand<String> _workPhoneCallCommand;
        private IMvxCommand<String> _twilioPhoneCallCommand;
        private IMvxCommand<String> _homePhoneCallCommand;
        private IMvxCommand _composeEmailCommand;

        private IMvxCommand _showRankingCommand;

        private ObservableCollection<KeyValuePair<string, string>> _phones;
        private KeyValuePair<string, string> _selectedCall;

        public ObservableCollection<KeyValuePair<string, string>> Phones
        {
            get { return _phones; }
            set
            {
                _phones = value;
                RaisePropertyChanged(() => Phones);
            }
        }

        public KeyValuePair<string, string> SelectedCall
        {
            get { return _selectedCall; }
            set
            {
                _selectedCall = value;
                if (_selectedCall.Key.Equals("Mobile"))
                    if (User.UsingTelephony)
                        TwilioPhoneCallCommand.Execute(Prospect.MobilePhoneNumber.Phone);
                    else
                        CallProspect(Prospect.MobilePhoneNumber.Phone);
                else if (_selectedCall.Key.Equals("Home"))
                    if (User.UsingTelephony)
                        TwilioPhoneCallCommand.Execute(Prospect.HomePhoneNumber.Phone);
                    else
                        CallProspect(Prospect.HomePhoneNumber.Phone);
                else if (_selectedCall.Key.Equals("Work"))
                    if (User.UsingTelephony)
                        TwilioPhoneCallCommand.Execute(Prospect.WorkPhoneNumber.Phone);
                    else
                        CallProspect(Prospect.WorkPhoneNumber.Phone);
                RaisePropertyChanged(() => SelectedCall);
            }
        }

        public IMvxCommand<String> TwilioPhoneCallCommand
        {
            get
            {
                return _twilioPhoneCallCommand ?? (_twilioPhoneCallCommand = new MvxCommand<String>(async (param) =>
                {

                    //var request = new TwilioCallParameters
                    //{
                    //    AccessToken = await _twilioService.GetClientToken(User.UserId),
                    //    ToPhoneNumber = Prospect.WorkPhoneNumber.Phone,
                    //    FromPhoneNumber = User.TwilioPhoneNumber.Phone
                    //};

                    //_makeCallInteraction.Raise(request);

                    TwilioClient.Init(Constants.PrivateKeys.TwilioAccountSid, Constants.PrivateKeys.TwilioAuthToken);

                    var call = CallResource.Create(
                        from: new Twilio.Types.PhoneNumber(Prospect.ProspectCommunity.Community.SalesOffice.TwilioPhoneNumber),
                        to: new Twilio.Types.PhoneNumber(User.MobilePhoneNumber.Phone),
                        machineDetection: "Enable",
                        url: new Uri($"https://optoutdv.khov.com/E1CRMWebApp/Call/Connect?phoneNumber={param}")
                    );

                    if (!string.IsNullOrEmpty(call.Sid))
                    {
                        var callActivity = new Activity()
                        {
                            ProspectAddressNumber = Prospect.ProspectAddressNumber,
                            SalespersonAddressNumber = User.AddressNumber,
                            ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
                            CallSid = call.Sid
                        };
                        var callActivityResult = await _activityService.LogCallAsync(Prospect.ProspectAddressNumber, callActivity);
                        if (!string.IsNullOrEmpty(callActivityResult.ActivityID))
                        {
                            var phoneCallActivity = new PhoneCallActivity()
                            {
                                FromPhoneNumber = User.MobilePhoneNumber.Phone,
                                ToPhoneNumber = param,
                                PhoneCallActivityId = callActivityResult.ActivityID,
                                PhoneCallInstanceId = callActivityResult.InstanceID,
                                CallSid = call.Sid,
                                ProspectAddressBook = Prospect.ProspectAddressNumber,
                                ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
                                SalespersonAddressBook = User.AddressNumber,
                                CallTime = DateTime.UtcNow,
                                Community = Prospect.ProspectCommunity.CommunityNumber
                            };
                            await _navigationService.Close(this);
                            await _navigationService.Navigate<CallResultViewModel, PhoneCallActivity>(phoneCallActivity);
                        }
                    }
                }));
            }
        }

        public IMvxCommand ShowRankingCommand
        {
            get
            {
                return _showRankingCommand ?? (_showRankingCommand = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<BuyerDecisionsViewModel, Prospect>(Prospect)));
            }
        }

        public IMvxCommand SMSCommand
        {
            get
            {
                return _smsCommand ?? (_smsCommand = new MvxCommand<Prospect>((prospect) =>
                {
                    _navigationService.Navigate<ProspectSMSViewModel, Prospect>(Prospect);
                }
                ));
            }
        }

        public IMvxCommand<String> MobilePhoneCallCommand => _mobilePhoneCallCommand ?? (_mobilePhoneCallCommand = new MvxCommand<String>(param =>
            {
                if (User.UsingTelephony)
                    TwilioPhoneCallCommand.Execute(Prospect.MobilePhoneNumber.Phone);
                else
                    CallProspect(Prospect.MobilePhoneNumber.Phone);
            },
            param => { return AllowCalling; }
         ));

        public IMvxCommand<String> WorkPhoneCallCommand => _workPhoneCallCommand ?? (_workPhoneCallCommand = new MvxCommand<String>(param =>
            {
                if (User.UsingTelephony)
                    TwilioPhoneCallCommand.Execute(Prospect.WorkPhoneNumber.Phone);
                else
                    CallProspect(Prospect.WorkPhoneNumber.Phone);
            },
            param => { return AllowCalling; }
        ));

        public IMvxCommand<String> HomePhoneCallCommand => _homePhoneCallCommand ?? (_homePhoneCallCommand = new MvxCommand<String>(param =>
            {
                if (User.UsingTelephony)
                    TwilioPhoneCallCommand.Execute(Prospect.HomePhoneNumber.Phone);
                else
                    CallProspect(Prospect.HomePhoneNumber.Phone);
            },
            param => { return AllowCalling; }
        ));

        public IMvxCommand ComposeEmailCommand
        {
            get
            {
                return _composeEmailCommand ?? (_composeEmailCommand = new MvxCommand(ComposeEmailToProspect, () => AllowEmailing));
            }
        }

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
                Community = Prospect.ProspectCommunity.CommunityNumber,
                Prospect = Prospect
            };
        }

        private void ComposeEmailToProspect()
        {
            if (_emailTask.CanSendEmail)
            {
                var activity = createAdHocActivity("Email", "Emailed from App");
                _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
                _emailTask.ComposeEmail(_prospect.Email.EmailAddress);
                Analytics.TrackEvent("Emailed From App", new Dictionary<string, string>
                    {
                        {"SalesAssociate", _prospect.ProspectCommunity.SalespersonAddressNumber.ToString() + " " + _prospect.ProspectCommunity.SalespersonName },
                        {"Community", _prospect.ProspectCommunity.CommunityNumber + " " + _prospect.ProspectCommunity.Community.Description},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                });

            }
            else
            {
                _dialogService.ShowAlertAsync("Please configure email on this device and then try again.", "Email Not Configured", "Close");
            }
        }

        private void CallProspect(string phoneNumber)
        {
            var activity = createAdHocActivity("Phone", "Called from App");
            _navigationService.Navigate<AddActivityViewModel, Activity>(activity);
            _phoneCallTask.MakePhoneCall(_prospect.Name, phoneNumber);
            Analytics.TrackEvent("Called From App", new Dictionary<string, string>
                    {
                        {"SalesAssociate", _prospect.ProspectCommunity.SalespersonAddressNumber.ToString() + " " + _prospect.ProspectCommunity.SalespersonName },
                        {"Community", _prospect.ProspectCommunity.CommunityNumber + " " + _prospect.ProspectCommunity.Community.Description},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                });
        }

        public bool AllowTexting
        {
            get { return Prospect.FollowUpSettings.ConsentToText && Assigned; }
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

        private MvxInteraction<TwilioCallParameters> _makeCallInteraction = new MvxInteraction<TwilioCallParameters>();

        // need to expose it as a public property for binding (only IMvxInteraction is needed in the view)
        public IMvxInteraction<TwilioCallParameters> MakeCallInteraction => _makeCallInteraction;

        public SplitDetailViewModel(IActivityService activityService, ITwilioService twilioService, IDialogService dialogService, IMvxComposeEmailTaskEx emailTask, IMvxPhoneCallTask phoneCallTask, IMvxMessenger messenger, IProspectService prospectService, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _prospectService = prospectService;
            _navigationService = navigationService;
            _phoneCallTask = phoneCallTask;
            _emailTask = emailTask;
            _dialogService = dialogService;
            _twilioService = twilioService;
            _activityService = activityService;

            Messenger.Subscribe<RefreshMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<ProspectChangedMessage>(message => Prepare(new KeyValuePair<Prospect, User>(message.UpdatedProspect, User)), MvxReference.Strong);
            Messenger.Subscribe<UserLogoutMessage>(message => UserLogout(), MvxReference.Strong);
            Messenger.Subscribe<ActivityAddedMessage>(message => ActivityAdded(message.AddedActivity), MvxReference.Strong);
            Messenger.Subscribe<ProspectChangedMessage>(message =>
                        {
                            RaisePropertyChanged(() => EmailEntered);
                            RaisePropertyChanged(() => StreetAddressEntered);
                            RaisePropertyChanged(() => MobilePhoneEntered);
                            RaisePropertyChanged(() => HomePhoneEntered);
                            RaisePropertyChanged(() => WorkPhoneEntered);
                            RaisePropertyChanged(() => AllowCalling);
                            RaisePropertyChanged(() => AllowEmailing);
                            RaisePropertyChanged(() => AllowTexting);
                        },
                        MvxReference.Strong);

        }

        public void UserLogout()
        {
            _clearDetailsInteraction.Raise();
        }

        public void ActivityAdded(Activity activity)
        {
            if (activity.ProspectAddressNumber == Prospect.ProspectAddressNumber)
            {
                if (activity.ActivityType.Equals("VISIT") || activity.ActivityType.Equals("APPOINTMENT"))
                {
                    Prospect.ProspectCommunity.AddressType = "Prospect";
                    RaisePropertyChanged(() => IsLead);
                    RaisePropertyChanged(() => AssignedProspect);
                    RaisePropertyChanged(() => IsLeadWithAppointment);
                    RaisePropertyChanged(() => AssignedWithoutAppointment);
                    _assignedProspectInteraction.Raise();
                }
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
                        RaisePropertyChanged(() => AllowTexting);
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



        public ICommand EditProspectCommand
        {
            get
            {
                return _editProspectCommand ?? (_editProspectCommand = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<EditProspectViewModel, Prospect>(_prospect)));
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
                RaisePropertyChanged(() => AllowCalling);
                RaisePropertyChanged(() => AllowEmailing);
                RaisePropertyChanged(() => AllowTexting);
            }
        }

        public bool AssignedProspect
        {
            get { return Assigned && !IsLead; }
        }

        public bool AssignedWithoutAppointment
        {
            get { return AssignedProspect || (IsLead && !Prospect.ProspectCommunity.AppointmentStatus.Equals("Pending")); }
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

        public async void Prepare(KeyValuePair<Prospect, User> param)
        {
            Prospect = param.Key;
            User = param.Value;
            RaisePropertyChanged(() => AllowCalling);
            RaisePropertyChanged(() => AllowEmailing);
            RaisePropertyChanged(() => AllowTexting);
            Phones = new ObservableCollection<KeyValuePair<string, string>>();
            if (HomePhoneEntered)
                Phones.Add(new KeyValuePair<string, string>("Home", Prospect.HomePhoneNumber.Phone));
            if (MobilePhoneEntered)
                Phones.Add(new KeyValuePair<string, string>("Mobile", Prospect.MobilePhoneNumber.Phone));
            if (WorkPhoneEntered)
                Phones.Add(new KeyValuePair<string, string>("Work", Prospect.WorkPhoneNumber.Phone));
        }

    }
}
