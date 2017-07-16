using System;
using System.Globalization;
using System.Windows.Data;

namespace LucaHome.Converter
{
    public class TwoValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string stringOne = values[0] as string;
            string stringTwo = values[1] as string;

            if (!string.IsNullOrEmpty(stringOne) && !string.IsNullOrEmpty(stringTwo))
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
