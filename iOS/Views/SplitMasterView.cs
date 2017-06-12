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

        private void setTableViewSource()
        {
			source = new IncrementalTableViewSource(MasterTableView, ProspectViewCell.Key); // new MvxSimpleTableViewSource(MasterTableView, ProspectViewCell.Key, ProspectViewCell.Key, null);
			source.CreateBinding<SplitMasterViewModel>(this, vm => vm.Prospects);
			MasterTableView.Source = source;
			MasterTableView.ReloadData();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            setTableViewSource();			

            var set = this.CreateBindingSet<SplitMasterView, SplitMasterViewModel>();
            set.Bind(FilterSearchBar).To(vm => vm.SearchTerm);
            set.Bind(FilterSegmentControl).To(vm => vm.SelectedSegment);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
            set.Apply();

			var refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += (sender, e) =>
			{
                ViewModel.Page = 0;
				ViewModel.Prospects = null;
				setTableViewSource();
				set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
				set.Apply();
			};

			ViewModel.LoadingDataFromBackendStarted += (sender, e) =>
			{
                InvokeOnMainThread(() => refreshControl.BeginRefreshing()); 
			};

            ViewModel.LoadingDataFromBackendCompleted += (sender, e) => {
                InvokeOnMainThread(() => refreshControl.EndRefreshing());
            };

			MasterTableView.RefreshControl = refreshControl;

			FilterSearchBar.TextChanged += (sender, e) =>
			{
				ViewModel.Prospects = null;
				setTableViewSource();
				set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
				set.Apply();
			};

			FilterSegmentControl.ValueChanged += (sender, e) =>
			{
                ViewModel.Prospects = null;
                setTableViewSource();
				set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
				set.Apply();
			};

            InvokeOnMainThread(() => refreshControl.BeginRefreshing());
            //this.Title = ViewModel.User.UserId;
            //this.NavigationItem.TitleView = new UIImageView(UIImage.FromBundle("ic-unassigned"));
            //UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            //{
            //    Font = UIFont.FromName("Raleway-Bold",20),
            //    TextColor = ProspectManagementColors.DarkColor
            //});
        }
    }
}