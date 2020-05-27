using Foundation;
using System;
using UIKit;
using MvvmCross.Platforms.Ios.Views;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using ProspectManagement.iOS.Utility;
using System.Threading.Tasks;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Converters;
using CoreGraphics;
using MvvmCross.Base;
using ProspectManagement.iOS.Sources;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxSplitViewPresentation(Position = MasterDetailPosition.Master)]
    public partial class SplitMasterView : MvxViewController<SplitMasterViewModel>
    {
        private IncrementalTableViewSource source;

        private IMvxInteraction<Filter> _filterInteraction;
        public IMvxInteraction<Filter> FilterInteraction
        {
            get => _filterInteraction;
            set
            {
                if (_filterInteraction != null)
                    _filterInteraction.Requested -= OnFilterInteractionRequested;

                _filterInteraction = value;
                _filterInteraction.Requested += OnFilterInteractionRequested;
            }
        }

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

        private async void OnFilterInteractionRequested(object sender, MvxValueEventArgs<Filter> eventArgs)
        {
            if (!ViewModel.FilterActive)
            {
                LeadsOnlyButton.SetImage(UIImage.FromBundle("ic-filter-active"), UIControlState.Normal);
            }
            else
            {
                LeadsOnlyButton.SetImage(UIImage.FromBundle("ic-filter-inactive"), UIControlState.Normal);
            }
            ViewModel.FilterActive = !ViewModel.FilterActive;

            var set = this.CreateBindingSet<SplitMasterView, SplitMasterViewModel>();
            setTableViewSource(set);
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
            MasterTableView.RowHeight = UITableView.AutomaticDimension;
            MasterTableView.EstimatedRowHeight = 40;
            MasterTableView.ReloadData();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.Prospects);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
            set.Apply();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            System.Diagnostics.Debug.WriteLine("Loading Master View: " + this.Handle);

            var set = this.CreateBindingSet<SplitMasterView, SplitMasterViewModel>();
            set.Bind(FilterSearchBar).To(vm => vm.SearchTerm);
            set.Bind(FilterSegmentControl).To(vm => vm.SelectedSegment);
            //set.Bind(LeadsOnlyButton).To(vm => vm.FilterCommand);
            LeadsOnlyButton.TouchUpInside += (sender, e) =>
            {
                OnFilterInteractionRequested(sender, new MvxValueEventArgs<Filter>(new Filter() { Active = !ViewModel.FilterActive }));
                setNavigationTitle(ViewModel.FilterActive ? "My Leads" : "My Prospects");
            };
            set.Bind(FilterLabel).For(v => v.Hidden).To(vm => vm.FilterActive).WithConversion(new InverseValueConverter()); ;
            set.Bind(CurrentFilterLabel).For(v => v.Hidden).To(vm => vm.FilterActive).WithConversion(new InverseValueConverter()); ;
            set.Bind(this).For(view => view.UpdateRowInteraction).To(viewModel => viewModel.UpdateRowInteraction).OneWay();
            set.Bind(this).For(view => view.FilterInteraction).To(viewModel => viewModel.FilterInteraction).OneWay();
            setTableViewSource(set);

            FilterSearchBar.ScopeButtonTitles = new string[] { "All", "Name", "Notes", "Phone", "Email", "Address" };
            FilterSearchBar.SelectedScopeButtonIndexChanged += (sender, e) =>
            {
                ViewModel.ScopeFilter = FilterSearchBar.ScopeButtonTitles[(int)e.SelectedScope];
            };

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

            FilterSearchBar.CancelButtonClicked += (sender, e) =>
           {
               FilterSearchBar.ShowsScopeBar = false;
               ViewModel.SearchTerm = null;
               setTableViewSource(set);
               ((UISearchBar)sender).ResignFirstResponder();
           };

            FilterSearchBar.SearchButtonClicked += (sender, e) =>
           {
               FilterSearchBar.ShowsScopeBar = false;
               ((UISearchBar)sender).ResignFirstResponder();
               setTableViewSource(set);
           };

            FilterSearchBar.OnEditingStarted += (sender, e) =>
            {
                FilterSearchBar.ShowsScopeBar = true;
                //if (String.IsNullOrEmpty(FilterSearchBar.Text))
                //{
                //    setTableViewSource(set);
                //}
            };

            var inactiveFilterImage = UIImage.FromBundle("ic-filter-inactive");
            LeadsOnlyButton.SetImage(inactiveFilterImage, UIControlState.Normal);

            FilterSegmentControl.ValueChanged += (sender, e) =>
            {
                setTableViewSource(set);
            };

            if (ViewModel.User.UsingTelephony)
            {
                //Only allow them to go to home screen if using telophony,
                //since home screen has sms inbox link + my follow up
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
            }
            else
            {
                var b = new UIBarButtonItem("Logout", UIBarButtonItemStyle.Plain, (sender, e) =>
                {
                    ViewModel.LogoutCommand.Execute(null);
                });

                b.SetTitleTextAttributes(new UITextAttributes()
                {
                    Font = UIFont.FromName("Raleway-Bold", 18),
                    TextColor = ProspectManagementColors.DarkColor
                }, UIControlState.Normal);

                NavigationItem.SetRightBarButtonItem(b, true);

            }
            setNavigationTitle();
            InvokeOnMainThread(() => refreshControl.BeginRefreshing());

        }

        private void setNavigationTitle(string title = "My Prospects")
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