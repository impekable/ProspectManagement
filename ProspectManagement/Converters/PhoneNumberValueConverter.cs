﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MvvmCross.Converters;

namespace ProspectManagement.Core.Converters
{
	public class PhoneNumberValueConverter : MvxValueConverter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var numbers = Regex.Replace(value.ToString(), @"\D", "");

			if (numbers.Length <= 3)
				return numbers;
			if (numbers.Length <= 7)
				return string.Format("{0}-{1}", numbers.Substring(0, 3), numbers.Substring(3));

			return string.Format("{0}-{1}-{2}", numbers.Substring(0, 3), numbers.Substring(3, 3), numbers.Substring(6));
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
            return value; //Regex.Replace(value.ToString(), @"\D", "");
		}
	}
}
