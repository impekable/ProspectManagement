using System;
using System.Windows.Input;
using Foundation;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Binding.Views;
using ProspectManagement.iOS.Views;
using UIKit;

namespace ProspectManagement.iOS.Sources
{
	public class ProspectTableViewSource : MvxStandardTableViewSource
	{
		private static readonly NSString cellIdentifier = new NSString("ProspectViewCell");
		public ICommand FetchCommand { get; set; }

		public ProspectTableViewSource(UITableView tableView) : base(tableView, new NSString("ProspectViewCell"))
		{
			DeselectAutomatically = true;
			tableView.RegisterNibForCellReuse(UINib.FromName("ProspectViewCell", NSBundle.MainBundle),
			                                  cellIdentifier);
		}

		protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
		{
			var cell = (UITableViewCell)TableView.DequeueReusableCell(cellIdentifier, indexPath);
			if (indexPath.Item == ItemsSource.Count() - 10)
			{
				FetchCommand?.Execute(null);
			}

			return cell;
		}
	}
}
