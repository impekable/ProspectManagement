using System;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using ProspectManagement.Core.ViewModels;

namespace ProspectManagement.Core
{
	public class AppStart: MvxAppStart
    {
		private readonly IMvxNavigationService _mvxNavigationService;

		public AppStart(IMvxApplication app, IMvxNavigationService mvxNavigationService): base(app, mvxNavigationService)
        {
			_mvxNavigationService = mvxNavigationService;

        }

		protected override void NavigateToFirstViewModel(object hint = null)
		{
			_mvxNavigationService.Navigate<RootViewModel>();
		}
	}
}
