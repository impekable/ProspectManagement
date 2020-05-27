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
    [Register ("SMSTaskView")]
    partial class SMSTaskView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView MessagesScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView MessagesTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView MessageTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SendButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MessagesScrollView != null) {
                MessagesScrollView.Dispose ();
                MessagesScrollView = null;
            }

            if (MessagesTableView != null) {
                MessagesTableView.Dispose ();
                MessagesTableView = null;
            }

            if (MessageTextView != null) {
                MessageTextView.Dispose ();
                MessageTextView = null;
            }

            if (SendButton != null) {
                SendButton.Dispose ();
                SendButton = null;
            }
        }
    }
}