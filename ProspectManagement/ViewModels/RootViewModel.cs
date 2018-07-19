using System;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
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

		public async override void ViewAppeared()
		{
			base.ViewAppeared();
			if (User == null || User.AddressNumber == 0)
            {
              User = await _userService.GetLoggedInUser();
            }
            await _navigationService.Navigate<SplitRootViewModel, User>(User);
		}
        
    }
}
