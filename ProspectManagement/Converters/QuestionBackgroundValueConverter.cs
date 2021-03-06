﻿using System;
using System.Globalization;
using MvvmCross.Converters;
using MvvmCross.UI;
using MvvmCross.Plugin.Color;
using ProspectManagement.Core.Models;
using System.Drawing;
using ProspectManagement.Core.Models.App;

namespace ProspectManagement.Core.Converters
{
    public class QuestionBackgroundValueConverter: MvxColorValueConverter<TrafficCardResponse>
    {
        protected override Color Convert(TrafficCardResponse value, object parameter, CultureInfo culture)
        {
            if ((Theme)parameter == Theme.Light)
            {
                if(value.AnswerNumber == 0 && value.TrafficCardQuestion.WeightingValue > 0)
                    return Color.Gray;
            else
                    return Color.White;
            }
            else
            {
                if (value.AnswerNumber == 0 && value.TrafficCardQuestion.WeightingValue > 0)
                    return Color.Gray;
                else
                    return Color.Black;
            }

            
        }
    }
}
