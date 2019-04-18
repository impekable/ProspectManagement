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
    [Register ("ActivitiesView")]
    partial class ActivitiesView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ActivitiesTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AddNoteButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AddVisitButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CompleteApptButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBar ProspectTabBar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ActivitiesTableView != null) {
                ActivitiesTableView.Dispose ();
                ActivitiesTableView = null;
            }

            if (AddNoteButton != null) {
                AddNoteButton.Dispose ();
                AddNoteButton = null;
            }

            if (AddVisitButton != null) {
                AddVisitButton.Dispose ();
                AddVisitButton = null;
            }

            if (CompleteApptButton != null) {
                CompleteApptButton.Dispose ();
                CompleteApptButton = null;
            }

            if (ProspectTabBar != null) {
                ProspectTabBar.Dispose ();
                ProspectTabBar = null;
            }
        }
    }
}