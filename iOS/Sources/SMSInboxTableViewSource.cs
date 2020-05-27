using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using Nito.Mvvm;
using ProspectManagement.Core.Interfaces.InfiniteScroll;
using UIKit;

namespace ProspectManagement.iOS.Sources
{
    public class SMSInboxTableViewSource : MvxSimpleTableViewSource
	{
        private static readonly NSString cellIdentifier = new NSString("SMSInboxViewCell");
        private int _lastViewedPosition;

        public SMSInboxTableViewSource(UITableView tableView, NSString cellIdentifier) : base(tableView, new NSString("SMSInboxViewCell"))
        {
            DeselectAutomatically = false;
            tableView.RegisterNibForCellReuse(UINib.FromName("SMSInboxViewCell", NSBundle.MainBundle),
                                              cellIdentifier);
        }

        public void CreateBinding<TSource>(MvxViewController controller, Expression<Func<TSource, object>> sourceProperty)
        {
            controller.CreateBinding(this).To(sourceProperty).Apply();
            _lastViewedPosition = 0;
            LoadMoreItems();
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            int position = indexPath.Row;

            if ((position > _lastViewedPosition) && (position == (ItemsSource.Count() - 1)))
            {
                _lastViewedPosition = position;
                LoadMoreItems();
            }

            return base.GetOrCreateCellFor(tableView, indexPath, item);
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
                await source.LoadMoreItemsAsync();
            }
        }
    }
}
