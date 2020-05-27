// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ProspectManagement.iOS.Views
{
    [Register ("FollowUpViewCell")]
    partial class FollowUpViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CallButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CategoryLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CommunityLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DayCountLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EmailButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ProspectLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SMSButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TaskDueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TaskLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CallButton != null) {
                CallButton.Dispose ();
                CallButton = null;
            }

            if (CategoryLabel != null) {
                CategoryLabel.Dispose ();
                CategoryLabel = null;
            }

            if (CommunityLabel != null) {
                CommunityLabel.Dispose ();
                CommunityLabel = null;
            }

            if (DayCountLabel != null) {
                DayCountLabel.Dispose ();
                DayCountLabel = null;
            }

            if (EmailButton != null) {
                EmailButton.Dispose ();
                EmailButton = null;
            }

            if (ProspectLabel != null) {
                ProspectLabel.Dispose ();
                ProspectLabel = null;
            }

            if (SMSButton != null) {
                SMSButton.Dispose ();
                SMSButton = null;
            }

            if (TaskDueLabel != null) {
                TaskDueLabel.Dispose ();
                TaskDueLabel = null;
            }

            if (TaskLabel != null) {
                TaskLabel.Dispose ();
                TaskLabel = null;
            }
        }
    }
}