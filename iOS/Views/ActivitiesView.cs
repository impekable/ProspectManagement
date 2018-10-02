using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.ViewModels;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxSplitViewPresentation(Position = MasterDetailPosition.Detail)]
    public partial class ActivitiesView : MvxViewController<ActivitiesViewModel>
    {

        private MvxSimpleTableViewSource source;

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

            var source = new MvxSimpleTableViewSource(ActivitiesTableView, ActivitiesViewCell.Key, ActivitiesViewCell.Key, null);
            ActivitiesTableView.Source = source;
            ActivitiesTableView.RowHeight = UITableView.AutomaticDimension;
            ActivitiesTableView.EstimatedRowHeight = 40;

            var set = this.CreateBindingSet<ActivitiesView, ActivitiesViewModel>();
            set.Bind(source).To(vm => vm.ActivitiesList);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
            set.Bind(this).For(view => view.ClearDetailsInteraction).To(viewModel => viewModel.ClearDetailsInteraction).OneWay();

            set.Apply();

            ActivitiesTableView.ReloadData();

            foreach (UITabBarItem item in ProspectTabBar.Items)
            {
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