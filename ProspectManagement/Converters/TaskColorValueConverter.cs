using System;
using System.Drawing;
using System.Globalization;
using MvvmCross.Plugin.Color;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Models.App;

namespace ProspectManagement.Core.Converters
{
    public class TaskColorValueConverter: MvxColorValueConverter<Activity>
    {
        protected override Color Convert(Activity value, object parameter, CultureInfo culture)
        {
            if (value.TimeDateEnd.Value.CompareTo(DateTime.Now) < 0)
                return Color.Red;
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
