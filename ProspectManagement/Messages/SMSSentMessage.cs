using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class SMSSentMessage : MvxMessage
    {
        public SmsActivity SMSActivitySent
        {
            get;
            set;
        }

        public SMSSentMessage(object sender) : base(sender)
        {
        }
    }
}
