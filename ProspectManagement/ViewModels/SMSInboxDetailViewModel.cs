using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SMSInboxDetailViewModel : BaseViewModel, IMvxViewModel<KeyValuePair<SmsActivity, User>>
    {
        private SmsActivity _smsActivity;
        private User _user;

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;
        private MvxInteraction _showAlertInteraction = new MvxInteraction();
        public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

        private ObservableCollection<SmsActivity> _smsMessages;
        private String _smsMessageBody;
        //private HubConnection _hubConnection;
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
                    var activity = await _activityService.SendAdHocSMSAsync(SmsActivity.Prospect.ProspectAddressNumber, adhocTask);
                    if (activity != null)
                    {
                        SmsActivity sms = new SmsActivity()
                        {
                            ActivityId = activity.ActivityID,
                            InstanceId = activity.InstanceID,
                            ProspectAddressBook = SmsActivity.Prospect.ProspectAddressNumber,
                            ProspectCommunityId = SmsActivity.Prospect.ProspectCommunity.ProspectCommunityId,
                            SalespersonAddressBook = SmsActivity.Prospect.ProspectCommunity.SalespersonAddressNumber,
                            Direction = "outbound",
                            UpdatedDate = DateTime.UtcNow,
                            MessageBody = SmsMessageBody,
                            Prospect = SmsActivity.Prospect
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

        public ObservableCollection<SmsActivity> SmsMessages
        {
            get { return _smsMessages; }
            set
            {
                _smsMessages = value;
                RaisePropertyChanged(() => SmsMessages);
            }
        }

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }

        public SmsActivity SmsActivity
        {
            get { return _smsActivity; }
            set
            {
                _smsActivity = value;
            }
        }

        public SMSInboxDetailViewModel(IActivityService activityService, IProspectService prospectService, IMvxMessenger messenger, IMvxNavigationService navigationService, IUserService userService)
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
            var smsMessages = await _prospectService.GetProspectSMSActivityAsync(SmsActivity.Prospect.ProspectAddressNumber);
            SmsMessages = smsMessages.ToObservableCollection();
            _hideAlertInteraction.Raise();
        }

        public void Prepare(KeyValuePair<SmsActivity, User> param)
        {
            SmsActivity = param.Key;
            User = param.Value;
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
                ProspectAddressNumber = SmsActivity.Prospect.ProspectAddressNumber,
                SalespersonAddressNumber = SmsActivity.Prospect.ProspectCommunity.SalespersonAddressNumber,
                ProspectCommunityId = SmsActivity.Prospect.ProspectCommunity.ProspectCommunityId,
                Community = SmsActivity.Prospect.ProspectCommunity.CommunityNumber,
                Prospect = SmsActivity.Prospect,
                FollowUpStatus = "Completed",
                Notes = SmsMessageBody
            };
        }
    }
}
