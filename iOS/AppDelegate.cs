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
using MvvmCross.Navigation;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.Core.Models;
using AudioToolbox;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Messages;

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

            if (options != null && options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
            {
                var notificationData = options[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
                // process notification data
                navigateOnPush(notificationData);
            }

            return result;
        }

        async void navigateOnPush(NSDictionary options)
        {
            Prospect prospect = null;
            if (options.ContainsKey(new NSString("prospectId")))
            {
                var prospectId = (options[new NSString("prospectId")] as NSNumber).Int32Value;
                if (prospectId > 0)
                {
                    var prospectService = Mvx.IoCProvider.Resolve<IProspectService>();
                    prospect = await prospectService.GetProspectAsync(prospectId);
                }
            }
            if (prospect != null)
            {
                var navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                await navigationService.Navigate<ProspectSMSViewModel, Prospect>(prospect);
            }
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

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, false);
        }

        async void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != options && options.ContainsKey(new NSString("aps")))
            {
                //Get the aps dictionary
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
                string message = string.Empty;
                string title = string.Empty;

                //Extract the alert text
                if (aps.ContainsKey(new NSString("alert")))
                {
                    NSDictionary alert = aps.ObjectForKey(new NSString("alert")) as NSDictionary;
                    if (alert.ContainsKey(new NSString("title")))
                        title = (alert[new NSString("title")] as NSString).ToString();
                    if (alert.ContainsKey(new NSString("body")))
                        message = (alert[new NSString("body")] as NSString).ToString();
                }

                //If this came from the ReceivedRemoteNotification while the app was running,
                // we of course need to manually process things like the sound, badge, and alert.
                if (!fromFinishedLaunching)
                {
                    var messenger = Mvx.IoCProvider.Resolve<IMvxMessenger>();
                    messenger.Publish(new SMSReceivedMessage(this) { SMSActivityReceived = new SmsActivity {  MessageBody = message, Direction = "inbound"} });

                    //Manually show an alert
                    if (!string.IsNullOrEmpty(message))
                    {
                        var dialogService = Mvx.IoCProvider.Resolve<IDialogService>();
                        var result = await dialogService.ShowAlertAsync(title, message, "View Message", "Cancel");
                        if (result == 0)
                        {
                            navigateOnPush(options);
                        }
                    }
                }
            }
        }
    }
}
