using System;

using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using ProspectManagement.Core.Models;
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
				var set = this.CreateBindingSet<QuestionViewCell, TrafficCardResponse>();
				set.Bind(QuestionLabel).To(v => v.TrafficCardQuestion.QuestionText);
				set.Bind(AnswerLabel).To(v => v.AnswerText);
				
				set.Apply();
			});
		}
	}
}
