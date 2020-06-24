using System;
using System.Threading.Tasks;
using Foundation;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.ViewModels;
using UserNotifications;

namespace ProspectManagement.iOS
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {

        public async override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            // Take action based on identifier
            if (response.IsDefaultAction)
            {
                // Handle default action...
                var info = response.Notification.Request.Content.UserInfo;
                if (info.ContainsKey(new NSString("prospectId")))
                {
                    var prospectId = (info[new NSString("prospectId")] as NSNumber).Int32Value;
                    var prospectService = Mvx.IoCProvider.Resolve<IProspectService>();
                    var prospect = await prospectService.GetProspectAsync(prospectId);
                    var navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                    await navigationService.Navigate<ProspectSMSViewModel, Prospect>(prospect);
                }
            }
            else if (response.IsDismissAction)
            {
                // Handle dismiss action
            }

            // Inform caller it has been handled
            completionHandler();
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var info = notification.Request.Content.UserInfo;
            if (info.ContainsKey(new NSString("prospectId")))
            {
                var prospectId = (info[new NSString("prospectId")] as NSNumber).Int32Value;
                var messenger = Mvx.IoCProvider.Resolve<IMvxMessenger>();
                messenger.Publish(new SMSReceivedMessage(this) { SMSActivityReceived = new SmsActivity { ProspectAddressBook = prospectId, UpdatedDate = DateTime.UtcNow, MessageBody = notification.Request.Content.Body, Direction = "inbound" } });

            }
            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}
