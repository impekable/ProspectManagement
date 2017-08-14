using System;
using MvvmCross.Platform.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
	public class StateCountryConverter : MvxValueConverter<UserDefinedCode, string>
	{
		protected override string Convert(UserDefinedCode value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || string.IsNullOrEmpty(value.Code) || value.Code.Equals("US"))
				return "State/Country";
			else
				return "Country";
		}
	}
}
