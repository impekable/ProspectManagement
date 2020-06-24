using CoreAnimation;
using CoreFoundation;
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
using System.Drawing;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class ProspectSMSView : MvxViewController<ProspectSMSViewModel>, IUITextViewDelegate
    {
        CGPoint keyboardEndFrameLocation;
        AlertOverlay alertOverlay;
        private SMSTableViewSource source;
        CGPoint lastScrollOffset;

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
            MessagesTableView.ScrollToRow(NSIndexPath.FromRowSection(ViewModel.SmsMessages.Count - 1, 0), UITableViewScrollPosition.Bottom, false);
        }

        private void OnHideAlertInteractionRequested(object sender, EventArgs eventArgs)
        {
            alertOverlay?.Hide();
            MessagesTableView.ScrollToRow(NSIndexPath.FromRowSection(ViewModel.SmsMessages.Count - 1, 0), UITableViewScrollPosition.Bottom, false);
        }

        public ProspectSMSView(IntPtr handle) : base(handle)
        {
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            keyboardEndFrameLocation = notification == null ? keyboardEndFrameLocation : (notification.UserInfo[UIKeyboard.FrameEndUserInfoKey] as NSValue).CGRectValue.Location;

            // Calculate the new bottom constraint, which is equal to the area covered by the keyboard
            CGRect screen = UIScreen.MainScreen.Bounds;
            nfloat newBottomConstraint = (screen.Size.Height - keyboardEndFrameLocation.Y);

            nfloat oldYContentOffset = MessagesTableView.ContentOffset.Y;
            nfloat oldTableViewHeight = MessagesTableView.Bounds.Size.Height;

            // Set the new bottom constraint
            BottomStackViewConstraint.Constant = newBottomConstraint;
            // Request layout with the new bottom constraint
            this.View.LayoutIfNeeded();


            // Set the new content offset

            // Calculate the new y content offset
            nfloat newTableViewHeight = MessagesTableView.Bounds.Size.Height;
            nfloat tableViewHeightDifference = newTableViewHeight - oldTableViewHeight;
            nfloat newYContentOffset = oldYContentOffset - tableViewHeightDifference;

            // Prevent new y content offset from exceeding max, i.e. the bottommost part of the UITableView
            nfloat contentSizeHeight = MessagesTableView.ContentSize.Height;
            nfloat possibleBottommostYContentOffset = contentSizeHeight - newTableViewHeight;
            newYContentOffset = (newYContentOffset < possibleBottommostYContentOffset ? newYContentOffset : possibleBottommostYContentOffset);

            // Prevent new y content offset from exceeding min, i.e. the topmost part of the UITableView
            nfloat possibleTopmostYContentOffset = 0;
            newYContentOffset = (possibleTopmostYContentOffset > newYContentOffset ? possibleTopmostYContentOffset : newYContentOffset);

            // Create new content offset
            CGPoint newTableViewContentOffset = new CGPoint(MessagesTableView.ContentOffset.X, newYContentOffset);
            MessagesTableView.ContentOffset = newTableViewContentOffset;

        }

        protected virtual void RegisterForKeyboardNotifications()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        private void setTableViewSource(MvxFluentBindingDescriptionSet<ProspectSMSView, ProspectSMSViewModel> set)
        {
            ViewModel.Page = 0;
            ViewModel.SmsMessages = null;

            source = new SMSTableViewSource(MessagesTableView, SMSMessageViewCell.Key); // new MvxSimpleTableViewSource(MasterTableView, ProspectViewCell.Key, ProspectViewCell.Key, null);
            source.CreateBinding<SMSInboxDetailViewModel>(this, vm => vm.SmsMessages);
            MessagesTableView.Source = source;
            MessagesTableView.RowHeight = UITableView.AutomaticDimension;
            MessagesTableView.EstimatedRowHeight = 40;
            MessagesTableView.ReloadData();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.SmsMessages);
            set.Apply();
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
            RegisterForKeyboardNotifications();

            MessagesTableView.ScrollIndicatorInsets = new UIEdgeInsets(0, 0, 0, MessagesTableView.Bounds.Size.Width - 8);
            MessagesTableView.AllowsSelection = false;
            MessagesTableView.TableFooterView = new UIView();

            var set = this.CreateBindingSet<ProspectSMSView, ProspectSMSViewModel>();
            setTableViewSource(set);
            set.Bind(MessageTextView).To(vm => vm.SmsMessageBody);

            SendButton.TouchUpInside += (sender, e) =>
            {
                var bounds = UIScreen.MainScreen.Bounds;
                alertOverlay = new AlertOverlay(bounds, "Sending...");
                View.Add(alertOverlay);
                View.EndEditing(true);
                ViewModel.SendSMSCommand.Execute(null);
            };

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

            var refreshControl = new UIRefreshControl();

            MessagesTableView.RefreshControl = refreshControl;
            InvokeOnMainThread(() => refreshControl.BeginRefreshing());

            MessagesTableView.ReloadData();

            var width = (MessageTextView.Frame.Size.Width < 250) ? 250 : MessageTextView.Frame.Size.Width;
            var height = (MessageTextView.Frame.Size.Height < 50) ? 50 : MessageTextView.Frame.Size.Height;
            MessageTextView.Frame = new CGRect(x: 0, y: 0, width: width, height: height);

            var closeButton = new UIBarButtonItem("Close", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                ViewModel.CloseCommand.Execute(null);
            });
            closeButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            NavigationItem.SetLeftBarButtonItem(closeButton, true);
            setNavigationTitle();

            ViewModel.LoadingDataFromBackendStarted += (sender, e) =>
            {
                InvokeOnMainThread(() => refreshControl.BeginRefreshing());
            };

            ViewModel.LoadingDataFromBackendCompleted += (sender, e) =>
            {
                InvokeOnMainThread(() => refreshControl.EndRefreshing());

                var numRecordsFetched = Convert.ToInt16(e);

                if (numRecordsFetched > 0)
                    MessagesTableView.ScrollToRow(NSIndexPath.FromRowSection(numRecordsFetched - 1, 0), UITableViewScrollPosition.Top, false);

                if (numRecordsFetched < ViewModel.PageSize)
                    MessagesTableView.RefreshControl = null;
                
            };
        }

        private void setNavigationTitle()
        {
            var view = new UIView(new CGRect(0, 0, 200, 40));

            var titleLabel = new UILabel(new CGRect(0, 0, 200, 24));
            titleLabel.Font = UIFont.FromName("Raleway-Bold", 18);
            titleLabel.Text = ViewModel.Prospect.FirstName + " " + ViewModel.Prospect.LastName;
            titleLabel.TextColor = ProspectManagementColors.LabelColor;
            titleLabel.TextAlignment = UITextAlignment.Center;
            view.AddSubview(titleLabel);

            var taskLabel = new UILabel(new CGRect(0, 24, 200, 16));
            taskLabel.Font = UIFont.FromName("Raleway-Italic", 14);
            taskLabel.Text = ViewModel.Prospect.MobilePhoneNumber?.Phone;
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
            var width = (textView.Frame.Size.Width < 250) ? 250 : textView.Frame.Size.Width;
            var height = (textView.Frame.Size.Height < 50) ? 50 : textView.Frame.Size.Height;
            textView.Frame = new CGRect(x: 0, y: 0, width: width, height: height);

            OnKeyboardNotification(null);
        }

    }
}