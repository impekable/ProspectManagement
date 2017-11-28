using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Utility;

namespace ProspectManagement.iOS.Views
{
	[MvxFromStoryboard("Main")]
	[MvxRootPresentation]
	public partial class SplitRootView : MvxSplitViewController<SplitRootViewModel>
	{
		private bool _isPresentedFirstTime = true;

		public SplitRootView(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PreferredPrimaryColumnWidthFraction = .3f;
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