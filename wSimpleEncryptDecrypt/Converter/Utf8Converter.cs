using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace wSimpleEncryptDecrypt.Converter
{
	public class Utf8ConverterValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			try
			{
				if (value is string)
				{
					Utf8Converter.ConvertBack(value);
				}
				else
				{
					Utf8Converter.Convert(value);
				}

				return new ValidationResult(true, null);
			}
			catch
			{
				return new ValidationResult(false, "Please enter valid base 64!");
			}
		}
	}

	public class Utf8Converter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Convert(value);
		}

		public static object Convert(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (!(value is byte[]))
			{
				throw new InvalidOperationException("value has to be byte[]");
			}

			return Encoding.UTF8.GetString((byte[])value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ConvertBack(value);
		}

		public static object ConvertBack(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (!(value is string))
			{
				throw new InvalidOperationException("value has to be string");
			}

			return Encoding.UTF8.GetBytes((string)value);
		}
	}
}