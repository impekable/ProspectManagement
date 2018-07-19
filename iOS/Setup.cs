using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.iOS.Services;
using ProspectManagement.Core;
using MvvmCross.Binding.Bindings.Target.Construction;
using ProspectManagement.iOS.CustomBindings;
using ProspectManagement.iOS.Views;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Views;

namespace ProspectManagement.iOS
{
    public class Setup : MvxIosSetup<App>
    { 

        protected override void InitializeLastChance()
        {
			base.InitializeLastChance();
            Mvx.RegisterSingleton<IDialogService>(() => new DialogService());
            Mvx.RegisterSingleton<IAuthenticator>(() => new Authenticator());
        }

		protected override IMvxIosViewsContainer CreateIosViewsContainer()
		{
			return new StoryBoardContainer();
		}

		protected override IMvxIocOptions CreateIocOptions()
		{
			return new MvxIocOptions
			{
                PropertyInjectorOptions = MvxPropertyInjectorOptions.MvxInject
			};
		}

		protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);
            registry.RegisterPropertyInfoBindingFactory(
                typeof(CustomAlertControllerActionsTargetBinding),
                typeof(CustomAlertController), "AlertController");
            registry.RegisterPropertyInfoBindingFactory(
                typeof(CustomAlertControllerSelectedCodeTargetBinding),
                typeof(CustomAlertController), "SelectedCode");
        }
    }
}
