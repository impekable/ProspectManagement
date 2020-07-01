using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
    public class SMSInboxDetailViewModel : BaseViewModel, IMvxViewModel<KeyValuePair<SmsActivity, User>>
    {
        public event EventHandler LoadingDataFromBackendStarted;
        public event EventHandler<int> LoadingDataFromBackendCompleted;
        private event EventHandler<int> _incrementalLoadFromBackendCompleted;
        private MvxSubscriptionToken messengerToken;

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;
        private MvxInteraction _showAlertInteraction = new MvxInteraction();
        public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

        private readonly IIncrementalCollectionFactory _collectionFactory;
        private ObservableCollection<SmsActivity> _smsMessages;
        private String _smsMessageBody;
        private IMvxCommand _sendSMSCommand;
        private IMvxCommand _closeCommand;

        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IProspectService _prospectService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticator _authService;
        private readonly IActivityService _activityService;

        private int _page = 0;
        private bool _loadingCompleted;

        public int PageSize { get; set; }

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
                    if (SmsActivity.Prospect.FollowUpSettings.ConsentToText)
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

                            Messenger.Publish(new SMSSentMessage(this) { SMSActivitySent = sms });

                        }
                    }
                    else
                    {
                        await _dialogService.ShowAlertAsync("Prospect has not consented to text", "Message not Sent", "Ok");
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
            get
            {
                {
                    if (_smsMessages == null)
                    {
                        _incrementalLoadFromBackendCompleted += (sender, e) =>
                        {
                            var numRecordsFetched = Convert.ToInt16(e);
                            OnLoadingDataFromBackendCompleted(numRecordsFetched);
                            if (numRecordsFetched < PageSize)
                                _loadingCompleted = true;
                        };

                        _smsMessages = _collectionFactory.GetCollection(async (count, pageSize) =>
                        {
                            var newSmsMessages = new ObservableCollection<SmsActivity>();
                            if (!_loadingCompleted)
                            {
                                var authResult = await _authService.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);

                                if (authResult != null && !String.IsNullOrEmpty(authResult.AccessToken))
                                {
                                    await Task.Run(async () =>
                                    {
                                        Page++;
                                        var SmsMessagesList = await _prospectService.GetProspectSMSActivityAsync(SmsActivity.Prospect.ProspectAddressNumber, authResult.AccessToken, Page, pageSize);
                                        newSmsMessages = SmsMessagesList.ToObservableCollection();
                                    });
                                }

                            }
                            return newSmsMessages;
                        }, _incrementalLoadFromBackendCompleted, PageSize);
                    }
                }
                return _smsMessages;
            }
            set
            {
                _smsMessages = value;
                RaisePropertyChanged(() => SmsMessages);
            }
        }

        public User User { get; set; }

        public SmsActivity SmsActivity { get; set; }

        public SMSInboxDetailViewModel(IAuthenticator authService, IDialogService dialogService, IIncrementalCollectionFactory collectionFactory, IActivityService activityService, IProspectService prospectService, IMvxMessenger messenger, IMvxNavigationService navigationService)
        {
            _dialogService = dialogService;
            _authService = authService;
            _activityService = activityService;
            _prospectService = prospectService;
            Messenger = messenger;
            _navigationService = navigationService;
            _collectionFactory = collectionFactory;
            PageSize = 20;
            messengerToken = Messenger.Subscribe<SMSReceivedMessage>(message => AddMessage(message));
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            if (messengerToken != null)
            {
                Messenger.Unsubscribe<SMSReceivedMessage>(messengerToken);
                messengerToken = null;
            }
        }

        private async void AddMessage(SMSReceivedMessage message)
        {
            //receiving a message from prospect whose messages are currently being viewed 
            if (message.SMSActivityReceived.ProspectAddressBook == SmsActivity.ProspectAddressBook)
            {
                //_showAlertInteraction.Raise();
                SmsMessages.Add(message.SMSActivityReceived);
                _hideAlertInteraction.Raise();
                var success = await _prospectService.UpdateProspectSMSActivityAsync(SmsActivity.ProspectAddressBook);
            }
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

        public void OnLoadingDataFromBackendStarted()
        {
            LoadingDataFromBackendStarted?.Invoke(null, EventArgs.Empty);
        }

        public void OnLoadingDataFromBackendCompleted(int numRecordsFetched)
        {
            LoadingDataFromBackendCompleted?.Invoke(null, numRecordsFetched);
        }

    }
}
