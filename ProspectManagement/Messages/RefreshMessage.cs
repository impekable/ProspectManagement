using System;
using MvvmCross.Plugin.Messenger;

namespace ProspectManagement.Core.Messages
{
    public class RefreshMessage : MvxMessage
    {
        public RefreshMessage(object sender) : base(sender)
        {
        }
    }
}

