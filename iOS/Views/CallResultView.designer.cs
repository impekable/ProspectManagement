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
    [Register ("CallResultView")]
    partial class CallResultView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CallResultErrorLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CallResultLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField CallResultTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView NoteTextView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CallResultErrorLabel != null) {
                CallResultErrorLabel.Dispose ();
                CallResultErrorLabel = null;
            }

            if (CallResultLabel != null) {
                CallResultLabel.Dispose ();
                CallResultLabel = null;
            }

            if (CallResultTextField != null) {
                CallResultTextField.Dispose ();
                CallResultTextField = null;
            }

            if (NoteTextView != null) {
                NoteTextView.Dispose ();
                NoteTextView = null;
            }
        }
    }
}