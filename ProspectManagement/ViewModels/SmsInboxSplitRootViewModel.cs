using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SmsInboxSplitRootViewModel : BaseViewModel, IMvxViewModel<User>
    {
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
            _navigationService.Navigate<SMSInboxViewModel, User>(User);
        }

        public SmsInboxSplitRootViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void Prepare(User user)
        {
            User = user;
        }
    }
}
