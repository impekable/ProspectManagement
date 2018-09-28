using System;
using Foundation;
using UIKit;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Models;
using MvvmCross.Platforms.Ios.Binding.Views;

using ProspectManagement.Core.Converters;

namespace ProspectManagement.iOS.Views
{
    public partial class ActivitiesViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("ActivitiesViewCell");
        public static readonly UINib Nib;

        static ActivitiesViewCell()
        {
            Nib = UINib.FromName("ActivitiesViewCell", NSBundle.MainBundle);
        }

        protected ActivitiesViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ActivitiesViewCell, Activity>();
                set.Bind(DescriptionLabel).To(v => v).WithConversion(new ActivityLabelValueConverter());
                set.Bind(DateTimeLabel).To(v => v.DateCompleted).WithConversion(new DateOnlyConverter());
                set.Apply();
            });
        }
    }
}
