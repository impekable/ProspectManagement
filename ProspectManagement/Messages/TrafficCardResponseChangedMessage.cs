﻿using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class TrafficCardResponseChangedMessage : MvxMessage
    {
        public TrafficCardResponse ChangedResponse
        {
            get;
            set;
        }

        public TrafficCardResponseChangedMessage(object sender) : base(sender)
        {
        }
    }
}
