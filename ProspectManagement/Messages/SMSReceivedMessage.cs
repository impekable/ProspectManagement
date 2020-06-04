using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class SMSReceivedMessage : MvxMessage
    {
        public SmsActivity SMSActivityReceived
        {
            get;
            set;
        }

        public SMSReceivedMessage(object sender) : base(sender)
        {
        }
    }
}
