using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using wSimpleEncryptDecrypt.Annotations;

namespace wSimpleEncryptDecrypt.Model
{
	public class CertificateModel : INotifyPropertyChanged
	{
		#region fields

		private readonly X509Certificate2 _certificate;
		private AesEncryptor _aesEncryptor;
		private AesEncrypedValue _aesEncryptedValue;
		private CertificateEncrypedValue _certificateEncrypedValue;
		private CertificateEncrypedValue _certificateEncryptedIv;
		private CertificateEncrypedValue _certificateEncryptedKey;

		#endregion fields

		#region ctor

		public CertificateModel(X509Certificate2 certificate)
		{
			_certificate = certificate;
		}

		#endregion ctor

		public X509Certificate2 Certificate => _certificate;

		[PublicAPI]
		public AesEncryptor AesEncryptor
		{
			get => _aesEncryptor ?? (AesEncryptor = new AesEncryptor());
		    set
			{
				if (Equals(value, _aesEncryptor)) return;
				if (_aesEncryptor != null)
				{
					_aesEncryptor.PropertyChanged -= AesEncryptorPropertyChanged;
				}
				_aesEncryptor = value;
				if (_aesEncryptor != null)
				{
					_aesEncryptor.PropertyChanged += AesEncryptorPropertyChanged;
				}
				OnPropertyChanged();
			}
		}

