using System;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
    public class UnreadSMSMessageCountConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return $"You have {value} unread messages";
        }
    }
}
