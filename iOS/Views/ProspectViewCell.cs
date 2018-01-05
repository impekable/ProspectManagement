using System;
using Foundation;
using UIKit;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Converters;

namespace ProspectManagement.iOS.Views
{
    public partial class ProspectViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("ProspectViewCell");
        public static readonly UINib Nib;
        private readonly MvxImageViewLoader imageViewLoader;

        static ProspectViewCell()
        {
            Nib = UINib.FromName("ProspectViewCell", NSBundle.MainBundle);
        }

        protected ProspectViewCell(IntPtr handle) : base(handle)
        {
            imageViewLoader = new MvxImageViewLoader(() => SalespersonImage);

            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ProspectViewCell, Prospect>();
                set.Bind(ProspectLabel).To(v => v.Name);
                set.Bind(CommunityLabel).To(v => v.ProspectCommunity.Community.Description);
                set.Bind(EnteredDateLabel).To(v => v.ProspectCommunity.EnteredDate).WithConversion(new ElapsedTimeConverter());;
				//set.Bind(SalespersonLabel).To(v => v.ProspectCommunity.SalespersonAddressNumber);
				
                set.Bind(imageViewLoader).For(i => i.DefaultImagePath).To(v => v.ProspectCommunity.SalespersonAddressNumber).WithConversion(new ImageValueConverter());
				set.Apply();
            });
        }
    }
}
