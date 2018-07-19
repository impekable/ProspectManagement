using System;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Messages
{
    public class ProspectChangedMessage: MvxMessage
    {
		public Prospect UpdatedProspect
		{
			get;
			set;
		}

		public ProspectChangedMessage(object sender) : base(sender)
		{
		}
    }
}
