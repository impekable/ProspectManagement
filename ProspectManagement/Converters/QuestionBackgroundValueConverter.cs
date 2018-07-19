using System;
using System.Globalization;
using MvvmCross.Converters;
using MvvmCross.UI;
using MvvmCross.Plugin.Color;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Converters
{
    public class QuestionBackgroundValueConverter: MvxColorValueConverter<TrafficCardResponse>
    {
        protected override MvxColor Convert(TrafficCardResponse value, object parameter, CultureInfo culture)
        {
            if (value.AnswerNumber == 0 && value.TrafficCardQuestion.WeightingValue > 0)
                return MvxColors.Gray;
            else
                return MvxColors.White;
        }
    }
}
