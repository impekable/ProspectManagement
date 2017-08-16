using System;

using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public partial class AnswerViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("AnswerViewCell");
        public static readonly UINib Nib;

        static AnswerViewCell()
        {
            Nib = UINib.FromName("AnswerViewCell", NSBundle.MainBundle);
        }

        protected AnswerViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AnswerViewCell, string>();
                set.Bind(AnswerLabel).To(v => v);

                set.Apply();
            });
        }
    }
}