// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [Register ("RootView")]
    partial class RootView
    {
        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UILabel AttemptingLoginLabel { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView LoginActivityIndicator { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UIButton LoginButton { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (AttemptingLoginLabel != null)
            {
                AttemptingLoginLabel.Dispose();
                AttemptingLoginLabel = null;
            }

            if (LoginActivityIndicator != null)
            {
                LoginActivityIndicator.Dispose();
                LoginActivityIndicator = null;
            }

            if (LoginButton != null)
            {
                LoginButton.Dispose();
                LoginButton = null;
            }
        }
    }
}