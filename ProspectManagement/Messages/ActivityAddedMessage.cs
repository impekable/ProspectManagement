using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class ActivityAddedMessage : MvxMessage
    {
        public Activity AddedActivity
        {
            get;
            set;
        }

        public ActivityAddedMessage(object sender) : base(sender)
        {
        }

    }
}
