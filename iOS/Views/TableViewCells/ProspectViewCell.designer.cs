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
    [Register ("ProspectViewCell")]
    partial class ProspectViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CommunityLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DayCountLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SystemActivityLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ProspectLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView SalespersonImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SalespersonLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView TopStackView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CommunityLabel != null) {
                CommunityLabel.Dispose ();
                CommunityLabel = null;
            }

            if (DayCountLabel != null) {
                DayCountLabel.Dispose ();
                DayCountLabel = null;
            }

            if (SystemActivityLabel != null) {
                SystemActivityLabel.Dispose ();
                SystemActivityLabel = null;
            }

            if (ProspectLabel != null) {
                ProspectLabel.Dispose ();
                ProspectLabel = null;
            }

            if (SalespersonImage != null) {
                SalespersonImage.Dispose ();
                SalespersonImage = null;
            }

            if (SalespersonLabel != null) {
                SalespersonLabel.Dispose ();
                SalespersonLabel = null;
            }

            if (TopStackView != null) {
                TopStackView.Dispose ();
                TopStackView = null;
            }
        }
    }
}