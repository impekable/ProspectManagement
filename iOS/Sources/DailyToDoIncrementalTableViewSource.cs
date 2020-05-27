using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Extensions;
using MvvmCross.Commands;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using Nito.Mvvm;
using ProspectManagement.Core.Interfaces.InfiniteScroll;
using ProspectManagement.Core.ViewModels;
using UIKit;

namespace ProspectManagement.iOS.Sources
{
    public class DailyToDoIncrementalTableViewSource : MvxSimpleTableViewSource
    {
        private static readonly NSString cellIdentifier = new NSString("FollowUpViewCell");
        private int _lastViewedPosition;

        public DailyToDoIncrementalTableViewSource(UITableView tableView, NSString cellIdentifier) : base(tableView, new NSString("FollowUpViewCell"))
        {
            DeselectAutomatically = false;
            tableView.RegisterNibForCellReuse(UINib.FromName("FollowUpViewCell", NSBundle.MainBundle),
                                              cellIdentifier);
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var rowActions = new List<UITableViewRowAction>();
            var item = GetItemAt(indexPath) as DailyToDoTaskViewModel;
            if (item.DismissCommand.CanExecute(indexPath.Row))
            {
                rowActions.Add(UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Dismiss", (action, path) => { item.DismissCommand.Execute(item); }));
            }

            return rowActions.ToArray();
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
