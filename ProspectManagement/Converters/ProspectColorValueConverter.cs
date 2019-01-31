using System;
using System.Globalization;
using MvvmCross.Plugin.Color;
using MvvmCross.UI;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
    public class ProspectColorValueConverter : MvxColorValueConverter<Prospect>
    {
        protected override MvxColor Convert(Prospect value, object parameter, CultureInfo culture)
        {
            if (value.FollowUpSettings.ExcludeFromFollowup)
                return MvxColors.Red;
            else
                return MvxColors.Black;
        }
    }
}