		private void AesEncryptorPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AesEncryptor.Key))
			{
				CertificateEncryptedKey.DecryptedValue = AesEncryptor.Key;
			}
			else if (e.PropertyName == nameof(AesEncryptor.Iv))
			{
				CertificateEncryptedIv.DecryptedValue = AesEncryptor.Iv;
			}
		}

		[PublicAPI]
		public AesEncrypedValue AesEncryptedValue
		{
			get => _aesEncryptedValue ?? (_aesEncryptedValue = new AesEncrypedValue(AesEncryptor));
		    set
			{
				if (Equals(value, _aesEncryptedValue)) return;
				_aesEncryptedValue = value;
				OnPropertyChanged();
			}
		}

		[PublicAPI]
		public CertificateEncrypedValue CertificateEncrypedValue
		{
			get => _certificateEncrypedValue ?? (CertificateEncrypedValue = new CertificateEncrypedValue(_certificate));
		    set
			{
				if (Equals(value, _certificateEncrypedValue)) return;
				_certificateEncrypedValue = value;
				OnPropertyChanged();
			}
		}

		[PublicAPI]
		public CertificateEncrypedValue CertificateEncryptedKey
		{
			get => _certificateEncryptedKey ?? (CertificateEncryptedKey = new CertificateEncrypedValue(_certificate));
		    set
			{
				if (Equals(value, _certificateEncryptedKey)) return;

				if (_certificateEncryptedKey != null)
				{
					_certificateEncryptedKey.PropertyChanged -= CertificateEncryptedKeyPropertyChanged;
				}
				_certificateEncryptedKey = value;
				if (_certificateEncryptedKey != null)
				{
					_certificateEncryptedKey.PropertyChanged += CertificateEncryptedKeyPropertyChanged;
				}
				OnPropertyChanged();
			}
		}

		[PublicAPI]
		public CertificateEncrypedValue CertificateEncryptedIv
		{
			get => _certificateEncryptedIv ?? (CertificateEncryptedIv = new CertificateEncrypedValue(_certificate));
		    set
			{
				if (Equals(value, _certificateEncryptedIv)) return;

				if (_certificateEncryptedIv != null)
				{
					_certificateEncryptedIv.PropertyChanged -= CertificateEncryptedIvPropertyChanged;
				}
				_certificateEncryptedIv = value;
				if (_certificateEncryptedIv != null)
				{
					_certificateEncryptedIv.PropertyChanged += CertificateEncryptedIvPropertyChanged;
				}
				OnPropertyChanged();
			}
		}

		private void CertificateEncryptedKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			AesEncryptor.Key = CertificateEncryptedKey.DecryptedValue;
		}

		private void CertificateEncryptedIvPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			AesEncryptor.Iv = CertificateEncryptedIv.DecryptedValue;
		}

		#region methods

		#endregion methods

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged
	}

	public class AesEncryptor : INotifyPropertyChanged
	{
	    // ReSharper disable once InconsistentNaming
	    private static readonly AesCryptoServiceProvider AES_CRYPTO_SERVICE_PROVIDER = new AesCryptoServiceProvider();
		private byte[] _iv;
		private byte[] _key;

		public byte[] Iv
		{
			get => _iv;
		    set
			{
				_iv = value;
				OnPropertyChanged();
			}
		}

		public byte[] Key
		{
			get => _key;
		    set
			{
				_key = value;
				OnPropertyChanged();
			}
		}

		[PublicAPI]
		public ICryptoTransform Decryptor => Key == null || Iv == null ? null : AES_CRYPTO_SERVICE_PROVIDER.CreateDecryptor(Key, Iv);

		[PublicAPI]
		public ICryptoTransform Encryptor => Key == null || Iv == null ? null : AES_CRYPTO_SERVICE_PROVIDER.CreateEncryptor(Key, Iv);

		private byte[] Transform(byte[] rawValue, ICryptoTransform transformer)
		{
			if (rawValue != null && transformer != null)
			{
				try
				{
					using (var cryptoStream = new CryptoStream(new MemoryStream(rawValue), transformer, CryptoStreamMode.Read))
					{
						using (var ms = new MemoryStream())
						{
							cryptoStream.CopyTo(ms);
							return ms.ToArray();
						}
					}
				}
				// ReSharper disable once UnusedVariable
#pragma warning disable 168
				catch (Exception ex)
#pragma warning restore 168
				{
					// ignore
				}
			}

			return null;
		}

		public byte[] Encrypt(byte[] rawValue) => Transform(rawValue, Encryptor);

		public byte[] Decrypt(byte[] rawValue) => Transform(rawValue, Decryptor);

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged
	}

	public class AesEncrypedValue : INotifyPropertyChanged
	{
		private byte[] _encryptedValue;
		private byte[] _decryptedValue;
		private readonly AesEncryptor _aesEncryptor;

		[PublicAPI]
		public byte[] DecryptedValue
		{
			get => _decryptedValue;
		    set
			{
				if (value == _decryptedValue) return;
				if (value != null && _decryptedValue != null && value.SequenceEqual(_decryptedValue)) return;
				_decryptedValue = value;

				OnPropertyChanged();

				EncryptedValue = _aesEncryptor.Encrypt(value);
			}
		}

		[PublicAPI]
		public byte[] EncryptedValue
		{
			get => _encryptedValue;
		    set
			{
				if (value == _encryptedValue) return;
				if (value != null && _encryptedValue != null && value.SequenceEqual(_encryptedValue)) return;
				_encryptedValue = value;
				OnPropertyChanged();

				DecryptedValue = _aesEncryptor.Decrypt(value);
			}
		}

		public AesEncrypedValue(AesEncryptor aesEncryptor)
		{
			_aesEncryptor = aesEncryptor;
			aesEncryptor.PropertyChanged += (sender, e) =>
			{
				if (DecryptedValue != null)
				{
					EncryptedValue = aesEncryptor.Encrypt(DecryptedValue);
				}
				else if (EncryptedValue != null)
				{
					DecryptedValue = aesEncryptor.Decrypt(EncryptedValue);
				}
			};
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged
	}

	public class CertificateEncrypedValue : INotifyPropertyChanged
	{
		private byte[] _encryptedValue;
		private byte[] _decryptedValue;
		private readonly X509Certificate2 _certificate2;

		[PublicAPI]
		public byte[] DecryptedValue
		{
			get => _decryptedValue;
		    set
			{
				if (value == _decryptedValue) return;
				if (value != null && _decryptedValue != null && value.SequenceEqual(_decryptedValue)) return;
				_decryptedValue = value;

				OnPropertyChanged();

				EncryptedValue = EncryptWithCertificate(value);
			}
		}

		[PublicAPI]
		public byte[] EncryptedValue
		{
			get => _encryptedValue;
		    set
			{
				if (value == _encryptedValue) return;
				if (value != null && _encryptedValue != null && value.SequenceEqual(_encryptedValue)) return;
				_encryptedValue = value;
				OnPropertyChanged();

				if (value?.Length > 0)
				{
					DecryptedValue = DecryptWithCertificate(value);
				}
				else
				{
					DecryptedValue = value;
				}
			}
		}

		private byte[] DecryptWithCertificate(byte[] value)
		{
			if (value != null)
			{
				try
				{
					var rsa = _certificate2.PrivateKey as RSACryptoServiceProvider;

					return rsa?.Decrypt(value, true);
				}
				catch
				{
					// ignore
				}
			}

			return null;
		}

		private byte[] EncryptWithCertificate(byte[] value)
		{
			if (value != null)
			{
				try
				{
					var rsa = _certificate2.PublicKey.Key as RSACryptoServiceProvider;

					return rsa?.Encrypt(value, true);
				}
				catch
				{
					// ignore
				}
			}

			return null;
		}

		public CertificateEncrypedValue(X509Certificate2 certificate2)
		{
			_certificate2 = certificate2;
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged
	}

	public class MainViewModel : INotifyPropertyChanged
	{
		#region fields

		private ObservableCollection<CertificateModel> _certificates = new ObservableCollection<CertificateModel>();
		private CertificateModel _selectedCertificate;
		private StoreName _selectedStoreName = StoreName.My;
		private StoreLocation _selectedStoreLocation = StoreLocation.LocalMachine;
		private string _searchText;

		#endregion fields

		public StoreLocation[] StoreLocations => (StoreLocation[])Enum.GetValues(typeof(StoreLocation));

		public StoreName[] StoreNames => (StoreName[])Enum.GetValues(typeof(StoreName));

		[PublicAPI]
		public StoreName SelectedStoreName
		{
			get => _selectedStoreName;
		    set
			{
				if (value == _selectedStoreName) return;
				_selectedStoreName = value;
				OnPropertyChanged();
				UpdateCertificates();
			}
		}

		[PublicAPI]
		public StoreLocation SelectedStoreLocation
		{
			get => _selectedStoreLocation;
		    set
			{
				if (value == _selectedStoreLocation) return;
				_selectedStoreLocation = value;
				OnPropertyChanged();
				UpdateCertificates();
			}
		}

		[PublicAPI]
		public ObservableCollection<CertificateModel> Certificates
		{
			get => _certificates;
		    set
			{
				if (Equals(value, _certificates)) return;
				_certificates = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(FilteredCertificates));
			}
		}

		[PublicAPI]
		public IEnumerable<CertificateModel> FilteredCertificates
		{
			get
			{
				if (string.IsNullOrWhiteSpace(SearchText))
					return Certificates;

				var split = SearchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				return (from certificate in Certificates
						from splitPart in split
						where (certificate.Certificate.Thumbprint != null && certificate.Certificate.Thumbprint.IndexOf(splitPart, StringComparison.InvariantCultureIgnoreCase) >= 0)
							   || (certificate.Certificate.FriendlyName.IndexOf(splitPart, StringComparison.InvariantCultureIgnoreCase) >= 0)
							   || (certificate.Certificate.Subject.IndexOf(splitPart, StringComparison.InvariantCultureIgnoreCase) >= 0)
						select certificate).Distinct();
			}
		}

		[PublicAPI]
		public CertificateModel SelectedCertificate
		{
			get => _selectedCertificate;
		    set
			{
				if (Equals(value, _selectedCertificate)) return;
				_selectedCertificate = value;
				OnPropertyChanged();
			}
		}

		[PublicAPI]
		public string SearchText
		{
			get => _searchText;
		    set
			{
				if (value == _searchText) return;
				_searchText = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(FilteredCertificates));
			}
		}

		public MainViewModel()
		{
			UpdateCertificates();
		}

		private void UpdateCertificates()
		{
			var certStore = new X509Store(SelectedStoreName, SelectedStoreLocation);
			certStore.Open(OpenFlags.ReadOnly);

			var certsToRemove = new List<CertificateModel>();
			var certsToAdd = new HashSet<X509Certificate2>();

			foreach (var certificate in certStore.Certificates)
			{
				if (!certificate.HasPrivateKey)
				{
					continue;
				}
				try
				{
					// ReSharper disable once UnusedVariable
					var eh = certificate.PrivateKey;
				}
				catch (Exception)
				{
					continue;
				}
				certsToAdd.Add(certificate);
			}

			foreach (var existingCert in Certificates)
			{
				if (!certsToAdd.Contains(existingCert.Certificate))
				{
					certsToRemove.Add(existingCert);
				}
				else
				{
					certsToAdd.Remove(existingCert.Certificate);
				}
			}

			foreach (var cert in certsToRemove)
			{
				Certificates.Remove(cert);
			}

			foreach (var cert in certsToAdd)
			{
				Certificates.Add(new CertificateModel(cert));
			}

			OnPropertyChanged(nameof(FilteredCertificates));
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged
	}
}