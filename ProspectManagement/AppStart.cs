using System;
using System.Threading.Tasks;
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

		protected override Task NavigateToFirstViewModel(object hint = null)
		{
			return _mvxNavigationService.Navigate<RootViewModel>();
		}
	}
}
