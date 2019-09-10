using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Data;

namespace wSimpleEncryptDecrypt.Converter
{
	/*public class EncryptionConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var cryptoTransform = values.OfType<ICryptoTransform>().FirstOrDefault();
			var rsa = values.OfType<RSACryptoServiceProvider>().FirstOrDefault();
			if (cryptoTransform == null && rsa == null)
			{
				throw new InvalidOperationException("No encryption algorythm bound");
			}
			if (cryptoTransform != null && rsa != null)
			{
				throw new InvalidOperationException("You can only bind one encryption algorithm");
			}
			

			var valueToEncrypt = values.OfType<byte[]>().SingleOrDefault();

			if (valueToEncrypt == null)
			{
				return null;
			}

			if (cryptoTransform != null)
			{
				using (var cryptoStream = new CryptoStream(new MemoryStream(valueToEncrypt), cryptoTransform, CryptoStreamMode.Read))
				{
					using (var ms = new MemoryStream())
					{
						cryptoStream.CopyTo(ms);
						return ms.ToArray();
					}
				}
			}
			if (rsa != null)
			{
				return rsa.Encrypt(valueToEncrypt);
			}

			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}*/
}
