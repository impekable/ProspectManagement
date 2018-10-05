using System;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using ProspectManagement.Core.Models;
using UIKit;

namespace ProspectManagement.iOS.Sources
{
	public class ActivityTableViewSource: MvxSimpleTableViewSource
    {
        private static readonly NSString cellIdentifier = new NSString("ActivitiesViewCell");

		public ActivityTableViewSource(UITableView tableView, NSString cellIdentifier) : base(tableView, new NSString("ActivitiesViewCell"))
        {
            DeselectAutomatically = false;
			tableView.RegisterNibForCellReuse(UINib.FromName("ActivitiesViewCell", NSBundle.MainBundle),
                                              cellIdentifier);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
			var cell = base.GetOrCreateCellFor(tableView, indexPath, item);
			var activity = (Activity)item;
			if (activity.AdditionalNotesExist)
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			else
				cell.Accessory = UITableViewCellAccessory.None;
            return cell;
        }
    }
}

