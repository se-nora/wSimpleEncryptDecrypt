using System;
using System.Security.Cryptography;

namespace wSimpleEncryptDecrypt.Helper
{
	class EncryptionHelper : IDisposable
	{
		private readonly RSACryptoServiceProvider m_rsaCryptoServiceProvider;
		public EncryptionHelper(RSACryptoServiceProvider rsaCryptoServiceProvider)
		{
			m_rsaCryptoServiceProvider = rsaCryptoServiceProvider;
		}

		internal byte[] Decrypt(byte[] p)
		{
			return m_rsaCryptoServiceProvider.Decrypt(p, true);
		}

		public void Dispose()
		{
			m_rsaCryptoServiceProvider.Dispose();
		}
	}
}
