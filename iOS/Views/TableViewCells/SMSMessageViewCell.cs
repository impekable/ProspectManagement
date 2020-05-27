using System;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.Models;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public partial class SMSMessageViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("SMSMessageViewCell");
        public static readonly UINib Nib;

        static SMSMessageViewCell()
        {
            Nib = UINib.FromName("SMSMessageViewCell", NSBundle.MainBundle);
        }

        protected SMSMessageViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<SMSMessageViewCell, SmsActivity>();
                set.Bind(MessageLabel).To(v => v.MessageBody);
                set.Bind(DateLabel).To(v => v.UpdatedDate).WithConversion(new DateTimeConverter());
                set.Apply();
            });
        }
    }
}
