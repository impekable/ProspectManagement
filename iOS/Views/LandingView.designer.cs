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
    [Register ("LandingView")]
    partial class LandingView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LogoutButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UnreadSMSLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ViewDailyToDoButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ViewProspectsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ViewSMSInboxButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LogoutButton != null) {
                LogoutButton.Dispose ();
                LogoutButton = null;
            }

            if (NameLabel != null) {
                NameLabel.Dispose ();
                NameLabel = null;
            }

            if (UnreadSMSLabel != null) {
                UnreadSMSLabel.Dispose ();
                UnreadSMSLabel = null;
            }

            if (ViewDailyToDoButton != null) {
                ViewDailyToDoButton.Dispose ();
                ViewDailyToDoButton = null;
            }

            if (ViewProspectsButton != null) {
                ViewProspectsButton.Dispose ();
                ViewProspectsButton = null;
            }

            if (ViewSMSInboxButton != null) {
                ViewSMSInboxButton.Dispose ();
                ViewSMSInboxButton = null;
            }
        }
    }
}