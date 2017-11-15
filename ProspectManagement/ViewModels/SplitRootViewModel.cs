using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitRootViewModel : BaseViewModel
    {
        private readonly IUserDefinedCodeService _userDefinedCodeService;

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
            ShowViewModel<SplitMasterViewModel>(User);
        }

        public SplitRootViewModel( IUserDefinedCodeService userDefinedCodeService)
        {
            _userDefinedCodeService = userDefinedCodeService;
        }

        public async void Init(User user)
        {
            User = user;

            await _userDefinedCodeService.GetPrefixUserDefinedCodes();
            await _userDefinedCodeService.GetSuffixUserDefinedCodes();
            await _userDefinedCodeService.GetContactPreferenceUserDefinedCodes();
            await _userDefinedCodeService.GetExcludeReasonUserDefinedCodes();
            await _userDefinedCodeService.GetStateUserDefinedCodes();
            await _userDefinedCodeService.GetCountryUserDefinedCodes();

        }
    }
}
