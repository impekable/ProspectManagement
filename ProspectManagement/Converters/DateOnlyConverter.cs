using System;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
    public class DateOnlyConverter : MvxValueConverter<DateTime, String>
    {
        protected override String Convert(DateTime value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToShortDateString();
        }
    }
}

