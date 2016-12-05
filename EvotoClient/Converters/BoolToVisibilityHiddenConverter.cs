using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EvotoClient.Converters
{
    public class BoolToVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            if (value is bool) flag = (bool) value;
            return flag ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Unsupported");
        }
    }
}