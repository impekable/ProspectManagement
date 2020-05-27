using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Sources;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    public partial class DailyToDoView : MvxViewController<DailyToDoViewModel>
    {
        private DailyToDoIncrementalTableViewSource source;

        public DailyToDoView (IntPtr handle) : base (handle)
        {
        }

        private void setTableViewSource(MvxFluentBindingDescriptionSet<DailyToDoView, DailyToDoViewModel> set)
        {
            ViewModel.Page = 0;
            ViewModel.Activities = null;

            source = new DailyToDoIncrementalTableViewSource(MasterTableView, FollowUpViewCell.Key); // new MvxSimpleTableViewSource(MasterTableView, ProspectViewCell.Key, ProspectViewCell.Key, null);
            source.CreateBinding<DailyToDoViewModel>(this, vm => vm.Activities);
            MasterTableView.Source = source;
            MasterTableView.RowHeight = UITableView.AutomaticDimension;
            MasterTableView.EstimatedRowHeight = 40;
            MasterTableView.ReloadData();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.Activities);
            set.Apply();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            System.Diagnostics.Debug.WriteLine("Loading Master View: " + this.Handle);

            var set = this.CreateBindingSet<DailyToDoView, DailyToDoViewModel>();
            set.Bind(FilterSegmentControl).To(vm => vm.SelectedSegment);
            setTableViewSource(set);

            var refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += (sender, e) =>
            {
                setTableViewSource(set);
                ViewModel.RefreshCommand.Execute(null);
            };
            MasterTableView.RefreshControl = refreshControl;

            ViewModel.LoadingDataFromBackendStarted += (sender, e) =>
            {
                InvokeOnMainThread(() => refreshControl.BeginRefreshing());
            };

            ViewModel.LoadingDataFromBackendCompleted += (sender, e) =>
            {
                InvokeOnMainThread(() => refreshControl.EndRefreshing());
            };

            FilterSegmentControl.ValueChanged += (sender, e) =>
            {
                setTableViewSource(set);
            };

            var h = new UIBarButtonItem("Home", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                ViewModel.HomeCommand.Execute(null);
            });

            h.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = ProspectManagementColors.DarkColor
            }, UIControlState.Normal);

            NavigationItem.SetLeftBarButtonItem(h, true);

            setNavigationTitle();
            InvokeOnMainThread(() => refreshControl.BeginRefreshing());

        }

        private void setNavigationTitle(string title = "My Follow Up")
        {
            var view = new UIView(new CGRect(0, 0, 150, 40));

            var titleLabel = new UILabel(new CGRect(0, 0, 150, 24));
            titleLabel.Font = UIFont.FromName("Raleway-Bold", 18);
            titleLabel.Text = title;
            titleLabel.TextColor = ProspectManagementColors.LabelColor;
            titleLabel.TextAlignment = UITextAlignment.Center;
            view.AddSubview(titleLabel);

            var userLabel = new UILabel(new CGRect(0, 24, 150, 16));
            userLabel.Font = UIFont.FromName("Raleway-Italic", 14);
            userLabel.Text = ViewModel?.User?.AddressBook?.Name;
            userLabel.TextColor = ProspectManagementColors.DarkColor;
            userLabel.TextAlignment = UITextAlignment.Center;
            view.AddSubview(userLabel);

            NavigationItem.TitleView = view;
        }
    }
}