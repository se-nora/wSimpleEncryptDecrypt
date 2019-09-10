using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace wSimpleEncryptDecrypt.Converter
{
	public class Base64ConverterValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			try
			{
				if (value is string)
				{
					Base64Converter.ConvertBack(value);
				}
				else
				{
					Base64Converter.Convert(value);
				}

				return new ValidationResult(true, null);
			}
			catch
			{
				return new ValidationResult(false, "Please enter valid base 64!");
			}
		}
	}

	public class Base64Converter : IValueConverter
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

			return System.Convert.ToBase64String((byte[])value);
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

			return System.Convert.FromBase64String((string) value);
		}
	}
}