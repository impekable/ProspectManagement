using System;
using MvvmCross.Platform.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
	public class LoggedInAsConverter : MvxValueConverter<User, string>
	{
		protected override string Convert(User value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || string.IsNullOrEmpty(value.UserId))
				return "Not Logged in. Please Login to reset community.";
			else
				return "Logged in As " + value.UserId;
		}
	}
}
