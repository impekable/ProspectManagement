using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxRootPresentation]
    public partial class SmsInboxSplitRootView : MvxSplitViewController<SmsInboxSplitRootViewModel>
    {
		private bool _isPresentedFirstTime = true;

		public SmsInboxSplitRootView (IntPtr handle) : base (handle)
        {
        }
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PreferredPrimaryColumnWidthFraction = .35f;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (ViewModel != null && _isPresentedFirstTime)
			{
				_isPresentedFirstTime = false;
				ViewModel.ShowInitialViewModelsCommand.Execute(null);

				//PreferredDisplayMode = UISplitViewControllerDisplayMode.PrimaryHidden;
			}

			UIBarButtonItem.Appearance.SetTitleTextAttributes(new UITextAttributes()
			{
				Font = UIFont.FromName("Raleway-Bold", 18),
				TextColor = ProspectManagementColors.DarkColor
			}, UIControlState.Normal);
		}
	}
}