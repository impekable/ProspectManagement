using System;
using MvvmCross.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
    public class FollowUpSettingsValueConverter: MvxValueConverter<FollowUpSettings, string>
    {
		protected override string Convert(FollowUpSettings value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
            var settingsString = ""; //value.ExcludeFromFollowup ? "Excluded From Follow Up" : "Consented To";

			if (value.ConsentToText)
				settingsString += "Text, ";
			if (value.ConsentToPhone)
				settingsString += "Phone, ";
			if (value.ConsentToEmail)
				settingsString += "Email, ";
			if (value.ConsentToMail)
				settingsString += "Mail, ";
			
			return settingsString.TrimEnd(',',' ');
		}
    }
}
