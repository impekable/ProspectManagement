using CoreGraphics;
using Foundation;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Sources;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxSplitViewPresentation(Position = MasterDetailPosition.Master)]
    public partial class SMSInboxView : MvxViewController<SMSInboxViewModel>
    {
        private SMSInboxTableViewSource source;

        private IMvxInteraction<TableRow> _updateRowInteraction;
        public IMvxInteraction<TableRow> UpdateRowInteraction
        {
            get => _updateRowInteraction;
            set
            {
                if (_updateRowInteraction != null)
                    _updateRowInteraction.Requested -= OnUpdateRowInteractionRequested;

                _updateRowInteraction = value;
                _updateRowInteraction.Requested += OnUpdateRowInteractionRequested;
            }
        }

        private void OnUpdateRowInteractionRequested(object sender, MvxValueEventArgs<TableRow> eventArgs)
        {
            NSIndexPath[] rowsToReload = new NSIndexPath[] { NSIndexPath.FromRowSection(eventArgs.Value.TableRowToUpdate, 0) };
            MasterTableView.ReloadRows(rowsToReload, UITableViewRowAnimation.None);
            MasterTableView.SelectRow(NSIndexPath.FromRowSection(eventArgs.Value.TableRowToUpdate, 0), true, UITableViewScrollPosition.None);
        }

        public SMSInboxView(IntPtr handle) : base(handle)
        {
        }

        private void setTableViewSource(MvxFluentBindingDescriptionSet<SMSInboxView, SMSInboxViewModel> set)
        {
            ViewModel.Page = 0;
            ViewModel.SMSActivities = null;

            source = new SMSInboxTableViewSource(MasterTableView, SMSInboxViewCell.Key); // new MvxSimpleTableViewSource(MasterTableView, ProspectViewCell.Key, ProspectViewCell.Key, null);
            source.CreateBinding<SMSInboxViewModel>(this, vm => vm.SMSActivities);
            MasterTableView.Source = source;
            MasterTableView.RowHeight = UITableView.AutomaticDimension;
            MasterTableView.EstimatedRowHeight = 40;
            MasterTableView.ReloadData();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.SMSActivities);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
            set.Apply();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            System.Diagnostics.Debug.WriteLine("Loading Master View: " + this.Handle);

            var set = this.CreateBindingSet<SMSInboxView, SMSInboxViewModel>();
            set.Bind(FilterSegmentControl).To(vm => vm.SelectedSegment);
            set.Bind(this).For(view => view.UpdateRowInteraction).To(viewModel => viewModel.UpdateRowInteraction).OneWay();
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

            this.NavigationItem.SetLeftBarButtonItem(h, true);

            setNavigationTitle();
            InvokeOnMainThread(() => refreshControl.BeginRefreshing());

        }

        private void setNavigationTitle(string title = "SMS Inbox")
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

            this.NavigationItem.TitleView = view;
        }
    }
}