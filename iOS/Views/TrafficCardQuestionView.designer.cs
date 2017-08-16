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
    [Register ("TrafficCardQuestionView")]
    partial class TrafficCardQuestionView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView AnswersTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SaveButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TrafficCardQuestionLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AnswersTableView != null) {
                AnswersTableView.Dispose ();
                AnswersTableView = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }

            if (TrafficCardQuestionLabel != null) {
                TrafficCardQuestionLabel.Dispose ();
                TrafficCardQuestionLabel = null;
            }
        }
    }
}