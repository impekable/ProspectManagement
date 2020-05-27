using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class CallTaskView : MvxViewController<CallTaskViewModel>
    {
        private CustomAlertController _CallAlertController;

        public CallTaskView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _CallAlertController = new CustomAlertController("Call");

            var set = this.CreateBindingSet<CallTaskView, CallTaskViewModel>();
            set.Bind(CallScriptTextView).To(vm => vm.Activity.TemplateData);
            set.Bind(_CallAlertController).For(p => p.AlertController).To(vm => vm.Phones);
            set.Bind(_CallAlertController).For(p => p.SelectedCode).To(vm => vm.SelectedCall);

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

            CallButton.TouchUpInside += (sender, e) =>
            {
                var popPresenter = _CallAlertController.AlertController.PopoverPresentationController;
                if (popPresenter != null)
                {
                    popPresenter.SourceView = CallButton;
                    popPresenter.SourceRect = CallButton.Bounds;
                }
                PresentViewController(_CallAlertController.AlertController, true, null);
            };

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

        private void setNavigationTitle(string title = "Follow Up Call")
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