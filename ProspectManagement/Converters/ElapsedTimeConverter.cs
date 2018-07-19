using System;
using System.Globalization;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
    public class ElapsedTimeConverter: MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enteredDate = DateTime.Parse(value.ToString());
            var utcNowDate = DateTime.UtcNow;
            var timespan = utcNowDate - enteredDate;

            if (timespan.Days > 0)
                return timespan.Days + (timespan.Days == 1 ? " day ago" : " days ago");
            else if (timespan.Hours > 0)
                return timespan.Hours + (timespan.Hours == 1 ? " hour ago" : " hours ago");
            else if (timespan.Minutes > 0)
                return timespan.Minutes + (timespan.Minutes == 1 ? " minute ago" : " minutes ago");
            else if (timespan.Seconds > 0)
                return timespan.Seconds + (timespan.Seconds == 1 ? " seconds ago" : " seconds ago");
            else
                return "just now";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; 
        }
    }
}
