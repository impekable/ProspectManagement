using System;
using MvvmCross.Platform.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
	public class TrafficSourceDetailConverter : MvxValueConverter<TrafficSource, string>
	{
		protected override string Convert(TrafficSource value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || string.IsNullOrEmpty(value.SourceDescription))
				return "";
			else if (value.SourceDescription.Equals("Internet"))
				return "Website Name";
			else if (value.SourceDescription.Equals("Other"))
				return "Specify Other";
			else
				return value + " Name";
		}
	}
}
