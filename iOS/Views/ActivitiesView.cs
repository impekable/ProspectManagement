using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Sources;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxSplitViewPresentation(Position = MasterDetailPosition.Detail)]
    public partial class ActivitiesView : MvxViewController<ActivitiesViewModel>
    {
        private IMvxInteraction _activityAddedInteraction;
        public IMvxInteraction ActivityAddedInteraction
        {
            get => _activityAddedInteraction;
            set
            {
                if (_activityAddedInteraction != null)
                    _activityAddedInteraction.Requested -= OnActivityAddedInteraction;

                _activityAddedInteraction = value;
                _activityAddedInteraction.Requested += OnActivityAddedInteraction;
            }
        }

        private async void OnActivityAddedInteraction(object sender, EventArgs eventArgs)
        {
            setTabBar();
        }

        private IMvxInteraction _clearDetailsInteraction;
        public IMvxInteraction ClearDetailsInteraction
        {
            get => _clearDetailsInteraction;
            set
            {
                if (_clearDetailsInteraction != null)
                    _clearDetailsInteraction.Requested -= OnClearDetailsInteractionRequested;

                _clearDetailsInteraction = value;
                _clearDetailsInteraction.Requested += OnClearDetailsInteractionRequested;
            }
        }


        private async void OnClearDetailsInteractionRequested(object sender, EventArgs eventArgs)
        {
            ActivitiesTableView.Hidden = true;
            AddNoteButton.Hidden = true;
            AddVisitButton.Hidden = true;
            ProspectTabBar.Hidden = true;
            this.NavigationItem.Title = "";
        }


        public ActivitiesView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ActivitiesTableView.TableFooterView = new UIView();

            var source = new ActivityTableViewSource(ActivitiesTableView, ActivitiesViewCell.Key);
            ActivitiesTableView.Source = source;
            ActivitiesTableView.RowHeight = UITableView.AutomaticDimension;
            ActivitiesTableView.EstimatedRowHeight = 40;

            var set = this.CreateBindingSet<ActivitiesView, ActivitiesViewModel>();
            set.Bind(source).To(vm => vm.ActivitiesList);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
            set.Bind(this).For(view => view.ClearDetailsInteraction).To(viewModel => viewModel.ClearDetailsInteraction).OneWay();
            set.Bind(this).For(view => view.ActivityAddedInteraction).To(viewModel => viewModel.ActivityAddedInteraction).OneWay();

            set.Bind(CompleteApptButton).To(vm => vm.CompleteApptCommand);
            set.Bind(CompleteApptButton).For(v => v.Hidden).To(vm => vm.IsLeadWithAppointment).WithConversion(new InverseValueConverter());
            set.Bind(AddNoteButton).To(vm => vm.AddNoteCommand);
            set.Bind(AddVisitButton).To(vm => vm.AddVisitCommand);
            set.Bind(AddNoteButton).For(c => c.Hidden).To(vm => vm.AssignedProspect).WithConversion(new InverseValueConverter());
            set.Bind(AddVisitButton).For(v => v.Hidden).To(vm => vm.AssignedWithoutAppointment).WithConversion(new InverseValueConverter());

            set.Apply();

            var refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += (sender, e) =>
            {
                //setTableViewSource(set);
                ViewModel.RefreshCommand.Execute(null);
            };

            ActivitiesTableView.RefreshControl = refreshControl;
            InvokeOnMainThread(() => refreshControl.BeginRefreshing());

            ActivitiesTableView.ReloadData();

            ViewModel.LoadingDataFromBackendStarted += (sender, e) =>
            {
                InvokeOnMainThread(() => refreshControl.BeginRefreshing());
            };

            ViewModel.LoadingDataFromBackendCompleted += (sender, e) =>
            {
                InvokeOnMainThread(() => refreshControl.EndRefreshing());
            };

            setTabBar();
        }

        private void setTabBar()
        {
            foreach (UITabBarItem item in ProspectTabBar.Items)
            {
                if (ViewModel.IsLead && (item.Tag == 1 || item.Tag == 2))
                {
                    ProspectTabBar.Items[item.Tag].Enabled = false;
                }
                else
                {
                    ProspectTabBar.Items[item.Tag].Enabled = true;
                }

                if (item.Tag == 3)
                {
                    item.SetTitleTextAttributes(new UITextAttributes()
                    {
                        Font = UIFont.FromName("Raleway-Bold", 10),
                        TextColor = UIColor.Black
                    }, UIControlState.Normal);

                }
                else
                {
                    item.SetTitleTextAttributes(new UITextAttributes()
                    {
                        Font = UIFont.FromName("Raleway-Regular", 10),
                        TextColor = UIColor.Black
                    }, UIControlState.Normal);
                }
            }

            ProspectTabBar.SelectedItem = ProspectTabBar.Items[3];
            ProspectTabBar.ItemSelected += (sender, e) =>
            {
                if (e.Item.Tag == 0)
                    ViewModel.ShowDetailTab.Execute(null);
                else if (e.Item.Tag == 1)
                    ViewModel.ShowCobuyerTab.Execute(null);
                else if (e.Item.Tag == 2)
                    ViewModel.ShowTrafficCardTab.Execute(null);
            };
        }
    }
}
