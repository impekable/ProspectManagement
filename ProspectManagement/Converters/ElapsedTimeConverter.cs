using System;
using System.Globalization;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
    public class ElapsedTimeConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enteredDate = DateTime.Parse(value.ToString());
            var utcNowDate = DateTime.UtcNow;
            var timespan = utcNowDate - enteredDate;

            if (timespan.TotalDays >= 365)
            {
                var years = System.Convert.ToInt16(timespan.TotalDays / 365);
                return years + (years == 1 ? " year ago" : " year ago");
            }
            else if (timespan.TotalDays >= 30)
            {
                var months = System.Convert.ToInt16(timespan.TotalDays / 30);
                return months + (months == 1 ? " month ago" : " months ago");
            }
            else if (timespan.TotalDays >= 7)
            {
                var weeks = System.Convert.ToInt16(timespan.TotalDays / 7);
                return weeks + (weeks == 1 ? " week ago" : " weeks ago");
            }
            else if (timespan.TotalDays > 1)
                return System.Convert.ToInt16(timespan.TotalDays) + (System.Convert.ToInt16(timespan.TotalDays) == 1 ? " day ago" : " days ago");
            else if (timespan.TotalHours > 1)
                return System.Convert.ToInt16(timespan.TotalHours) + (System.Convert.ToInt16(timespan.TotalHours) == 1 ? " hour ago" : " hours ago");
            else if (timespan.TotalMinutes > 1)
                return System.Convert.ToInt16(timespan.TotalMinutes) + (System.Convert.ToInt16(timespan.TotalMinutes) == 1 ? " minute ago" : " minutes ago");
            else if (timespan.TotalSeconds > 20)
                return System.Convert.ToInt16(timespan.TotalSeconds) + (System.Convert.ToInt16(timespan.TotalSeconds) == 1 ? " seconds ago" : " seconds ago");
            else
                return "just now";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
