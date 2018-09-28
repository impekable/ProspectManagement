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
        UIKit.UILabel ContactMethodLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ContactMethodTextLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ContactTypeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ContactTypeTextLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WebKit.WKWebView EmailWebView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView FirstRowStackView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView MainStackView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView SecondRowStackView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ContactMethodLabel != null) {
                ContactMethodLabel.Dispose ();
                ContactMethodLabel = null;
            }

            if (ContactMethodTextLabel != null) {
                ContactMethodTextLabel.Dispose ();
                ContactMethodTextLabel = null;
            }

            if (ContactTypeLabel != null) {
                ContactTypeLabel.Dispose ();
                ContactTypeLabel = null;
            }

            if (ContactTypeTextLabel != null) {
                ContactTypeTextLabel.Dispose ();
                ContactTypeTextLabel = null;
            }

            if (EmailWebView != null) {
                EmailWebView.Dispose ();
                EmailWebView = null;
            }

            if (FirstRowStackView != null) {
                FirstRowStackView.Dispose ();
                FirstRowStackView = null;
            }

            if (MainStackView != null) {
                MainStackView.Dispose ();
                MainStackView = null;
            }

            if (SecondRowStackView != null) {
                SecondRowStackView.Dispose ();
                SecondRowStackView = null;
            }
        }
    }
}