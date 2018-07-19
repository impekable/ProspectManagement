using System;
using System.Globalization;
using MvvmCross.Converters;
using System.Linq;

namespace ProspectManagement.Core.Converters
{
    public class TitleCaseValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().ToCharArray().First().ToString().ToUpper() + value.ToString().Substring(1);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; //Regex.Replace(value.ToString(), @"\D", "");
        }
    }
}

