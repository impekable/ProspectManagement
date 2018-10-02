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
    [Register ("ActivityDetailView")]
    partial class ActivityDetailView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WebKit.WKWebView EmailWebView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView MainStackView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EmailWebView != null) {
                EmailWebView.Dispose ();
                EmailWebView = null;
            }

            if (MainStackView != null) {
                MainStackView.Dispose ();
                MainStackView = null;
            }
        }
    }
}