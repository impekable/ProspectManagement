﻿using System;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core
{
    public class CobuyerChangedMessage : MvxMessage
    {
        public Cobuyer UpdatedCobuyer
        {
			get;
			set;
        }

		public CobuyerChangedMessage(object sender) : base(sender)
        {
		}
		
    }
}



