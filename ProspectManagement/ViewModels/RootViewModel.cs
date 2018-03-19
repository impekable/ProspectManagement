using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class RootViewModel: BaseViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserService _userService;
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

        public RootViewModel(IUserService userService, IMvxNavigationService navigationService)
		{
			_userService = userService;
            _navigationService = navigationService;
		}

		public override async void Start()
		{
			base.Start();
			await ReloadDataAsync();
		}

		protected override async Task InitializeAsync()
		{
			while (User == null || User.AddressNumber == 0)
			{
				User = await _userService.GetLoggedInUser();
			}
            _navigationService.Navigate<SplitRootViewModel, User>(User);
		}
    }
}
