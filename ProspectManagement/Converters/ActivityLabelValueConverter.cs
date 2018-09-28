using System;
using MvvmCross.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
    
        public class ActivityLabelValueConverter : MvxValueConverter<Activity, string>
        {

            protected override string Convert(Activity value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
            
            return (value.ContactMethod + " " + value.FollowUpMethod + " " + value.Subject).Trim();
          
            }   
        }

}
