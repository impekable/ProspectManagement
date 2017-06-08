using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitRootViewModel : BaseViewModel
    {
        private ICommand _showInitialViewModelsCommand;
        public ICommand ShowInitialViewModelsCommand
        {
            get
            {
                return _showInitialViewModelsCommand ?? (_showInitialViewModelsCommand = new MvxCommand(ShowInitialViewModels));
            }
        }

        private void ShowInitialViewModels()
        {
            ShowViewModel<SplitMasterViewModel>();
        }
    }
}
