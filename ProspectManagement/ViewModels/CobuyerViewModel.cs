using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Models;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;


namespace ProspectManagement.Core.ViewModels
{
    public class CobuyerViewModel : BaseViewModel
    {
        private Prospect _prospect;
        private ICommand _showDetailTab;
        private ICommand _showTrafficCardTab;
        private ICommand _showCobuyerDetail;
        private ICommand _showCobuyerTab;
        private ICommand _selectionChangedCommand;
        private ICommand _addCobuyerCommand;

        private readonly ICobuyerService _cobuyerService;
        private readonly IProspectCache _prospectCache;
        protected IMvxMessenger Messenger;

        private MvxInteraction<TableRow> _updateRowInteraction = new MvxInteraction<TableRow>();
        public IMvxInteraction<TableRow> UpdateRowInteraction => _updateRowInteraction;

        private MvxInteraction _addRowInteraction = new MvxInteraction();
        public IMvxInteraction AddRowInteraction => _addRowInteraction;

        private List<Cobuyer> _cobuyers;

        public ICommand ShowDetailTab
        {
            get
            {
                return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<SplitDetailViewModel>(_prospect)));
            }
        }

        public ICommand ShowTrafficCardTab
        {
            get
            {
                return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<TrafficCardViewModel>(_prospect)));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Cobuyer>((cobuyer) => { _prospectCache.SaveCobuyerToCache(cobuyer); ShowViewModel<CobuyerDetailViewModel>(cobuyer); }));
            }
        }

        public ICommand AddCobuyerCommand
        {
            get
            {
                return _addCobuyerCommand ?? (_addCobuyerCommand = new MvxCommand(() => { ShowViewModel<CobuyerDetailViewModel>(new Cobuyer() { ProspectAddressNumber = _prospect.ProspectAddressNumber }); }));
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

        public List<Cobuyer> CobuyersList
        {
            get { return _cobuyers; }
            set
            {
                _cobuyers = value;
                RaisePropertyChanged(() => CobuyersList);
            }
        }

        public CobuyerViewModel(IMvxMessenger messenger, IProspectCache prospectCache, ICobuyerService cobuyerService)
        {
            Messenger = messenger;
            _cobuyerService = cobuyerService;
            _prospectCache = prospectCache;

            Messenger.Subscribe<CobuyerChangedMessage>(async message => CobuyerUpdated(message.UpdatedCobuyer), MvxReference.Strong);
            Messenger.Subscribe<CobuyerAddedMessage>(async message => CobuyerAdded(message.AddedCobuyer), MvxReference.Strong);
        }

        public async void CobuyerUpdated(Cobuyer cobuyer)
        {
            var r = CobuyersList.FirstOrDefault(res => res.CobuyerAddressNumber == cobuyer.CobuyerAddressNumber);
            if (r != null)
            {
                var request = new TableRow { TableRowToUpdate = CobuyersList.IndexOf(r) };
                _updateRowInteraction.Raise(request);
            }
        }

        public async void CobuyerAdded(Cobuyer cobuyer)
        {
            CobuyersList.Add(cobuyer);
            _addRowInteraction.Raise();
        }

        public override async void Start()
        {
            base.Start();
            await ReloadDataAsync();
        }

        public async void Init(Prospect prospect)
        {
            Prospect = prospect;
            CobuyersList = await _cobuyerService.GetCobuyersForProspectAsync(_prospect.ProspectAddressNumber);
        }
    }
}
