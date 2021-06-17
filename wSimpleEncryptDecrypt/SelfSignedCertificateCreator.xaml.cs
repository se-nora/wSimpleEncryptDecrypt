using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using wSimpleEncryptDecrypt.Model;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wSimpleEncryptDecrypt
{
    /// <summary>
    /// Interaction logic for SelfSignedCertificateCreator.xaml
    /// </summary>
    public partial class SelfSignedCertificateCreator : Window
    {
        public event EventHandler<X509Certificate2> CertificateCreated;
        private string _targetLocation;

        public SelfSignedCertificateCreator()
        {
            InitializeComponent();
        }

        X509Certificate2 CreateSelfSignedCertificate(string subjectName, string path, string password, int keyLength, DateTime notBeforeData, DateTime expiryDate)
        {
            // look: http://blog.differentpla.net/blog/2013/03/18/using-bouncy-castle-from-net
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var certificateGenerator = new X509V3CertificateGenerator();

            // The certificate needs a serial number. This is used for revocation, and usually should be an incrementing index (which makes it easier to revoke a range of certificates).
            // Since we don’t have anywhere to store the incrementing index, we can just use a random number.
            var serialNumber =
                BigIntegers.CreateRandomInRange(
                    BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            //When we say that the certificate is signed using the issuer’s private key, what we actually mean is that the certificate is hashed to create a smaller hash value, and then that hash value is encrypted using the issuer’s private key.

            // For this, we need to specify a signature algorithm.Algorithms that you might have heard of are are MD5(totally broken, you should never use it)
            // and SHA-1(probably broken; no longer recommended).
            // For certificates, the current recommendation seems to be SHA-256 or SHA-512.


            var subjectDn = new X509Name($"CN={subjectName}");
            var issuerDn = subjectDn;

            // issuer and subject
            certificateGenerator.SetIssuerDN(issuerDn);
            certificateGenerator.SetSubjectDN(subjectDn);

            // set expiration dates
            certificateGenerator.SetNotBefore(notBeforeData);
            certificateGenerator.SetNotAfter(expiryDate);

            // this creates a new public/private key
            var keyGenerationParameters = new KeyGenerationParameters(random, keyLength);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            var issuerKeyPair = subjectKeyPair;

            const string signatureAlgorithm = "SHA512WithRSA";

            ISignatureFactory signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerKeyPair.Private, random);
            var certificate = certificateGenerator.Generate(signatureFactory);

            return ExportCertificate(path, certificate, subjectKeyPair, password, random);
        }

        private X509Certificate2 ExportCertificate(string path, X509Certificate certificate, AsymmetricCipherKeyPair subjectKeyPair,
            string password, SecureRandom random)
        {
            var store = new Pkcs12Store();
            string friendlyName = certificate.SubjectDN.ToString();

            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);

            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });

            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), random);

            var convertedCertificate =
                new X509Certificate2(
                    stream.ToArray(), password,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            File.WriteAllBytes(Path.Combine(path, convertedCertificate.Subject + ".pfx"), convertedCertificate.Export(X509ContentType.Pfx, password));
            return convertedCertificate;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var model = (CreateCertificateModel)Resources["Model"];

            model.IsBusy = true;

            if (_targetLocation == null)
            {
                var fbd = new WPFFolderBrowser.WPFFolderBrowserDialog("Target location for certificate");
                if (fbd.ShowDialog() ?? false)
                {
                    _targetLocation = fbd.FileName;
                }
            }

            Task.Run(() =>
            {
                try
                {
                    var certificate = CreateSelfSignedCertificate(
                        model.Subject,
                        _targetLocation,
                        model.Password,
                        model.KeyLength,
                        DateTime.UtcNow,
                        DateTime.UtcNow.Add(model.ValidDuration));

                    Dispatcher.Invoke(() =>
                    {
                        Clipboard.SetText(model.Password);
                        Focus();
                    });

                    CertificateCreated?.Invoke(this, certificate);
                }
                finally
                {
                    model.IsBusy = false;
                }
            });
        }

        Random r = new Random();

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string s = "abcdefghijklmnopqrstuvwxyz";
            s += s.ToUpper();
            s += "!§$%&/()=?`´*'+#-.,;:_><|²@\"\\³0123456789";
            var model = (CreateCertificateModel)Resources["Model"];
            model.Password = "";

            for (int i = 0; i++ < model.PasswordLength;)
            {
                model.Password += s[r.Next(s.Length)];
            }
        }

        private void CopyCommandParameterCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (e.Parameter?.ToString() ?? "") != "";
        }

        private void CopyCommandParameterCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(e.Parameter.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Button_Click_1(sender, e);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_shouldReallyClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private bool _shouldReallyClose;
        public void CloseReally()
        {
            _shouldReallyClose = true;
            Close();
        }
    }
}
