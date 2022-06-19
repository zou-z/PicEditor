using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PicEditor.Converter
{
    internal class MultiBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null)
            {
                foreach (object value in values)
                {
                    if (value is bool b)
                    {
                        if (!b)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                return true;
            }
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
