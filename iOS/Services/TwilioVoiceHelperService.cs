using System;
using Foundation;
using Twilio.Voice.iOS;

namespace ProspectManagement.iOS.Services
{


    public class TwilioVoiceHelperService : IDisposable
    {
        private CallDelegate _callDelegate;
        private NotificationDelegate _notificationDelegate;
        private string _registeredAccessToken;
        private string _registeredDeviceToken;

        public TVOCallInvite CallInvite { get; private set; }
        public TVOCall Call { get; private set; }

        public event EventHandler Registered;
        public event EventHandler<NSError> RegisteredWithError;
        public event EventHandler CallInviteReceived;
        public event EventHandler CallDidConnect;
        public event EventHandler<NSError> CallDidDisconnectWithError;
        public event EventHandler<NSError> CallDidFailToConnectWithError;
        public event EventHandler<(TVOCancelledCallInvite, NSError)> CancelledCallInviteReceived;

        public TwilioVoiceHelperService()
        {
            _callDelegate = CallDelegate.Instance;
            _notificationDelegate = NotificationDelegate.Instance;
            _callDelegate.CallDidConnectEvent -= CallDelegateOnCallDidConnectEvent;
            _callDelegate.CallDidDisconnectWithErrorEvent -= CallDelegateOnCallDidDisconnectWithError;
            _callDelegate.CallDidFailToConnectWithErrorEvent -= CallDelegateOnCallDidFailToConnectWithErrorEvent;
            _notificationDelegate.CallInviteReceivedEvent -= NotificationDelegateOnCallInviteReceivedEvent;
            _notificationDelegate.CancelledCallInviteReceivedEvent -= NotificationDelegateOnCancelledCallInviteReceivedEvent;

            _callDelegate.CallDidConnectEvent += CallDelegateOnCallDidConnectEvent;
            _callDelegate.CallDidDisconnectWithErrorEvent += CallDelegateOnCallDidDisconnectWithError;
            _callDelegate.CallDidFailToConnectWithErrorEvent += CallDelegateOnCallDidFailToConnectWithErrorEvent;
            _notificationDelegate.CallInviteReceivedEvent += NotificationDelegateOnCallInviteReceivedEvent;
            _notificationDelegate.CancelledCallInviteReceivedEvent += NotificationDelegateOnCancelledCallInviteReceivedEvent;
        }

        public void Register(string accessToken, string deviceToken)
        {
            if (accessToken == null || deviceToken == null) return;
            TwilioVoice.RegisterWithAccessToken(accessToken, deviceToken,
                                                error =>
                                                {
                                                    if (error == null)
                                                    {
                                                        _registeredAccessToken = accessToken;
                                                        _registeredDeviceToken = deviceToken;
                                                        Registered?.Invoke(this, EventArgs.Empty);
                                                    }
                                                    else
                                                    {
                                                        RegisteredWithError?.Invoke(this, error);
                                                    }
                                                });
        }

        public void Unregister()
        {
            if (_registeredAccessToken == null || _registeredDeviceToken == null) return;
            TwilioVoice.UnregisterWithAccessToken(_registeredAccessToken, _registeredDeviceToken,
                                                error =>
                                                {
                                                    if (error == null)
                                                    {
                                                        _registeredAccessToken = null;
                                                        _registeredDeviceToken = null;
                                                    }
                                                    else
                                                    {
                                                        
                                                    }
                                                });
        }

        public void MakeCall(string accessToken, NSDictionary<NSString, NSString> parameters)
        {
            TwilioVoice.AudioDevice = new TVOAudioDevice();
            
            if (accessToken == null || Call != null) return;
            var connectionOptions = TVOConnectOptions.OptionsWithAccessToken(accessToken,
                (arg0) =>
                {
                    arg0.Params =  parameters;
                });
            Call = TwilioVoice.ConnectWithOptions(connectionOptions, _callDelegate);
        }

        public void RejectCallInvite()
        {
            CallInvite?.Reject();
            CallInvite = null;
        }

        public void IgnoreCallInvite()
        {
            CallInvite = null;
        }

        public void AcceptCallInvite()
        {
            Call = CallInvite?.AcceptWithDelegate(_callDelegate);
            CallInvite = null;
        }

        public void Dispose()
        {
            if (CallInvite != null)
            {
                CallInvite.Reject();
                CallInvite.Dispose();
                CallInvite = null;
            }

            if (Call != null)
            {
                Call.Disconnect();
                Call.Dispose();
                Call = null;
            }

            if (_callDelegate != null)
            {
                _callDelegate.CallDidConnectEvent -= CallDelegateOnCallDidConnectEvent;
                _callDelegate.CallDidDisconnectWithErrorEvent -= CallDelegateOnCallDidDisconnectWithError;
                _callDelegate.CallDidFailToConnectWithErrorEvent -= CallDelegateOnCallDidFailToConnectWithErrorEvent;
                _callDelegate = null;
            }

            if (_notificationDelegate != null)
            {
                _notificationDelegate.CallInviteReceivedEvent -= NotificationDelegateOnCallInviteReceivedEvent;
                _notificationDelegate.CancelledCallInviteReceivedEvent += NotificationDelegateOnCancelledCallInviteReceivedEvent;
                _notificationDelegate = null;
            }
        }

        private void CallDelegateOnCallDidConnectEvent(object sender, TVOCall e)
        {
            Call = e;
            CallDidConnect?.Invoke(this, EventArgs.Empty);
        }

        private void CallDelegateOnCallDidDisconnectWithError(object sender, (TVOCall call, NSError error) e)
        {
            Call = null;
            CallDidDisconnectWithError?.Invoke(this, e.error);
        }

        private void CallDelegateOnCallDidFailToConnectWithErrorEvent(object sender, (TVOCall call, NSError error) e)
        {
            Call = null;
            CallDidFailToConnectWithError?.Invoke(this, e.error);
        }

        private void NotificationDelegateOnCallInviteReceivedEvent(object sender, TVOCallInvite e)
        {
            if (CallInvite != null)
            {
                //LogHelper.Info($"Already a pending call invite. Ignoring incoming call invite from {e.From}");
                return;
            }
            if (Call != null && Call.State == TVOCallState.Connected)
            {
                //LogHelper.Info($"Already an active call. Ignoring incoming call invite from {e.From}");
                return;
            }

            CallInvite = e;
            CallInviteReceived?.Invoke(this, EventArgs.Empty);
        }

        private void NotificationDelegateOnCancelledCallInviteReceivedEvent(object sender, (TVOCancelledCallInvite e, NSError error) invite)
        {
            // We've already accepted the invite.
            if (Call != null) return;
            CancelledCallInviteReceived?.Invoke(this, (invite.e, invite.error));
        }

    }
}
