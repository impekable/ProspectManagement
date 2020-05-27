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
    [Register ("EmailTaskView")]
    partial class EmailTaskView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WebKit.WKWebView EmailContentWebView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FromLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SendEmailButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SubjectLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ToLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EmailContentWebView != null) {
                EmailContentWebView.Dispose ();
                EmailContentWebView = null;
            }

            if (FromLabel != null) {
                FromLabel.Dispose ();
                FromLabel = null;
            }

            if (SendEmailButton != null) {
                SendEmailButton.Dispose ();
                SendEmailButton = null;
            }

            if (SubjectLabel != null) {
                SubjectLabel.Dispose ();
                SubjectLabel = null;
            }

            if (ToLabel != null) {
                ToLabel.Dispose ();
                ToLabel = null;
            }
        }
    }
}