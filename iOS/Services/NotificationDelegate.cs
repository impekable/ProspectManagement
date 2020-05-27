using System;
using Foundation;
using Twilio.Voice.iOS;

namespace ProspectManagement.iOS.Services
{
    public class NotificationDelegate : TVONotificationDelegate
    {

        private static NotificationDelegate _instance;

        public static NotificationDelegate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationDelegate();
                }

                return _instance;
            }
        }

        public event EventHandler<TVOCallInvite> CallInviteReceivedEvent;
        public event EventHandler<(TVOCancelledCallInvite, NSError)> CancelledCallInviteReceivedEvent;

        [Export("callInviteReceived:")]
        public override void CallInviteReceived(TVOCallInvite callInvite)
        {
            //LogHelper.Call(nameof(NotificationDelegate), nameof(CallInviteReceived));
            CallInviteReceivedEvent?.Invoke(this, callInvite);
        }

        [Export("cancelledCallInviteReceived:error:")]
        public override void CancelledCallInviteReceived(TVOCancelledCallInvite cancelledCallInvite, NSError error)
        {
            //LogHelper.Call(nameof(NotificationDelegate), nameof(CancelledCallInviteReceived));
            CancelledCallInviteReceivedEvent?.Invoke(this, (cancelledCallInvite, error));
        }

    }
}
