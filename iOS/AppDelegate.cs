using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.ViewModels;
using ProspectManagement.Core;
using ProspectManagement.Core.Constants;
using UIKit;

namespace ProspectManagement.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate<Setup, App>
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			var result = base.FinishedLaunching(app, options);

			AppCenter.Start(PrivateKeys.AppCenterAnalyticsAppSecret, typeof(Analytics), typeof(Crashes));

			return result;
        }
    }
}
