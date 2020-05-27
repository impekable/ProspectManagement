using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Microsoft.AspNet.SignalR.Client;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SMSTaskViewModel : BaseViewModel, IMvxViewModel<Activity>
    {
        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;
        private MvxInteraction _showAlertInteraction = new MvxInteraction();
        public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

        private Activity _activity;
        private User _user;
        private ObservableCollection<SmsActivity> _smsMessages;
        private String _smsMessageBody;
        //private HubConnection _hubConnection;
        private IMvxCommand _sendSMSCommand;
        private IMvxCommand _closeCommand;

        private readonly IUserService _userService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IActivityService _activityService;
        private readonly IDialogService _dialogService;

        public IMvxCommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(async () =>
                {
                    await _navigationService.Close(this);
                }));
             }
        }

        public IMvxCommand SendSMSCommand
        {
            get
            {
                return _sendSMSCommand ?? (_sendSMSCommand = new MvxCommand(async () =>
                {
                    if (!String.IsNullOrEmpty(SmsMessageBody))
                    {
                        _showAlertInteraction.Raise();
                        Activity.DateCompleted = DateTime.UtcNow;
                        Activity.FollowUpStatus = "Completed";
                        Activity.FollowUpMethod = "Text";
                        Activity.Notes = SmsMessageBody;
                        var success = await _activityService.SendSMSAsync(Activity);
                        if (success)
                        {
                            SmsActivity sms = new SmsActivity()
                            {
                                ActivityId = Activity.ActivityID,
                                InstanceId = Activity.InstanceID,
                                ProspectAddressBook = Activity.ProspectAddressNumber,
                                ProspectCommunityId = Activity.ProspectCommunityId,
                                SalespersonAddressBook = Activity.SalespersonAddressNumber,
                                Direction = "outbound",
                                UpdatedDate = DateTime.UtcNow,
                                MessageBody = SmsMessageBody,
                                Prospect = Activity.Prospect
                            };
                            SmsMessages.Add(sms);

                            SmsMessageBody = "";

                            Messenger.Publish(new TaskCompletedMessage(this) { Activity = Activity });

                            await _dialogService.ShowAlertAsync("Message sent", "Task Completed", "OK");
                            await _navigationService.Close(this);
                        }
                        _hideAlertInteraction.Raise();
                    }
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

        public Activity Activity
        {
            get { return _activity; }
            set
            {
                _activity = value;
                RaisePropertyChanged(() => Activity);
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

        public SMSTaskViewModel(IDialogService dialogService, IMvxMessenger messenger, IActivityService activityService, IMvxNavigationService navigationService, IUserService userService)
        {
            _dialogService = dialogService;
            Messenger = messenger;
            _navigationService = navigationService;
            _userService = userService;
            _activityService = activityService;
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();
            Analytics.TrackEvent("SMS Task Viewed", new Dictionary<string, string>
            {
                {"Community", Activity.Prospect.ProspectCommunity.CommunityNumber + " " }, // + Activity.Prospect.ProspectCommunity.Community.Description},
                {"SalesAssociate", Activity.Prospect.ProspectCommunity.SalespersonAddressNumber + " " }, // + Prospect.ProspectCommunity.SalespersonName},
                {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
            });

            SmsMessages = new ObservableCollection<SmsActivity>();
        }

        public async void Prepare(Activity activity)
        {
            Activity = activity;
            SmsMessageBody = Activity.TemplateData;

            //_hubConnection = new HubConnection("https://browsercallsweb.azurewebsites.net/");
            //var hubProxy = _hubConnection.CreateHubProxy("twilioCallHub");
            //hubProxy.On<string, string, string>("broadcastSmsMessage", (salesPhoneNumber, prospectPhoneNumber, message) =>
            //{
            //    SmsMessages.Add(new SmsActivity() { From = prospectPhoneNumber, To = salesPhoneNumber, MessageBody = message });
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

        //private Activity createAdHocActivity(string contactMethod, string subject)
        //{
        //    return new Activity
        //    {
        //        ActivityType = "ADHOC",
        //        ContactMethod = contactMethod,
        //        Subject = subject,
        //        TimeDateStart = DateTime.UtcNow,
        //        TimeDateEnd = DateTime.UtcNow,
        //        DateCompleted = DateTime.UtcNow,
        //        ProspectAddressNumber = Activity.Prospect.ProspectAddressNumber,
        //        SalespersonAddressNumber = Activity.SalespersonAddressNumber,
        //        ProspectCommunityId = Activity.Prospect.ProspectCommunity.ProspectCommunityId,
        //        Community = Activity.Prospect.ProspectCommunity.CommunityNumber,
        //        Prospect = Activity.Prospect
        //    };
        //}
    }
}
