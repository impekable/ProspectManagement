using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.InfiniteScroll;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class ProspectSMSViewModel : BaseViewModel, IMvxViewModel<Prospect>
    {
        public event EventHandler LoadingDataFromBackendStarted;
        public event EventHandler<int> LoadingDataFromBackendCompleted;
        private event EventHandler<int> _incrementalLoadFromBackendCompleted;
        private bool _loadingCompleted;

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;
        private MvxInteraction _showAlertInteraction = new MvxInteraction();
        public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

        private readonly IIncrementalCollectionFactory _collectionFactory;
        private Prospect _prospect;
        private ObservableCollection<SmsActivity> _smsMessages;
        private String _smsMessageBody;
        private IMvxCommand _sendSMSCommand;
        private IMvxCommand _closeCommand;

        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IProspectService _prospectService;
        private readonly IActivityService _activityService;
        private readonly IAuthenticator _authService;

        private int _page = 0;

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
            get
            {
                {
                    if (_smsMessages == null)
                    {
                        _incrementalLoadFromBackendCompleted += (sender, e) =>
                        {
                            var numRecordsFetched = Convert.ToInt16(e);
                            OnLoadingDataFromBackendCompleted(numRecordsFetched);
                            if (_smsMessages.Any(s => s.Unread) && !_loadingCompleted)
                                _prospectService.UpdateProspectSMSActivityAsync(Prospect.ProspectAddressNumber);
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
                                        var SmsMessagesList = await _prospectService.GetProspectSMSActivityAsync(Prospect.ProspectAddressNumber, authResult.AccessToken, Page, pageSize);
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

        public ProspectSMSViewModel(IAuthenticator authService, IIncrementalCollectionFactory collectionFactory, IActivityService activityService, IProspectService prospectService, IMvxMessenger messenger, IMvxNavigationService navigationService)
        {
            _authService = authService;
            _collectionFactory = collectionFactory;
            _activityService = activityService;
            _prospectService = prospectService;
            Messenger = messenger;
            _navigationService = navigationService;
            PageSize = 20;
        }

        public void Prepare(Prospect prospect)
        {
            Prospect = prospect;
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
