using System;
using System.Globalization;
using System.Windows.Data;
using PicEditor.Model;

namespace PicEditor.Converter
{
    internal class EditModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditMode.Modes mode && parameter is string param)
            {
                return ((int)mode).ToString() == param;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b && parameter is string param)
            {
                if(int.TryParse(param, out int number))
                {
                    return (EditMode.Modes)number;
                }
            }
            return value;
        }
    }
}
