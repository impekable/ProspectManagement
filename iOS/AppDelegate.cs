using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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

            AppCenter.Start("85a96bc0-a6ca-410e-a703-f4f1188ec592",
                   typeof(Analytics), typeof(Crashes));
            
			return true;
        }
    }
}
