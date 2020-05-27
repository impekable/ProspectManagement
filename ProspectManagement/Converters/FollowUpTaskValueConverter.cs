using System;
using MvvmCross.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
    public class FollowUpTaskValueConverter : MvxValueConverter<Activity, string>
    {

        protected override string Convert(Activity value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value.Subject + " - " + value.Prospect.FirstName + " " + value.Prospect.LastName).Trim();
        }
    }

}
