using System;
using MvvmCross.Converters;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
    public class NameConverter : MvxValueConverter<Prospect, string>
    {
        protected override string Convert(Prospect value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value.NamePrefix + " " + value.FirstName + " " + value.MiddleName + " " + value.LastName + " " + value.NameSuffix).Trim();
        }
    }
}
