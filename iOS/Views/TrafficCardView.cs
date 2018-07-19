using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Interactions;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.ViewModels;
using MvvmCross.Base;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace ProspectManagement.iOS.Views
{
	[MvxFromStoryboard("Main")]
	[MvxSplitViewPresentation(Position = MasterDetailPosition.Detail)]
	public partial class TrafficCardView : MvxViewController<TrafficCardViewModel>
    {
		protected TrafficCardViewModel TrafficCardViewModel => ViewModel as TrafficCardViewModel;

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
            QuestionsTableView.Hidden = true;
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

		private async void OnUpdateRowInteractionRequested(object sender, MvxValueEventArgs<TableRow> eventArgs)
		{

			NSIndexPath[] rowsToReload = new NSIndexPath[] { NSIndexPath.FromRowSection(eventArgs.Value.TableRowToUpdate, 0) };
            var c = QuestionsTableView.Source.GetCell(QuestionsTableView,NSIndexPath.FromRowSection(eventArgs.Value.TableRowToUpdate, 0));
            QuestionsTableView.ReloadRows(rowsToReload, UITableViewRowAnimation.None);
		}

		public TrafficCardView (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			QuestionsTableView.TableFooterView = new UIView();

			var source = new MvxSimpleTableViewSource(QuestionsTableView, QuestionViewCell.Key, QuestionViewCell.Key, null);
			QuestionsTableView.Source = source;
            QuestionsTableView.RowHeight = UITableView.AutomaticDimension;
            QuestionsTableView.EstimatedRowHeight = 40;
			var set = this.CreateBindingSet<TrafficCardView, TrafficCardViewModel>();
			set.Bind(source).To(vm => vm.Responses);
			set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.SelectionChangedCommand);
			set.Bind(this).For(view => view.UpdateRowInteraction).To(viewModel => viewModel.UpdateRowInteraction).OneWay();
            set.Bind(this).For(view => view.ClearDetailsInteraction).To(viewModel => viewModel.ClearDetailsInteraction).OneWay();
			set.Apply();

            QuestionsTableView.ReloadData();

			foreach (UITabBarItem item in ProspectTabBar.Items)
			{
				if (item.Tag == 2)
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

            ProspectTabBar.SelectedItem = ProspectTabBar.Items[2];
			ProspectTabBar.ItemSelected += (sender, e) =>
			{
				if (e.Item.Tag == 0)
					ViewModel.ShowDetailTab.Execute(null);
				else if (e.Item.Tag == 1)
					ViewModel.ShowCobuyerTab.Execute(null);
			};

            //this.NavigationItem.Title = TrafficCardViewModel.Prospect.FirstName + " " + TrafficCardViewModel.Prospect.LastName + " Traffic Card";
		}
    }
}