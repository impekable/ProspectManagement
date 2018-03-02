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
    [Register ("AddActivityView")]
    partial class AddActivityView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NoteErrorLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView NoteTextView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (NoteErrorLabel != null) {
                NoteErrorLabel.Dispose ();
                NoteErrorLabel = null;
            }

            if (NoteTextView != null) {
                NoteTextView.Dispose ();
                NoteTextView = null;
            }
        }
    }
}