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
    [Register ("TrafficCardView")]
    partial class TrafficCardView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBar ProspectTabBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView QuestionsStackView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView QuestionsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProspectTabBar != null) {
                ProspectTabBar.Dispose ();
                ProspectTabBar = null;
            }

            if (QuestionsStackView != null) {
                QuestionsStackView.Dispose ();
                QuestionsStackView = null;
            }

            if (QuestionsTableView != null) {
                QuestionsTableView.Dispose ();
                QuestionsTableView = null;
            }
        }
    }
}