using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views.Presenters.Attributes;
using System.Linq;

namespace ProspectManagement.iOS.Views
{
	[MvxFromStoryboard("Main")]
	[MvxModalPresentation(WrapInNavigationController = true)]
	public partial class TrafficCardQuestionView : MvxViewController<TrafficCardQuestionViewModel>
	{
		protected TrafficCardQuestionViewModel TrafficCardQuestionViewModel => ViewModel as TrafficCardQuestionViewModel;

		public TrafficCardQuestionView(IntPtr handle) : base (handle)
        {
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AnswersTableView.TableFooterView = new UIView();

			var source = new MvxSimpleTableViewSource(AnswersTableView, AnswerViewCell.Key, AnswerViewCell.Key, null);
			AnswersTableView.Source = source;

			var set = this.CreateBindingSet<TrafficCardQuestionView, TrafficCardQuestionViewModel>();
			set.Bind(TrafficCardQuestionLabel).To(vm => vm.CurrentQuestion.QuestionText);
			set.Bind(source).To(vm => vm.CurrentQuestion.TrafficCardAnswers);
			set.Bind(source).For(s => s.SelectedItem).To(vm => vm.CurrentAnswer);
			set.Apply();

			AnswersTableView.ReloadData();

			var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				TrafficCardQuestionViewModel.CancelCommand.Execute(null);
			});

			var saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				TrafficCardQuestionViewModel.SaveCommand.Execute(null);

			});

			this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
			this.NavigationItem.SetRightBarButtonItem(saveButton, true);
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
			{
				Font = UIFont.FromName("Raleway-Bold", 20)
			});
			this.NavigationItem.Title = "Question";

            selectAnswerInTableView();
		}

		private void selectAnswerInTableView()
		{
			if (TrafficCardQuestionViewModel.Response.AnswerNumber > 0)
			{
				var answer = TrafficCardQuestionViewModel.CurrentQuestion.TrafficCardAnswers.FirstOrDefault(a => a.AnswerNumber == TrafficCardQuestionViewModel.Response.AnswerNumber);
                var i = TrafficCardQuestionViewModel.CurrentQuestion.TrafficCardAnswers.IndexOf(answer);
                AnswersTableView.SelectRow(NSIndexPath.FromRowSection(i, 0), true, UITableViewScrollPosition.None);
			}
		}
	}
}