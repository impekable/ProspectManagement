using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using Sequence.Plugins.InfiniteScroll.iOS;
using ProspectManagement.iOS.Utility;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Interactions;
using MvvmCross.Platform.Core;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxMasterSplitViewPresentation]
    public partial class SplitMasterView : MvxViewController<SplitMasterViewModel>
    {
        private IncrementalTableViewSource source;

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

        private async void OnUpdateRowInteractionRequested(object sender, MvxValueEventArgs<TableRow> eventArgs)
        {
            NSIndexPath[] rowsToReload = new NSIndexPath[] { NSIndexPath.FromRowSection(eventArgs.Value.TableRowToUpdate, 0) };// points to second row in the first section of the model
            MasterTableView.ReloadRows(rowsToReload, UITableViewRowAnimation.None);
            MasterTableView.SelectRow(NSIndexPath.FromRowSection(eventArgs.Value.TableRowToUpdate, 0), true, UITableViewScrollPosition.None);
        }

        public SplitMasterView(IntPtr handle) : base(handle)
        {
        }

        private void setTableViewSource(MvxFluentBindingDescriptionSet<SplitMasterView, SplitMasterViewModel> set)
        {
            ViewModel.Page = 0;
            ViewModel.Prospects = null;

            source = new IncrementalTableViewSource(MasterTableView, ProspectViewCell.Key); // new MvxSimpleTableViewSource(MasterTableView, ProspectViewCell.Key, ProspectViewCell.Key, null);
            source.CreateBinding<SplitMasterViewModel>(this, vm => vm.Prospects);
            MasterTableView.Source = source;
            MasterTableView.ReloadData();
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
            set.Apply();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<SplitMasterView, SplitMasterViewModel>();
            set.Bind(FilterSearchBar).To(vm => vm.SearchTerm);
            set.Bind(FilterSegmentControl).To(vm => vm.SelectedSegment);
			set.Bind(this).For(view => view.UpdateRowInteraction).To(viewModel => viewModel.UpdateRowInteraction).OneWay();
			
			setTableViewSource(set);

            var refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += (sender, e) =>
            {
                setTableViewSource(set);
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

            ViewModel.LogoutCompleted += (sender, e) =>
            {
                FilterSearchBar.Hidden = true;
                FilterSegmentControl.Hidden = true;
                MasterTableView.Hidden = true;
                setTableViewSource(set);
                this.NavigationItem.RightBarButtonItem.Title = "Login";
            };

            ViewModel.LoginCompleted += (sender, e) =>
            {
                FilterSearchBar.Hidden = false;
                FilterSegmentControl.Hidden = false;
                MasterTableView.Hidden = false;
                setTableViewSource(set);
                this.NavigationItem.RightBarButtonItem.Title = "Logout";
            };

            FilterSearchBar.CancelButtonClicked += (sender, e) =>
           {
               ViewModel.SearchTerm = null;
               setTableViewSource(set);
               ((UISearchBar)sender).ResignFirstResponder();
           };

            FilterSearchBar.SearchButtonClicked += (sender, e) =>
           {
               ((UISearchBar)sender).ResignFirstResponder();
           };

            FilterSearchBar.TextChanged += (sender, e) =>
            {
                setTableViewSource(set);
            };

            FilterSegmentControl.ValueChanged += (sender, e) =>
            {
                setTableViewSource(set);
            };

            InvokeOnMainThread(() => refreshControl.BeginRefreshing());

            var b = new UIBarButtonItem("Logout", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                ViewModel.LogoutCommand.Execute(null);
            });

            b.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = ProspectManagementColors.DarkColor
            }, UIControlState.Normal);

            this.NavigationItem.SetRightBarButtonItem(b, true);

            var stringAttributes = new UIStringAttributes();
            stringAttributes.Font = UIFont.FromName("Raleway-Bold", 20);
            stringAttributes.ForegroundColor = UIColor.Black;
            //NavigationController.NavigationBar.BarTintColor = ProspectManagementColors.DarkColor;
            NavigationController.NavigationBar.TintColor = ProspectManagementColors.DarkColor;
            NavigationController.NavigationBar.TitleTextAttributes = stringAttributes;

        }
    }
}