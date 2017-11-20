using System;
using Foundation;
using UIKit;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.ViewModels;

namespace ProspectManagement.iOS.Views
{
    public partial class CobuyerViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("CobuyerViewCell");
        public static readonly UINib Nib;
        private readonly MvxImageViewLoader imageViewLoader;

        static CobuyerViewCell()
        {
            Nib = UINib.FromName("CobuyerViewCell", NSBundle.MainBundle);
        }

        protected CobuyerViewCell(IntPtr handle) : base(handle)
        {
            //imageViewLoader = new MvxImageViewLoader(() => CobuyerImage);
			this.DelayBind(() =>
	        {
		        var set = this.CreateBindingSet<CobuyerViewCell, CobuyerDetailViewModel>();
                set.Bind(CobuyerLabel).To(v => v.FullName);
				//set.Bind(imageViewLoader).For(i => i.DefaultImagePath);
				set.Apply();
	        });
        }
    }
}




