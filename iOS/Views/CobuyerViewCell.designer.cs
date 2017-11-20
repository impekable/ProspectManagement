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
    [Register ("CobuyerViewCell")]
    partial class CobuyerViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView CobuyerImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CobuyerLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CobuyerImage != null) {
                CobuyerImage.Dispose ();
                CobuyerImage = null;
            }

            if (CobuyerLabel != null) {
                CobuyerLabel.Dispose ();
                CobuyerLabel = null;
            }
        }
    }
}