using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Interactions;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.ViewModels;
using MvvmCross.Base;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxSplitViewPresentation(Position = MasterDetailPosition.Detail)]
    public partial class CobuyerView : MvxViewController<CobuyerViewModel>
    {
        private UIBarButtonItem _AddButton;
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
            CobuyerTableView.Hidden = true;
            if (_AddButton != null)
            {
                _AddButton.Enabled = false;
                _AddButton.Title = "";
            }
            ProspectTabBar.Hidden = true;
            this.NavigationItem.Title = "";
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

		private IMvxInteraction _addRowInteraction;
		public IMvxInteraction AddRowInteraction
		{
			get => _addRowInteraction;
			set
			{
				if (_addRowInteraction != null)
					_addRowInteraction.Requested -= OnAddRowInteractionRequested;

				_addRowInteraction = value;
				_addRowInteraction.Requested += OnAddRowInteractionRequested;
			}
		}

		private async void OnUpdateRowInteractionRequested(object sender, MvxValueEventArgs<TableRow> eventArgs)
		{
			NSIndexPath[] rowsToReload = new NSIndexPath[] { NSIndexPath.FromRowSection(eventArgs.Value.TableRowToUpdate, 0) };// points to second row in the first section of the model
            CobuyerTableView.ReloadRows(rowsToReload, UITableViewRowAnimation.None);
		}

		private async void OnAddRowInteractionRequested(object sender, EventArgs eventArgs)
		{
            CobuyerTableView.ReloadData();
		}


		public CobuyerView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


			CobuyerTableView.TableFooterView = new UIView();

            var source = new MvxSimpleTableViewSource(CobuyerTableView, CobuyerViewCell.Key, CobuyerViewCell.Key, null);
			CobuyerTableView.Source = source;

			var set = this.CreateBindingSet<CobuyerView, CobuyerViewModel>();
            set.Bind(source).To(vm => vm.CobuyersList);
			set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
			set.Bind(this).For(view => view.AddRowInteraction).To(viewModel => viewModel.AddRowInteraction).OneWay();
            set.Bind(this).For(view => view.UpdateRowInteraction).To(viewModel => viewModel.UpdateRowInteraction).OneWay();
            set.Bind(this).For(view => view.ClearDetailsInteraction).To(viewModel => viewModel.ClearDetailsInteraction).OneWay();

			set.Apply();

            CobuyerTableView.ReloadData();

            _AddButton = new UIBarButtonItem("Add", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				ViewModel.AddCobuyerCommand.Execute(null);
			});

            this.NavigationItem.SetRightBarButtonItem(_AddButton, true);

			foreach (UITabBarItem item in ProspectTabBar.Items)
			{
				if (item.Tag == 1)
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



            ProspectTabBar.SelectedItem = ProspectTabBar.Items[1];
            ProspectTabBar.ItemSelected += (sender, e) =>
            {
                if (e.Item.Tag == 0)
                    ViewModel.ShowDetailTab.Execute(null);
                else if (e.Item.Tag == 2)
                    ViewModel.ShowTrafficCardTab.Execute(null);
                else if (e.Item.Tag == 3)
                    ViewModel.ShowContactHistoryTab.Execute(null);
                    
            };

        }
    }
}