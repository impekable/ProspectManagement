using System;
using Foundation;
using UIKit;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Models;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace ProspectManagement.iOS.Views
{
    public partial class CobuyerViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("CobuyerViewCell");
        public static readonly UINib Nib;

        static CobuyerViewCell()
        {
            Nib = UINib.FromName("CobuyerViewCell", NSBundle.MainBundle);
        }

        protected CobuyerViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
	        {
		        var set = this.CreateBindingSet<CobuyerViewCell, Cobuyer>();
                set.Bind(CobuyerLabel).To(v => v.FullName);
				set.Apply();
	        });
        }
    }
}




