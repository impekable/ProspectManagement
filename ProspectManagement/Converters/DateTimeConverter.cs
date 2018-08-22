using System;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
	public class DateTimeConverter: MvxValueConverter<DateTime, String>
    {
		protected override String Convert(DateTime value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			    var t = TimeZoneInfo.Local;

                return value.ToLocalTime().ToString() + " " + t.StandardName;
        }
    }
}
