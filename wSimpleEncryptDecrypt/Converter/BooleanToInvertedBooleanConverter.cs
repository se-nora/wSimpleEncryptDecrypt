using System;
using System.Globalization;
using System.Windows.Data;

namespace wSimpleEncryptDecrypt.Converter
{
    public class BooleanToInvertedBooleanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new InvalidOperationException("Value must be bool");
            }

            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new InvalidOperationException("Value must be bool");
            }

            return !(bool)value;
        }
    }
}