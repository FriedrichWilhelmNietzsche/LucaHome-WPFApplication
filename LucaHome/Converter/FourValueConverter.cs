using System;
using System.Globalization;
using System.Windows.Data;

namespace LucaHome.Converter
{
    public class FourValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string stringOne = values[0] as string;
            string stringTwo = values[1] as string;
            string stringThree = values[2] as string;
            string stringFour = values[3] as string;

            if (!string.IsNullOrEmpty(stringOne) && !string.IsNullOrEmpty(stringTwo) && !string.IsNullOrEmpty(stringThree) && !string.IsNullOrEmpty(stringFour))
            {
                return true;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
