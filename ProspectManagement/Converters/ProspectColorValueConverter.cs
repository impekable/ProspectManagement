using System;
using System.Drawing;
using System.Globalization;
using MvvmCross.Plugin.Color;
using MvvmCross.UI;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Models.App;

namespace ProspectManagement.Core.Converters
{
    public class ProspectColorValueConverter : MvxColorValueConverter<Prospect>
    {
        protected override Color Convert(Prospect value, object parameter, CultureInfo culture)
        {
            if (value.FollowUpSettings.ExcludeFromFollowup)
                return Color.Red;
            else if (value.Status.Equals("Inactive"))
                return Color.Gray;
            else
            {
                if ((Theme)parameter == Theme.Light)
                    return Color.Black;
                else
                    return Color.White;
            }
        }
    }
}
