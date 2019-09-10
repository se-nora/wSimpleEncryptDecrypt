using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace wSimpleEncryptDecrypt.Converter
{
	public class ScrollbarVisibilityMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var viewWidth = (double)values[0];
			var minWidth = (double)values[1];
			var itemCount = (int)values[2];
			if (minWidth * itemCount < viewWidth)
			{
				return ScrollBarVisibility.Hidden;
			}
			return ScrollBarVisibility.Visible;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
