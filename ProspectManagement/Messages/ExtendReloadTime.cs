using System;
using MvvmCross.Plugin.Messenger;

namespace ProspectManagement.Core.Messages
{
    public class ExtendReloadTime : MvxMessage
    {
        public int ExtendMinutes { get; set; }

        public ExtendReloadTime(object sender) : base(sender)
        {
        }
    }
}
