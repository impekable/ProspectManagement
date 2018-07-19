using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace ProspectManagement.iOS.Views
{
	[MvxRootPresentation]
    public partial class RootView : MvxViewController<RootViewModel>
    {
        public RootView (IntPtr handle) : base (handle)
        {
        }
    }
}