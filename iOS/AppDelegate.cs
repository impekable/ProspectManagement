using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using UIKit;

namespace ProspectManagement.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			Window = new UIWindow(UIScreen.MainScreen.Bounds);

			//var presenter = new MvxModalSupportIosViewPresenter(this, Window);
			var setup = new Setup(this, Window);
			setup.Initialize();

			var startup = Mvx.Resolve<IMvxAppStart>();
			startup.Start();

			Window.MakeKeyAndVisible();
			return true;
        }
    }
}
