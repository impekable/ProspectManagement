using System;
using MvvmCross.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
    public class DayCountConverter : MvxValueConverter<ProspectCommunity, String>
    {
        protected override String Convert(ProspectCommunity value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var t = value.AgingStartDate == DateTime.MinValue ? "" : "Day " + (DateTime.UtcNow - value.AgingStartDate).Days;

            return t;
        }
    }
}
