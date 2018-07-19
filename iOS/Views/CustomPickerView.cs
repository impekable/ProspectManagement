using System;
using System.Drawing;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public class CustomPickerView: MvxPickerViewModel
    {
		public CustomPickerView(UIPickerView pickerView) : base(pickerView)
		{

		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel(new RectangleF(0, 0, 2000, 60f));
			lbl.BackgroundColor = UIColor.White;
			lbl.TextColor = UIColor.Black;
			lbl.Font = UIFont.FromName("Raleway-Regular", 16);
			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Text = GetTitle(pickerView, row, component);
			return lbl;

		}
    }
}
