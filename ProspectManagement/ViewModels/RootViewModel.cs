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
    public class RootViewModel : BaseViewModel, IMvxViewModel<bool>
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserService _userService;
        private readonly IAuthenticator _authService;
        private User _user;
        private bool _attemptingLogin;
        private ICommand _loginCommand;
        private bool _promptForLoginInfo;

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public bool AttemptingLogin
        {
            get { return _attemptingLogin; }
            set
            {
                _attemptingLogin = value;
                RaisePropertyChanged(() => AttemptingLogin);
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new MvxCommand(async () =>
                {
                    AttemptingLogin = true;
                    User = await _userService.GetLoggedInUser();
                    if (User != null && User.AddressNumber != 0)
                    {
                        if (User.UsingTelephony)
                            await _navigationService.Navigate<LandingViewModel, User>(User);
                        else
                            await _navigationService.Navigate<SplitRootViewModel, User>(User);
                    }
                    AttemptingLogin = false;
                }));
            }
        }

        public RootViewModel(IAuthenticator authService, IUserService userService, IMvxNavigationService navigationService)
        {
            _authService = authService;
            _userService = userService;
            _navigationService = navigationService;
            _promptForLoginInfo = true;
        }

        public override async void Start()
        {
            base.Start();
            await ReloadDataAsync();
        }

        public void Prepare(bool parameter = true)
        {
            _promptForLoginInfo = parameter;
        }

        public async override void ViewAppeared()
        {
            base.ViewAppeared();
            if (_promptForLoginInfo && !AttemptingLogin)
            {
                AttemptingLogin = true;
                if (User == null || User.AddressNumber == 0)
                {
                    User = await _userService.GetLoggedInUser();
                }

                if (User != null && User.AddressNumber > 0)
                {
                    if (User.UsingTelephony)
                        await _navigationService.Navigate<LandingViewModel, User>(User);
                    else
                        await _navigationService.Navigate<SplitRootViewModel, User>(User);
                }
                else
                {
                    AttemptingLogin = false;
                }
            }
        }

    }
}
