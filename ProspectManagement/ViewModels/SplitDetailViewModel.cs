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
    public class SplitDetailViewModel : BaseViewModel
    {
        private Prospect _prospect;
        private bool _prospectAssigned;
        private string _workPhoneLabel;
        private string _homePhoneLabel;
        private string _mobilePhoneLabel;
        private ICommand _editProspectCommand;
        private ICommand _assignCommand;
        private ICommand _showCobuyerTab;
        private ICommand _showTrafficCardTab;
        private readonly IProspectService _prospectService;
        private readonly IProspectCache _prospectCache;
        protected IMvxMessenger Messenger;

		private MvxInteraction _showAlertInteraction = new MvxInteraction();
		public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

		private MvxInteraction _hideAlertInteraction = new MvxInteraction();
		public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

		public SplitDetailViewModel(IMvxMessenger messenger, IProspectCache prospectCache, IProspectService prospectService)
        {
            Messenger = messenger;
            _prospectService = prospectService;
            _prospectCache = prospectCache;

            Messenger.Subscribe<ProspectChangedMessage>(async message => Init(message.UpdatedProspect), MvxReference.Strong);
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

					if (_assignedTo > 0)
                    {
                        ProspectAssigned = true;
                        Prospect.ProspectCommunity.SalespersonAddressNumber = _assignedTo;
                        Messenger.Publish(new ProspectAssignedMessage(this) { AssignedProspect = Prospect });
                    }
				}));
            }
        }

		public ICommand EditProspectCommand
		{
			get
			{
                return _editProspectCommand ?? (_editProspectCommand = new MvxCommand(() => { ShowViewModel<EditProspectViewModel>(_prospect); }));
			}
		}

        public ICommand ShowCobuyerTab
        {
            get
            {
                return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<CobuyerViewModel>(_prospect)));
            }
        }

        public ICommand ShowTrafficCardTab
        {
            get
            {
                return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<TrafficCardViewModel>(_prospect)));
            }
        }

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                ProspectAssigned = _prospect.ProspectCommunity.SalespersonAddressNumber > 0;
                WorkPhoneLabel = _prospect.WorkPhoneNumber == null || string.IsNullOrEmpty(_prospect.WorkPhoneNumber.Phone) ? null : "Work";
                MobilePhoneLabel = _prospect.MobilePhoneNumber == null || string.IsNullOrEmpty(_prospect.MobilePhoneNumber.Phone) ? null : "Mobile";
                HomePhoneLabel = _prospect.HomePhoneNumber == null || string.IsNullOrEmpty(_prospect.HomePhoneNumber.Phone) ? null : "Home";
                RaisePropertyChanged(() => Prospect);
            }
        }

        public bool ProspectAssigned
        {
            get { return _prospectAssigned; }
            set
            {
                _prospectAssigned = value;
                RaisePropertyChanged(() => ProspectAssigned);
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

        public async void Init(Prospect prospect)
        {
            Prospect = _prospectCache.GetProspectFromCache(prospect.ProspectAddressNumber);
            if (Prospect == null)
            {
                Prospect = await _prospectService.GetProspectAsync(prospect.ProspectAddressNumber);
            }
        }
    }


}
