using System;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core
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