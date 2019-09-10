using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace wSimpleEncryptDecrypt.Converter
{
    public class NullableDateTimeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }

            if (value is DateTime)
            {
                return false;
            }
            
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
