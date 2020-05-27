using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using ProspectManagement.Core.Models;
using ProspectManagement.iOS.Views;
using UIKit;

namespace ProspectManagement.iOS.Sources
{
    public class SMSTableViewSource : MvxSimpleTableViewSource
    {
        private static readonly NSString outgoingCellIdentifier = new NSString("OutgoingCell");
        private static readonly NSString incomingCellIdentifier = new NSString("IncomingCell");
        
        public SMSTableViewSource(UITableView tableView, NSString cellIdentifier) : base(tableView, new NSString("BubbleCell"))
        {
            DeselectAutomatically = false;

            tableView.RegisterNibForCellReuse(UINib.FromName("OutgoingCell", NSBundle.MainBundle),
                                              outgoingCellIdentifier);
            tableView.RegisterNibForCellReuse(UINib.FromName("IncomingCell", NSBundle.MainBundle),
                                              incomingCellIdentifier);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var smsMessage = (SmsActivity)item;

            var cell = (BubbleCell)tableView.DequeueReusableCell(GetReuseId(smsMessage));    
            cell.Message = smsMessage;
            //cell.Transform = CGAffineTransform.MakeRotation(-3.14159f);

            return cell;
        }

        NSString GetReuseId(SmsActivity msg)
        {
            return msg.Direction.Equals("outbound") ? outgoingCellIdentifier : incomingCellIdentifier;
        }
    }
}
