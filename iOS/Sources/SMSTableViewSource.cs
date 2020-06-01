using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using Nito.Mvvm;
using ProspectManagement.Core.Interfaces.InfiniteScroll;
using ProspectManagement.Core.Models;
using ProspectManagement.iOS.Views;
using UIKit;

namespace ProspectManagement.iOS.Sources
{
    public class SMSTableViewSource : MvxSimpleTableViewSource
    {
        private static readonly NSString outgoingCellIdentifier = new NSString("OutgoingCell");
        private static readonly NSString incomingCellIdentifier = new NSString("IncomingCell");

        private bool initialScroll;

        public CGPoint OffsetPriorToLoad { get; set; }

        public SMSTableViewSource(UITableView tableView, NSString cellIdentifier) : base(tableView, new NSString("BubbleCell"))
        {
            DeselectAutomatically = false;
            initialScroll = true;

            tableView.RegisterNibForCellReuse(UINib.FromName("OutgoingCell", NSBundle.MainBundle),
                                              outgoingCellIdentifier);
            tableView.RegisterNibForCellReuse(UINib.FromName("IncomingCell", NSBundle.MainBundle),
                                              incomingCellIdentifier);
        }

        public void CreateBinding<TSource>(MvxViewController controller, Expression<Func<TSource, object>> sourceProperty)
        {
            controller.CreateBinding(this).To(sourceProperty).Apply();
            LoadMoreItems();
        }

        public override void WillEndDragging(UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
        {
            if ((scrollView.ContentOffset.Y + scrollView.SafeAreaInsets.Top) < 0)
            {
                Console.WriteLine("scrolling up past top: " + scrollView.ContentOffset.Y + scrollView.SafeAreaInsets.Top);
                LoadMoreItems();
            }
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var smsMessage = (SmsActivity)item;

            var cell = (BubbleCell)tableView.DequeueReusableCell(GetReuseId(smsMessage));
            cell.Message = smsMessage;

            return cell;
        }

        NSString GetReuseId(SmsActivity msg)
        {
            return msg.Direction.Equals("outbound") ? outgoingCellIdentifier : incomingCellIdentifier;
        }

        private void LoadMoreItems()
        {
            NotifyTask taskCompletion = NotifyTask.Create(LoadMoreItemsAsync());
        }

        public async Task LoadMoreItemsAsync()
        {
            ICoreSupportIncrementalLoading source = ItemsSource as ICoreSupportIncrementalLoading;

            if (source != null)
            {
                await source.LoadMoreItemsAsync(prependItem: true);
            }
        }
    }
}
