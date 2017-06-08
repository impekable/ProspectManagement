using System;
using MvvmCross.Platform.Converters;

namespace ProspectManagement.Core.Converters
{
	public class ImageValueConverter : MvxValueConverter<int, string>
	{
		protected override string Convert(int value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == 0)
				return "res:unknown-person1.jpg";
			else
				return "res:timthumb.jpeg";
		}
	}
}
