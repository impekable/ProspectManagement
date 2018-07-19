using System;
using MvvmCross.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
	public class DefaultCommunityConverter: MvxValueConverter<Community, string>
	{
		protected override string Convert(Community value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || string.IsNullOrEmpty(value.Description))
				return "Community not set.";
			else
				return value.Description;
		}
	}
}
