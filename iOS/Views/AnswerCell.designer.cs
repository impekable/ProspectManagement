// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ProspectRegistration.iOS
{
    [Register ("AnswerCell")]
    partial class AnswerCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AnswerLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AnswerLabel != null) {
                AnswerLabel.Dispose ();
                AnswerLabel = null;
            }
        }
    }
}