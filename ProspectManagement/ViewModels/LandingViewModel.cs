using System;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class LandingViewModel : BaseViewModel, IMvxViewModel<User>
    {
        protected IMvxMessenger Messenger;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticator _authService;
        private readonly IUserService _userService;
        private readonly IMvxNavigationService _navigationService;
        private User _user;

        private ICommand _viewProspectsCommand;
        private ICommand _viewDailyToDoCommand;
        private ICommand _goHomeCommand;
        private ICommand _logoutCommand;
        private ICommand _viewSMSInboxCommand;

        public DateTime LoadTime { get; set; }

        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new MvxCommand(async () =>
                {
                    var result = await _dialogService.ShowAlertAsync("Confirm", "Logout?", "Yes", "No");
                    if (result == 0)
                    {
                        _authService.Logout();
                        await _navigationService.Navigate<RootViewModel, bool>(false);
                    }
                }));
            }
        }

        public ICommand ViewProspectsCommand
        {
            get
            {
                return _viewProspectsCommand ?? (_viewProspectsCommand = new MvxCommand(async () => { LoadTime = DateTime.Now; await _navigationService.Navigate<SplitRootViewModel, User>(User); }));
            }
        }

        public ICommand ViewDailyToDoCommand
        {
            get
            {
                return _viewDailyToDoCommand ?? (_viewDailyToDoCommand = new MvxCommand(async () => { LoadTime = DateTime.Now; await _navigationService.Navigate<DailyToDoViewModel, User>(User); }));
            }
        }

        public ICommand ViewSMSInboxCommand
        {
            get
            {
                return _viewSMSInboxCommand ?? (_viewSMSInboxCommand = new MvxCommand(async () => { LoadTime = DateTime.Now; await _navigationService.Navigate<SmsInboxSplitRootViewModel, User>(User); }));
            }
        }

        public ICommand GoHomeCommand
        {
            get
            {
                return _goHomeCommand ?? (_goHomeCommand = new MvxCommand(async () => { await _navigationService.Navigate<LandingViewModel, User>(User); }));
            }
        }

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public LandingViewModel(IMvxNavigationService navigationService, IDialogService dialogService, IAuthenticator authService, IMvxMessenger messenger, IUserService userService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authService = authService;
            _userService = userService;
            Messenger = messenger;

            Messenger.Subscribe<SMSReceivedMessage>(message => Process(message.SMSActivityReceived), MvxReference.Strong);
            Messenger.Subscribe<ExtendReloadTime>(message => Extend(message.ExtendMinutes), MvxReference.Strong);
        }

        private void Extend(int extendMinutes)
        {
            LoadTime = DateTime.Now.AddMinutes(extendMinutes);
        }

        private async void Process(SmsActivity smsActivityReceived)
        {
            User = await _userService.GetLoggedInUser();
        }

        public void Prepare(User parameter)
        {
            User = parameter;
        }
    }
}
