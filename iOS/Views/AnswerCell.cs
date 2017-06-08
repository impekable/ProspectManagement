using System;

using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace ProspectManagement.iOS
{
    public partial class AnswerCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("AnswerCell");
        public static readonly UINib Nib;

        static AnswerCell()
        {
            Nib = UINib.FromName("AnswerCell", NSBundle.MainBundle);
        }

        protected AnswerCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AnswerCell, string>();
                set.Bind(AnswerLabel).To(v => v);
                set.Apply();
            });
        }
    }
}
