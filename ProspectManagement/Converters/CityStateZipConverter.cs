using System;
using MvvmCross.Platform.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
	public class CityStateZipConverter : MvxValueConverter<StreetAddress, string>
	{
		protected override string Convert(StreetAddress value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
            return value.City + (String.IsNullOrEmpty(value.City) ? "" : ", ") + value.State + " " + value.PostalCode;
		}
	}
}
