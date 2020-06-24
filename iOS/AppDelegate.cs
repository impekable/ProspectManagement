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
using WindowsAzure.Messaging;
using UserNotifications;
using ProspectManagement.Core.Interfaces.Services;

namespace ProspectManagement.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate<Setup, App>
    {
        private SBNotificationHub Hub { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var result = base.FinishedLaunching(app, options);

            AppCenter.Start(PrivateKeys.AppCenterAnalyticsAppSecret, typeof(Analytics), typeof(Crashes));

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                                                                        (granted, error) =>
                                                                        {
                                                                            if (granted)
                                                                                InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                                                                        });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }

            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

            return result;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Hub = new SBNotificationHub(AzurePushConstants.ListenConnectionString, AzurePushConstants.NotificationHubName);

            Hub.UnregisterAll(deviceToken, async (error) =>
            {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }

                var userService = Mvx.IoCProvider.Resolve<IUserService>();
                var user = await userService.GetLoggedInUser();
                if (user != null)
                {
                    NSSet tags = new NSSet(new string[] { "username:" + user.UserId }); // create tags if you want

                    Hub.RegisterNative(deviceToken, tags, (errorCallback) =>
                    {
                        if (errorCallback != null)
                            System.Diagnostics.Debug.WriteLine("RegisterNativeAsync error: " + errorCallback.ToString());
                    });
                }
            });
        }
    }
}
