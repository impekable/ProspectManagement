using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitRootViewModel : BaseViewModel
    {
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

        public async void Init(User user)
        {
            User = user;
        }
    }
}
