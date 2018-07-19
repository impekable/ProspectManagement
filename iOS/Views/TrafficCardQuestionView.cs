using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using System.Linq;
using ProspectManagement.iOS.Utility;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class TrafficCardQuestionView : MvxViewController<TrafficCardQuestionViewModel>
    {
        AlertOverlay alertOverlay;
        protected TrafficCardQuestionViewModel TrafficCardQuestionViewModel => ViewModel as TrafficCardQuestionViewModel;

        public TrafficCardQuestionView(IntPtr handle) : base(handle)
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
            cancelButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            var saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                var bounds = UIScreen.MainScreen.Bounds;

                // show the loading overlay on the UI thread using the correct orientation sizing
                alertOverlay = new AlertOverlay(bounds, "Saving...");
                View.Add(alertOverlay);
                TrafficCardQuestionViewModel.SaveCommand.Execute(null);

            });
            saveButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
            this.NavigationItem.SetRightBarButtonItem(saveButton, true);
            this.NavigationItem.Title = "Question";

            var stringAttributes = new UIStringAttributes();
            stringAttributes.Font = UIFont.FromName("Raleway-Bold", 20);
            stringAttributes.ForegroundColor = UIColor.FromRGB(255, 255, 255);
            NavigationController.NavigationBar.BarTintColor = ProspectManagementColors.DarkColor;
            NavigationController.NavigationBar.TintColor = UIColor.White;
            NavigationController.NavigationBar.TitleTextAttributes = stringAttributes;

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