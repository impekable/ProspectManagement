using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitRootViewModel : BaseViewModel, IMvxViewModel<User>
    {
        private readonly IUserDefinedCodeService _userDefinedCodeService;
        private readonly IMvxNavigationService _navigationService;

        private ICommand _showInitialViewModelsCommand;

        private User _user;

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public ICommand ShowInitialViewModelsCommand
        {
            get
            {
                return _showInitialViewModelsCommand ?? (_showInitialViewModelsCommand = new MvxCommand(ShowInitialViewModels));
            }
        }

        private void ShowInitialViewModels()
        {
            _navigationService.Navigate<SplitMasterViewModel, User>(User);
        }

        public SplitRootViewModel(IUserDefinedCodeService userDefinedCodeService, IMvxNavigationService navigationService)
        {
            _userDefinedCodeService = userDefinedCodeService;
            _navigationService = navigationService;
        }

        public override async Task Initialize()
        {
            await _userDefinedCodeService.GetPrefixUserDefinedCodes();
            await _userDefinedCodeService.GetSuffixUserDefinedCodes();
            await _userDefinedCodeService.GetContactPreferenceUserDefinedCodes();
            await _userDefinedCodeService.GetExcludeReasonUserDefinedCodes();
            await _userDefinedCodeService.GetStateUserDefinedCodes();
            await _userDefinedCodeService.GetCountryUserDefinedCodes();
        }

        public void Prepare(User user)
        {
            User = user;
        }
    }
}
