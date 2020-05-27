using System;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
    public class DayCountConverter : MvxValueConverter<DateTime, String>
    {
        protected override String Convert(DateTime value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var t =  (DateTime.UtcNow - value).Days;

            return  "Day " + t;
        }
    }
}
