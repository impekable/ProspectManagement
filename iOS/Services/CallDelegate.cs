using System;
using Foundation;
using Twilio.Voice.iOS;

namespace ProspectManagement.iOS.Services
{
    internal class CallDelegate : TVOCallDelegate
    {

        private static CallDelegate _instance;

        public static CallDelegate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CallDelegate();
                }

                return _instance;
            }
        }

        public event EventHandler<TVOCall> CallDidConnectEvent;
        public event EventHandler<(TVOCall call, NSError error)> CallDidDisconnectWithErrorEvent;
        public event EventHandler<(TVOCall call, NSError error)> CallDidFailToConnectWithErrorEvent;
        public event EventHandler<TVOCall> CallDidStartRingingEvent;

        [Export("callDidConnect:")]
        public override void CallDidConnect(TVOCall call)
        {
            //LogHelper.Call(nameof(CallDelegate), nameof(CallDidConnect));
            CallDidConnectEvent?.Invoke(this, call);
        }

        [Export("call:didDisconnectWithError:")]
        public override void CallDidDisconnectWithError(TVOCall call, NSError error)
        {
            //LogHelper.Call(nameof(CallDelegate), nameof(CallDidDisconnectWithError));
            CallDidDisconnectWithErrorEvent?.Invoke(this, (call, error));
        }

        [Export("call:didFailToConnectWithError:")]
        public override void CallDidFailToConnectWithError(TVOCall call, NSError error)
        {
            //LogHelper.Call(nameof(CallDelegate), nameof(CallDidFailToConnectWithError));
            CallDidFailToConnectWithErrorEvent?.Invoke(this, (call, error));
        }

        [Export("callDidStartRinging:")]
        public override void CallDidStartRinging(TVOCall call)
        {
            //LogHelper.Call(nameof(CallDelegate), nameof(CallDidStartRinging));
            CallDidStartRingingEvent?.Invoke(this, call);
        }
    }
}

