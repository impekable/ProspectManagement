using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class CobuyerAddedMessage : MvxMessage
    {
        public Cobuyer AddedCobuyer
        {
            get;
            set;
        }

        public CobuyerAddedMessage(object sender) : base(sender)
        {
        }

    }
}