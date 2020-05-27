using System;
using Foundation;
using UIKit;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Converters;
using MvvmCross.Platforms.Ios.Binding.Views;
using FFImageLoading.Cross;
using FFImageLoading;
using Cirrious.FluentLayouts.Touch;
using ProspectManagement.iOS.Services;

namespace ProspectManagement.iOS.Views
{
    public partial class ProspectViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("ProspectViewCell");
        public static readonly UINib Nib;
		private MvxCachedImageView _imageControl;
		private bool _constraintsCreated;

        static ProspectViewCell()
        {
            Nib = UINib.FromName("ProspectViewCell", NSBundle.MainBundle);
        }
        
        protected ProspectViewCell(IntPtr handle) : base(handle)
        {
			_imageControl = new MvxCachedImageView();
            
			ContentView.AddSubview(_imageControl);
			ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

			SetNeedsUpdateConstraints();

            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
			{
                var e = new Environment_iOS();
                var theme = e.GetOperatingSystemTheme();

                var set = this.CreateBindingSet<ProspectViewCell, Prospect>();
                set.Bind(ProspectLabel).To(v => v.Name);
                set.Bind(ProspectLabel).For(v=> v.TextColor).To(v => v).WithConversion("ProspectColor", theme);
                set.Bind(CommunityLabel).To(v => v.ProspectCommunity.Community.Description);
                set.Bind(SalespersonLabel).To(v => v.ProspectCommunity.SalespersonName);
				set.Bind(FirstVisitLabel).To(v => v.ProspectCommunity.StartDate).WithConversion(new DateOnlyConverter());
				set.Bind(LastVisitLabel).To(v => v.ProspectCommunity.EndDate).WithConversion(new DateOnlyConverter());
                set.Bind(DayCountLabel).To(v => v.ProspectCommunity.StartDate).WithConversion(new DayCountConverter());

                //set.Bind(this).For(u => u.BackgroundColor).To(v => v).WithConversion(new ProspectBackgroundColorValueConverter());

                set.Bind(_imageControl).For(i => i.ImagePath).To(v => v.ProspectCommunity.SalespersonAddressNumber).WithConversion(new ImageValueConverter());
				set.Apply();
            });
        }

        public override void UpdateConstraints()
		{
			if (!_constraintsCreated)
			{
				ContentView.AddConstraints(
					_imageControl.WithSameLeft(SalespersonImage),
					_imageControl.WithSameTop(SalespersonImage),
					_imageControl.WithSameWidth(SalespersonImage),
					_imageControl.WithSameHeight(SalespersonImage)
				);
				_constraintsCreated = true;
			}
			base.UpdateConstraints();
		}
    }
}
