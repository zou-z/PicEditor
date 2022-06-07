using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PicEditor.Converter
{
    internal class LeftToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double left)
            {
                return new Thickness(0, 0, left, 0);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
