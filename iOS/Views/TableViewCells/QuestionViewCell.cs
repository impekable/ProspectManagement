using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.Models;
using ProspectManagement.iOS.Services;
using UIKit;

namespace ProspectManagement.iOS.Views
{
	public partial class QuestionViewCell : MvxTableViewCell
	{
		public static readonly NSString Key = new NSString("QuestionViewCell");
		public static readonly UINib Nib;

		static QuestionViewCell()
		{
			Nib = UINib.FromName("QuestionViewCell", NSBundle.MainBundle);
		}

		protected QuestionViewCell(IntPtr handle) : base(handle)
        {
			// Note: this .ctor should not contain any initialization logic.
			this.DelayBind(() =>
			{
                var e = new Environment_iOS();
                var theme = e.GetOperatingSystemTheme();

                var set = this.CreateBindingSet<QuestionViewCell, TrafficCardResponse>();
				set.Bind(QuestionLabel).To(v => v.TrafficCardQuestion.QuestionText);
				set.Bind(AnswerLabel).To(v => v.AnswerText);
                set.Bind(this).For(u => u.BackgroundColor).To(v => v).WithConversion("QuestionBackground", theme);
                set.Bind(QuestionLabel).For(u => u.TextColor).To(v => v).WithConversion("QuestionTextColor", theme);
                set.Bind(AnswerLabel).For(u => u.TextColor).To(v => v).WithConversion(new QuestionTextColorValueConverter());
				set.Apply();
			});
		}
	}
}
