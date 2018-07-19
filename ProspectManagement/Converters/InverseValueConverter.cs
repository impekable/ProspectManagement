using System;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
	public class InverseValueConverter: MvxValueConverter<bool, bool>
	{
		protected override bool Convert(bool value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !value;
		}
	}
}
