using MvvmCross.iOS.Views;
using ProspectManagement.iOS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Foundation;
using System.Drawing;
using CoreGraphics;
using ProspectManagement.iOS.Extensions;

namespace ProspectManagement.iOS.Views
{
	public class BaseView : MvxViewController
	{
		/// <summary>
		/// Set this field to any view inside the scroll view to center this view instead of the current responder
		/// </summary>
		protected UIView ViewToCenterOnKeyboardShown;
		protected float ScrollViewInset = 0;

		public BaseView(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (NavigationController != null)
			{
				//var stringAttributes = new UIStringAttributes();
				//stringAttributes.Font = UIFont.SystemFontOfSize(16);
				//stringAttributes.ForegroundColor = UIColor.FromRGB(255, 255, 255);
				//NavigationController.NavigationBar.BarTintColor = RegistrationColors.DarkColor;
				//NavigationController.NavigationBar.TintColor = UIColor.White;
				//NavigationController.NavigationBar.TitleTextAttributes = stringAttributes;
				NavigationController.NavigationBarHidden = true;
			}

			CreateBindings();
			if (HandlesKeyboardNotifications())
			{
				RegisterForKeyboardNotifications();
			}

			DismissKeyboardOnBackgroundTap();
		}

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}

		protected virtual void CreateBindings()
		{

		}

		public virtual bool HandlesKeyboardNotifications()
		{
			return false;
		}

		protected virtual void RegisterForKeyboardNotifications()
		{
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		/// <summary>
		/// Gets the UIView that represents the "active" user input control (e.g. textfield, or button under a text field)
		/// </summary>
		/// <returns>
		/// A <see cref="UIView"/>
		/// </returns>
		protected virtual UIView KeyboardGetActiveView()
		{
			return View.FindFirstResponder();
		}

		private void OnKeyboardNotification(NSNotification notification)
		{
			if (!IsViewLoaded) return;

			//Check if the keyboard is becoming visible
			var visible = notification.Name == UIKeyboard.WillShowNotification;

			//Pass the notification, calculating keyboard height, etc.
			var keyboardFrame = visible ? UIKeyboard.FrameEndFromNotification(notification)	: UIKeyboard.FrameBeginFromNotification(notification);

			OnKeyboardChanged(visible, ((float)keyboardFrame.Height));

		}

		/// <summary>
		/// Override this method to apply custom logic when the keyboard is shown/hidden
		/// </summary>
		/// <param name='visible'>
		/// If the keyboard is visible
		/// </param>
		/// <param name='keyboardHeight'>
		/// Calculated height of the keyboard (width not generally needed here)
		/// </param>
		protected virtual void OnKeyboardChanged(bool visible, float keyboardHeight)
		{
			var activeView = ViewToCenterOnKeyboardShown ?? KeyboardGetActiveView();
			if (activeView == null)
				return;

			var scrollView = activeView.FindSuperviewOfType(View, typeof(UIScrollView)) as UIScrollView;
			if (scrollView == null)
				return;

			if (!visible)
				RestoreScrollPosition(scrollView, ScrollViewInset);
			else
			{
				CenterViewInScroll(activeView, scrollView, keyboardHeight);
			}
		}

		protected virtual void CenterViewInScroll(UIView viewToCenter, UIScrollView scrollView, float keyboardHeight)
		{
			var contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardHeight, 0.0f);
			scrollView.ContentInset = contentInsets;
			scrollView.ScrollIndicatorInsets = contentInsets;

			// Position of the active field relative isnside the scroll view
			CGRect relativeFrame = viewToCenter.Superview.ConvertRectToView(viewToCenter.Frame, scrollView);

			bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
			var spaceAboveKeyboard = (landscape ? scrollView.Frame.Width : scrollView.Frame.Height) - keyboardHeight;

			// Move the active field to the center of the available space
			var offset = (relativeFrame.Y - spaceAboveKeyboard - viewToCenter.Frame.Height) / 2;
			scrollView.ContentOffset = new PointF(0, (float)offset);

		}

		protected virtual void RestoreScrollPosition(UIScrollView scrollView, float height = 0f)
		{
			scrollView.ContentInset = new UIEdgeInsets(height, 0.0f, 0.0f, 0.0f);
			scrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
			scrollView.ContentOffset = new PointF(0, (float)-height);
		}

		/// <summary>
		/// Call it to force dismiss keyboard when background is tapped
		/// </summary>
		protected void DismissKeyboardOnBackgroundTap()
		{
			// Add gesture recognizer to hide keyboard
			var tap = new UITapGestureRecognizer { CancelsTouchesInView = false };
			tap.AddTarget(() => View.EndEditing(true));
			View.AddGestureRecognizer(tap);
		}

	}
}
