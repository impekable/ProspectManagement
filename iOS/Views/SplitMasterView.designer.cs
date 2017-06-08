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
    [Register ("SplitMasterView")]
    partial class SplitMasterView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchBar FilterSearchBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl FilterSegmentControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView MasterTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FilterSearchBar != null) {
                FilterSearchBar.Dispose ();
                FilterSearchBar = null;
            }

            if (FilterSegmentControl != null) {
                FilterSegmentControl.Dispose ();
                FilterSegmentControl = null;
            }

            if (MasterTableView != null) {
                MasterTableView.Dispose ();
                MasterTableView = null;
            }
        }
    }
}