using System;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.Models;
using ProspectManagement.iOS.Services;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public partial class SMSInboxViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("SMSInboxViewCell");
        public static readonly UINib Nib;

        static SMSInboxViewCell()
        {
            Nib = UINib.FromName("SMSInboxViewCell", NSBundle.MainBundle);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);

            if (UnreadCountLabel.Text.Equals("0"))
                UnreadCountLabel.Hidden = true;
            else
            {
                UnreadCountLabel.Layer.BackgroundColor = UIColor.Red.CGColor;
                UnreadCountLabel.Layer.BorderColor = UIColor.Red.CGColor;
            }
        }

        protected SMSInboxViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic

            this.DelayBind(() =>
            {
                UnreadCountLabel.TextAlignment = UITextAlignment.Center;
                UnreadCountLabel.TextColor = UIColor.White;

                UnreadCountLabel.Layer.CornerRadius = UnreadCountLabel.Frame.Width / 2;
                UnreadCountLabel.Layer.BorderWidth = (nfloat)3.0;
                UnreadCountLabel.Layer.MasksToBounds = true;
                UnreadCountLabel.TranslatesAutoresizingMaskIntoConstraints = false;

                var e = new Environment_iOS();
                var theme = e.GetOperatingSystemTheme();

                var set = this.CreateBindingSet<SMSInboxViewCell, SmsActivity>();
                set.Bind(UnreadCountLabel).To(v => v.UnreadCount);
                set.Bind(NameLabel).To(v => v.Prospect.Name);
                set.Bind(DateLabel).To(v => v.UpdatedDate).WithConversion(new ElapsedTimeConverter());
                set.Bind(CommunityLabel).To(v => v.Prospect.ProspectCommunity.Community.Description);
                set.Bind(MessageLabel).To(v => v.MessageBody);
                //set.Bind(this).For(u => u.BackgroundColor).To(v => v).WithConversion("QuestionBackground", theme);
                set.Apply();
            });
        }
    }
}
