using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.iOS.Services;
using MvvmCross.iOS.Views;
using ProspectManagement.Core;
using ProspectManagement.Core.Services;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Repositories;
using MvvmCross.Binding.Bindings.Target.Construction;
using ProspectManagement.iOS.CustomBindings;
using ProspectManagement.iOS.Views;

namespace ProspectManagement.iOS
{
    public class Setup : MvxIosSetup
    {
        private MvxApplicationDelegate _applicationDelegate;
        UIWindow _window;

        public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window) : base(applicationDelegate, window)
        {
            _applicationDelegate = applicationDelegate;
            _window = window;
        }

        public Setup(MvxApplicationDelegate applicationDelegate, IMvxIosViewPresenter presenter) : base(applicationDelegate, presenter)
        {

        }
        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();
            Mvx.RegisterSingleton<IDialogService>(() => new DialogService());
            Mvx.RegisterSingleton<IAuthenticator>(() => new Authenticator());
        }

        protected override IMvxIosViewsContainer CreateIosViewsContainer()
        {
            return new StoryBoardContainer();
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
