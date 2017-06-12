using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using ProspectManagement.Core.ViewModels;

namespace ProspectManagement.iOS.Views
{
    public partial class RootView : MvxViewController<RootViewModel>
    {
        public RootView (IntPtr handle) : base (handle)
        {
        }
    }
}