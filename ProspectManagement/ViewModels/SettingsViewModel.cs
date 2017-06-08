using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Extensions;

namespace ProspectManagement.Core.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
        private readonly IProspectService _prospectService;
		private readonly IDefaultCommunityService _defaultCommunityService;
		private readonly ICommunityService _communityService;
		private ObservableCollection<Community> _communities;
		private Community _defaultCommunity;
		private readonly IDialogService _dialogService;
		private User _activeUser;
		private bool _isUserLoggedIn;
		private bool _isActivityIndicatorActive;

		public bool IsActivityIndicatorActive
		{
			get { return _isActivityIndicatorActive; }
			set
			{
				_isActivityIndicatorActive = value;
				RaisePropertyChanged(() => IsActivityIndicatorActive);
			}
		}

		public bool IsUserLoggedIn
		{
			get { return _isUserLoggedIn; }
			set
			{
				_isUserLoggedIn = value;
				RaisePropertyChanged(() => IsUserLoggedIn);
			}
		}

		public User ActiveUser
		{
			get { return _activeUser; }
			set
			{
				_activeUser = value;
				IsUserLoggedIn = (_activeUser != null && !string.IsNullOrEmpty(_activeUser.UserId));
				RaisePropertyChanged(() => ActiveUser);
			}
		}

		public Community DefaultCommunity
		{
			get { return _defaultCommunity; }
			set
			{
				_defaultCommunity = value;
				RaisePropertyChanged(() => DefaultCommunity);
			}
		}

		public ObservableCollection<Community> Communities
		{
			get { return _communities; }
			set
			{
				_communities = value;
				RaisePropertyChanged(() => Communities);
			}
		}

		public MvxCommand SwitchCommunityCommand
		{
			get
			{
				return new MvxCommand(async () =>
				{
                    ShowViewModel<SplitRootViewModel>();
					//_defaultCommunityService.SaveDefaultCommunity(_defaultCommunity);
					//await _dialogService.ShowAlertAsync("Default Community Set", "Confirmation", "Close");
										//Close(this);
				});
			}
		}

		public SettingsViewModel(IProspectService prospectService, IDefaultCommunityService defaultCommunityService, ICommunityService communityService, IDialogService dialogService)
		{
			_defaultCommunityService = defaultCommunityService;
			_dialogService = dialogService;
			_communityService = communityService;
            _prospectService = prospectService; 
		}

		public void Init(User activeUser)
		{
			ActiveUser = activeUser;
		}

		public override async void Start()
		{
			base.Start();
			await ReloadDataAsync();
		}

		protected override async Task InitializeAsync()
		{
			DefaultCommunity = await _defaultCommunityService.GetDefaultCommunity();
			if (IsUserLoggedIn)
				Communities = (await _communityService.GetCommunitiesBySalesperson(ActiveUser.AddressNumber)).ToObservableCollection();
		}
	}
}
