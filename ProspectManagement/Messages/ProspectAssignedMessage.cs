using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class ProspectAssignedMessage: MvxMessage
    {
		public Prospect AssignedProspect
		{
			get;
			set;
		}

		public ProspectAssignedMessage(object sender) : base(sender)
        {
		}
    }
}
