using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class TrafficCardViewModel: BaseViewModel, IMvxViewModel<Prospect>
    {
        private readonly ITrafficCardResponseService _trafficCardResponseService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserService _userService;

		private MvxInteraction<TableRow> _updateRowInteraction = new MvxInteraction<TableRow>();
		public IMvxInteraction<TableRow> UpdateRowInteraction => _updateRowInteraction;

        private MvxInteraction _clearDetailsInteraction = new MvxInteraction();
        public IMvxInteraction ClearDetailsInteraction => _clearDetailsInteraction;

		private Prospect _prospect;
        private User _user;
        private List<TrafficCardResponse> _responses;

		private ICommand _showDetailTab;
		private ICommand _showCobuyerTab;
        private ICommand _selectionChangedCommand;

		public ICommand SelectionChangedCommand
		{
			get
			{
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<TrafficCardResponse>((response) =>  _navigationService.Navigate<TrafficCardQuestionViewModel, KeyValuePair<Prospect, TrafficCardResponse>>(new KeyValuePair<Prospect, TrafficCardResponse>(Prospect,  response))));
			}
		}

		public ICommand ShowDetailTab
		{
			get
			{
                return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<SplitDetailViewModel, Prospect>(Prospect)));
			}
		}
		public ICommand ShowCobuyerTab
		{
			get
			{
                return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<CobuyerViewModel, Prospect>(Prospect)));
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

		public List<TrafficCardResponse> Responses
		{
			get { return _responses; }
			set
			{
				_responses = value;
				RaisePropertyChanged(() => Responses);
			}
		}

        public TrafficCardViewModel(IMvxMessenger messenger, ITrafficCardResponseService trafficCardResponseService, IMvxNavigationService navigationService, IUserService userService)
        {
            Messenger = messenger;
			_trafficCardResponseService = trafficCardResponseService;
            _navigationService = navigationService;
            _userService = userService;

            Messenger.Subscribe<RefreshMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<TrafficCardResponseChangedMessage>(message => ResponseUpdated(message.ChangedResponse), MvxReference.Strong);
		}

		public void ResponseUpdated(TrafficCardResponse response)
		{
            var r = Responses.FirstOrDefault(res => res.ResponseNumber == response.ResponseNumber);
            if (r != null)
            {
                var request = new TableRow { TableRowToUpdate = Responses.IndexOf(r) };
                _updateRowInteraction.Raise(request);
            }
		}

        public void Prepare(Prospect prospect)
        {
            Prospect = prospect;
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();
            Analytics.TrackEvent("Traffic Card Viewed", new Dictionary<string, string>
            {
                {"Community", Prospect.ProspectCommunity.CommunityNumber + " " + Prospect.ProspectCommunity.Community.Description},
                {"SalesAssociate", Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Prospect.ProspectCommunity.SalespersonName},
                {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
            });
            Responses = await _trafficCardResponseService.GetTrafficCardResponsesForProspect(Prospect.ProspectAddressNumber, false);
		}
    }
}
