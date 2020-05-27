using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;
using WebKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class EmailTaskView : MvxViewController<EmailTaskViewModel>
    {
        private IMvxInteraction _getContentInteraction;
        public IMvxInteraction GetContentInteraction
        {
            get => _getContentInteraction;
            set
            {
                if (_getContentInteraction != null)
                    _getContentInteraction.Requested -= OnGetContentInteractionRequested;

                _getContentInteraction = value;
                _getContentInteraction.Requested += OnGetContentInteractionRequested;
            }
        }

        private async void OnGetContentInteractionRequested(object sender, EventArgs eventArgs)
        {
            EmailContentWebView.EvaluateJavaScript("document.body.innerHTML", (r, e) =>
            {
                ViewModel.Activity.TemplateData = r as NSString;
                ViewModel.SendEmailCommand.Execute();
            });
        }

        public EmailTaskView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<EmailTaskView, EmailTaskViewModel>();

            EmailContentWebView.LoadHtmlString(ViewModel.Activity.TemplateData, null);
            set.Bind(FromLabel).To(vm => vm.User.Email.EmailAddress);
            set.Bind(ToLabel).To(vm => vm.Activity.Prospect.Email.EmailAddress);
            set.Bind(SubjectLabel).To(vm => vm.Activity.EmailSubject);
            set.Bind(SendEmailButton).To(vm => vm.GetContentCommand);
            set.Bind(this).For(view => view.GetContentInteraction).To(viewModel => viewModel.GetContentInteraction).OneWay();

            set.Apply();

            if (NavigationController != null)
            {
                var stringAttributes = new UIStringAttributes();
                stringAttributes.Font = UIFont.FromName("Raleway-Bold", 20);
                stringAttributes.ForegroundColor = UIColor.FromRGB(255, 255, 255);
                NavigationController.NavigationBar.BarTintColor = ProspectManagementColors.DarkColor;
                NavigationController.NavigationBar.TintColor = UIColor.White;
                NavigationController.NavigationBar.TitleTextAttributes = stringAttributes;
                //NavigationController.NavigationBarHidden = true;
            }

            setNavigationTitle();

            var cancelButton = new UIBarButtonItem("Close", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                ViewModel.CloseCommand.Execute(null);
            });
            cancelButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
        }

        private void setNavigationTitle(string title = "Follow Up Email")
        {
            var view = new UIView(new CGRect(0, 0, 200, 40));

            var titleLabel = new UILabel(new CGRect(0, 0, 200, 24));
            titleLabel.Font = UIFont.FromName("Raleway-Bold", 18);
            titleLabel.Text = title;
            titleLabel.TextColor = ProspectManagementColors.LabelColor;
            titleLabel.TextAlignment = UITextAlignment.Center;
            view.AddSubview(titleLabel);

            var taskLabel = new UILabel(new CGRect(0, 24, 200, 16));
            taskLabel.Font = UIFont.FromName("Raleway-Italic", 14);
            taskLabel.Text = ViewModel?.Activity?.Subject + " - " + ViewModel?.Activity?.Prospect.FirstName + " " + ViewModel?.Activity?.Prospect.LastName;
            taskLabel.TextColor = ProspectManagementColors.LabelColor;
            taskLabel.TextAlignment = UITextAlignment.Center;
            view.AddSubview(taskLabel);

            this.NavigationItem.TitleView = view;
        }
    }
}