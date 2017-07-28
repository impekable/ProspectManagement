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

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxMasterSplitViewPresentation]
    public partial class SplitMasterView : MvxViewController<SplitMasterViewModel>
    {
        private IncrementalTableViewSource source;

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

            this.NavigationItem.SetRightBarButtonItem(b, true);
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 20)
            });
        }
    }
}