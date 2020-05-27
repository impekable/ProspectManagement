using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{

    public class TaskCompletedMessage : MvxMessage
    {
        public Activity Activity
        {
            get;
            set;
        }

        public TaskCompletedMessage(object sender) : base(sender)
        {
        }
    }

}
