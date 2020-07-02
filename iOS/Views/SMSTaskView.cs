using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Sources;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class SMSTaskView : MvxBaseViewController<SMSTaskViewModel>, IUITextViewDelegate
    {
        AlertOverlay alertOverlay;

        public SMSTaskView(IntPtr handle) : base(handle)
        {
        }

        private IMvxInteraction _showAlertInteraction;
        public IMvxInteraction ShowAlertInteraction
        {
            get => _showAlertInteraction;
            set
            {
                if (_showAlertInteraction != null)
                    _showAlertInteraction.Requested -= OnShowAlertInteractionRequested;

                _showAlertInteraction = value;
                _showAlertInteraction.Requested += OnShowAlertInteractionRequested;
            }
        }

        private IMvxInteraction _hideAlertInteraction;
        public IMvxInteraction HideAlertInteraction
        {
            get => _hideAlertInteraction;
            set
            {
                if (_hideAlertInteraction != null)
                    _hideAlertInteraction.Requested -= OnHideAlertInteractionRequested;

                _hideAlertInteraction = value;
                _hideAlertInteraction.Requested += OnHideAlertInteractionRequested;
            }
        }

        private void OnShowAlertInteractionRequested(object sender, EventArgs eventArgs)
        {
            var bounds = UIScreen.MainScreen.Bounds;
            alertOverlay = new AlertOverlay(bounds, "Sending...");
            View.Add(alertOverlay);
        }

        private void OnHideAlertInteractionRequested(object sender, EventArgs eventArgs)
        {
            alertOverlay.Hide();
        }

        private void OnPopInteractionRequested(object sender, EventArgs eventArgs)
        {
            NavigationController.PopViewController(true);
        }

        public override bool HandlesKeyboardNotifications()
        {
            return true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

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

            // setup the keyboard handling
            InitKeyboardHandling();

            //MessagesTableView.Transform = CGAffineTransform.MakeRotation(-3.14159f);
            MessagesTableView.ScrollIndicatorInsets = new UIEdgeInsets(0, 0, 0, MessagesTableView.Bounds.Size.Width - 8);

            MessagesTableView.TableFooterView = new UIView();

            var source = new SMSTableViewSource(MessagesTableView, SMSMessageViewCell.Key);
            MessagesTableView.Source = source;
            MessagesTableView.RowHeight = UITableView.AutomaticDimension;
            MessagesTableView.EstimatedRowHeight = 40;

            var set = this.CreateBindingSet<SMSTaskView, SMSTaskViewModel>();
            set.Bind(source).To(vm => vm.SmsMessages);
            set.Bind(MessageTextView).To(vm => vm.SmsMessageBody);
            set.Bind(SendButton).To(vm => vm.SendSMSCommand);

            set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();
            set.Bind(this).For(view => view.ShowAlertInteraction).To(viewModel => viewModel.ShowAlertInteraction).OneWay();
            set.Apply();

            MessageTextView.Layer.BorderColor = UIColor.SystemGrayColor.CGColor;
            MessageTextView.Layer.BorderWidth = 1;
            MessageTextView.Layer.CornerRadius = 8;

            MessageTextView.Delegate = this;
            MessageTextView.TranslatesAutoresizingMaskIntoConstraints = true;
            MessageTextView.SizeToFit();
            MessageTextView.ScrollEnabled = false;
            MessagesTableView.ReloadData();

            var width = UIScreen.MainScreen.Bounds.Width > 650 ? 550 : 250;
            var height = (MessageTextView.Frame.Size.Height < 50) ? 50 : MessageTextView.Frame.Size.Height;
            MessageTextView.Frame = new CGRect(x: 0, y: 0, width: width, height: height);

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

        private void setNavigationTitle(string title = "Follow Up Text")
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

            NavigationItem.TitleView = view;
        }

        [Export("textViewDidChange:")]
        public void Changed(UITextView textView)
        {
            textView.TranslatesAutoresizingMaskIntoConstraints = true;
            textView.SizeToFit();
            textView.ScrollEnabled = false;
            var width = UIScreen.MainScreen.Bounds.Width > 650 ? 550 : 250;
            var height = (textView.Frame.Size.Height < 50) ? 50 : textView.Frame.Size.Height;
            textView.Frame = new CGRect(x: 0, y: 0, width: width, height: height);
        }
    }
}