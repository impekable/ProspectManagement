 using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ProspectManagement.Core.ViewModels
{
    public class ProspectSMSViewModel : BaseViewModel, IMvxViewModel<Prospect>
    {
        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;
        private MvxInteraction _showAlertInteraction = new MvxInteraction();
        public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

        private Prospect _prospect;
        private ObservableCollection<SmsActivity> _smsMessages;
        private String _smsMessageBody;
        private HubConnection _hubConnection;
        private IMvxCommand _sendSMSCommand;
        private IMvxCommand _closeCommand;

        private readonly IUserService _userService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IProspectService _prospectService;
        private readonly IActivityService _activityService;

        public IMvxCommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(async () => { await _navigationService.Close(this); }));
            }
        }

        public IMvxCommand SendSMSCommand
        {
            get
            {
                return _sendSMSCommand ?? (_sendSMSCommand = new MvxCommand(async () =>
                {

                    var adhocTask = createAdHocActivity();
                    var activity = await _activityService.SendAdHocSMSAsync(Prospect.ProspectAddressNumber, adhocTask);
                    if (activity != null)
                    {
                        SmsActivity sms = new SmsActivity()
                        {
                            ActivityId = activity.ActivityID,
                            InstanceId = activity.InstanceID,
                            ProspectAddressBook = Prospect.ProspectAddressNumber,
                            ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
                            SalespersonAddressBook = Prospect.ProspectCommunity.SalespersonAddressNumber,
                            Direction = "outbound",
                            UpdatedDate = DateTime.UtcNow,
                            MessageBody = SmsMessageBody,
                            Prospect = Prospect
                        };
                        SmsMessages.Add(sms);

                        SmsMessageBody = "";
                    }

                    _hideAlertInteraction.Raise();
                }));
            }
        }

        public String SmsMessageBody
        {
            get { return _smsMessageBody; }
            set
            {
                _smsMessageBody = value;
                RaisePropertyChanged(() => SmsMessageBody);
            }
        }

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                RaisePropertyChanged(() => Prospect);
            }
        }

        public ObservableCollection<SmsActivity> SmsMessages
        {
            get { return _smsMessages; }
            set
            {
                _smsMessages = value;
                RaisePropertyChanged(() => SmsMessages);
            }
        }

        public ProspectSMSViewModel(IActivityService activityService, IProspectService prospectService, IMvxMessenger messenger, IMvxNavigationService navigationService, IUserService userService)
        {
            _activityService = activityService;
            _prospectService = prospectService;
            Messenger = messenger;
            _navigationService = navigationService;
            _userService = userService;  
        }

        public async override Task Initialize()
        {
            _showAlertInteraction.Raise();
            var smsMessages = await _prospectService.GetProspectSMSActivityAsync(Prospect.ProspectAddressNumber);
            SmsMessages = smsMessages.ToObservableCollection();
            _hideAlertInteraction.Raise();
        }

        public async void Prepare(Prospect prospect)
        {
            Prospect = prospect;

            //_hubConnection = new HubConnection("https://browsercallsweb.azurewebsites.net/");
            //var hubProxy = _hubConnection.CreateHubProxy("twilioCallHub");
            //hubProxy.On<string, string, string>("broadcastSmsMessage", (salesPhoneNumber, prospectPhoneNumber, message) =>
            //{
            //    SmsMessages.Add(new SMSMessage() { From = prospectPhoneNumber, To = salesPhoneNumber, MessageBody = message });
            //});

            //try
            //{
            //    await _hubConnection.Start();
            //    if (String.IsNullOrEmpty(_hubConnection.ConnectionId))
            //        Console.WriteLine("Not Connected");
            //    else
            //        Console.WriteLine("Connected ConnectionId =" + _hubConnection.ConnectionId);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Not Connected " + ex.Message);
            //}
        }

        private Activity createAdHocActivity()
        {
            return new Activity
            {
                ActivityType = "ADHOC",
                ContactMethod = "Text",
                Subject = "Ad Hoc SMS",
                TimeDateStart = DateTime.UtcNow,
                TimeDateEnd = DateTime.UtcNow,
                DateCompleted = DateTime.UtcNow,
                ProspectAddressNumber = Prospect.ProspectAddressNumber,
                SalespersonAddressNumber = Prospect.ProspectCommunity.SalespersonAddressNumber,
                ProspectCommunityId = Prospect.ProspectCommunity.ProspectCommunityId,
                Community = Prospect.ProspectCommunity.CommunityNumber,
                Prospect = Prospect,
                FollowUpStatus = "Completed",
                Notes = SmsMessageBody
            };
        }
    }
}
