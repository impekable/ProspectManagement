using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class BuyerDecisionsViewModel : BaseViewModel, IMvxViewModel<Prospect>
    {
        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private Prospect _prospect;
        private ObservableCollection<UserDefinedCode> _rankings;
        private BuyerDecisions _buyerDecisions;
        private UserDefinedCode _activeRanking;
        private ICommand _saveCommand;
        private ICommand _closeCommand;
        private readonly IDialogService _dialogService;
        private readonly IBuyerDecisionsService _buyerDecisionsService;
        private readonly IUserDefinedCodeService _userDefinedCodeService;
        private readonly IMvxNavigationService _navigationService;

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                RaisePropertyChanged(() => Prospect);
            }
        }

        public BuyerDecisions BuyerDecisions
        {
            get { return _buyerDecisions; }
            set
            {
                _buyerDecisions = value;
                RaisePropertyChanged(() => BuyerDecisions);
            }
        }

        public ObservableCollection<UserDefinedCode> Rankings
        {
            get { return _rankings; }
            set
            {
                _rankings = value;
                RaisePropertyChanged(() => Rankings);
            }
        }

        public UserDefinedCode ActiveRanking
        {
            get { return _activeRanking; }
            set
            {
                _activeRanking = value;
                BuyerDecisions.Ranking = _activeRanking.Code;
                RaisePropertyChanged(() => ActiveRanking);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new MvxCommand(async () =>
                {
                    var decisionsUpdated = await _buyerDecisionsService.UpdateBuyerDecisionsAsync(BuyerDecisions);
                    _hideAlertInteraction.Raise();

                    if (decisionsUpdated)
                    {
                        //Messenger.Publish(new ProspectChangedMessage(this) { UpdatedProspect = Prospect });

                        await _navigationService.Close(this);
                    }
                }));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(async () => { await _navigationService.Close(this); }));
            }
        }

        public BuyerDecisionsViewModel(IUserDefinedCodeService userDefinedCodeService, IDialogService dialogService, IBuyerDecisionsService buyerDecisionsService, IMvxNavigationService navigationService)
        {
            _dialogService = dialogService;
            _buyerDecisionsService = buyerDecisionsService;
            _userDefinedCodeService = userDefinedCodeService;
            _navigationService = navigationService;
        }

        public void Prepare(Prospect prospect)
        {
            Prospect = prospect;
        }

        public override async Task Initialize()
        {
            Rankings = (await _userDefinedCodeService.GetRankingUserDefinedCodes()).ToObservableCollection();
            BuyerDecisions = await _buyerDecisionsService.GetBuyerDecisionsAsync(Prospect.ProspectAddressNumber);

            ActiveRanking = BuyerDecisions != null ? Rankings.FirstOrDefault(p => p.Code == BuyerDecisions.Ranking) : null;
            //ActiveRanking = ranking != null ? ranking.Code : null;
        }
    }
}
