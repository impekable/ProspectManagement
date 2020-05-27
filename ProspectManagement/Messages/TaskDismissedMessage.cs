using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class TaskDismissedMessage : MvxMessage
    {
        public Activity Activity
        {
            get;
            set;
        }

        public TaskDismissedMessage(object sender) : base(sender)
        {
        }
    }
}
