using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Models;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using MvvmCross.Navigation;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;

namespace ProspectManagement.Core.ViewModels
{
    public class CobuyerViewModel : BaseViewModel, IMvxViewModel<Prospect>
    {
        private Prospect _prospect;
        private User _user;
        private ICommand _showDetailTab;
        private ICommand _showTrafficCardTab;
        private ICommand _selectionChangedCommand;
        private ICommand _addCobuyerCommand;

        private readonly IUserService _userService;
        private readonly ICobuyerService _cobuyerService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;

        private MvxInteraction<TableRow> _updateRowInteraction = new MvxInteraction<TableRow>();
        public IMvxInteraction<TableRow> UpdateRowInteraction => _updateRowInteraction;

        private MvxInteraction _addRowInteraction = new MvxInteraction();
        public IMvxInteraction AddRowInteraction => _addRowInteraction;

        private MvxInteraction _clearDetailsInteraction = new MvxInteraction();
        public IMvxInteraction ClearDetailsInteraction => _clearDetailsInteraction;

        private List<Cobuyer> _cobuyers;

        public ICommand ShowDetailTab
        {
            get
            {
				return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<SplitDetailViewModel, KeyValuePair<Prospect, User>>(new KeyValuePair<Prospect, User>(_prospect, _user))));
            }
        }

        public ICommand ShowTrafficCardTab
        {
            get
            {
                return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<TrafficCardViewModel, Prospect>(_prospect)));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Cobuyer>((cobuyer) => _navigationService.Navigate<CobuyerDetailViewModel, Cobuyer>(cobuyer)));
            }
        }

        public ICommand AddCobuyerCommand
        {
            get
            {
                return _addCobuyerCommand ?? (_addCobuyerCommand = new MvxCommand(() => _navigationService.Navigate<CobuyerDetailViewModel, Cobuyer>(new Cobuyer() { ProspectAddressNumber = _prospect.ProspectAddressNumber, Prospect = Prospect })));
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

        public CobuyerViewModel(IMvxMessenger messenger, ICobuyerService cobuyerService, IMvxNavigationService navigationService, IUserService userService)
        {
            Messenger = messenger;
            _cobuyerService = cobuyerService;
            _navigationService = navigationService;
            _userService = userService;

            Messenger.Subscribe<RefreshMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<UserLogoutMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<CobuyerChangedMessage>(message => CobuyerUpdated(message.UpdatedCobuyer), MvxReference.Strong);
            Messenger.Subscribe<CobuyerAddedMessage>(message => CobuyerAdded(message.AddedCobuyer), MvxReference.Strong);
        }

        public void CobuyerUpdated(Cobuyer cobuyer)
        {
            var r = CobuyersList.FirstOrDefault(res => res.CobuyerAddressNumber == cobuyer.CobuyerAddressNumber);
            if (r != null)
            {
                var request = new TableRow { TableRowToUpdate = CobuyersList.IndexOf(r) };
                _updateRowInteraction.Raise(request);
            }
        }

        public void CobuyerAdded(Cobuyer cobuyer)
        {
            CobuyersList.Add(cobuyer);
            _addRowInteraction.Raise();
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();
            Analytics.TrackEvent("Cobuyers Viewed", new Dictionary<string, string>
            {
                {"Community", Prospect.ProspectCommunity.CommunityNumber + " " + Prospect.ProspectCommunity.Community.Description},
                {"SalesAssociate", Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Prospect.ProspectCommunity.SalespersonName},
                {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
            });
            CobuyersList = await _cobuyerService.GetCobuyersForProspectAsync(_prospect);
        }

        public void Prepare(Prospect prospect)
        {
            Prospect = prospect;
        }
    }
}
