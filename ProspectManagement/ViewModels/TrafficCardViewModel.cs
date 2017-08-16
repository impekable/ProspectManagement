using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class TrafficCardViewModel: BaseViewModel
    {
        private readonly ITrafficCardResponseService _trafficCardResponseService;
        private readonly IProspectCache _prospectCache;
        protected IMvxMessenger Messenger;

		private MvxInteraction<TableRow> _updateRowInteraction = new MvxInteraction<TableRow>();
		public IMvxInteraction<TableRow> UpdateRowInteraction => _updateRowInteraction;

		private Prospect _prospect;
        private List<TrafficCardResponse> _responses;

		private ICommand _showDetailTab;
		private ICommand _showCobuyerTab;
        private ICommand _selectionChangedCommand;

		public ICommand SelectionChangedCommand
		{
			get
			{
				return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<TrafficCardResponse>((response) => { _prospectCache.SaveTrafficCardResponseToCache(_prospect.ProspectAddressNumber, response); ShowViewModel<TrafficCardQuestionViewModel>(_prospect); }));
			}
		}

		public ICommand ShowDetailTab
		{
			get
			{
				return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<SplitDetailViewModel>(_prospect)));
			}
		}
		public ICommand ShowCobuyerTab
		{
			get
			{
				return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<CobuyerViewModel>(_prospect)));
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

		public TrafficCardViewModel(IMvxMessenger messenger, IProspectCache prospectCache, ITrafficCardResponseService trafficCardResponseService)
        {
            Messenger = messenger;
			_trafficCardResponseService = trafficCardResponseService;
            _prospectCache = prospectCache;

            Messenger.Subscribe<TrafficCardResponseChangedMessage>(async message => ResponseUpdated(message.ChangedResponse), MvxReference.Strong);
		}

		public async void ResponseUpdated(TrafficCardResponse response)
		{
            var r = Responses.FirstOrDefault(res => res.ResponseNumber == response.ResponseNumber);
            if (r != null)
            {
                var request = new TableRow { TableRowToUpdate = Responses.IndexOf(r) };
                _updateRowInteraction.Raise(request);
            }
		}

		public override async void Start()
		{
			base.Start();
			await ReloadDataAsync();
		}

		public async void Init(Prospect prospect)
		{
			_prospect = prospect;
			Responses = await _trafficCardResponseService.GetTrafficCardResponsesForProspect(_prospect.ProspectAddressNumber, false);
		}
    }
}
