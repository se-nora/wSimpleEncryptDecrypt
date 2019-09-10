using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Xml.Serialization;
using ConfigurationTool.XML.XMLServiceConfiguration;
using wSimpleEncryptDecrypt.Converter;
using wSimpleEncryptDecrypt.Model;

namespace wSimpleEncryptDecrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Lazy<SelfSignedCertificateCreator> m_lazyCertificateGenerator = new Lazy<SelfSignedCertificateCreator>(() => new SelfSignedCertificateCreator());

        private const uint WM_DROPFILES = 0x0233;
        private const uint WM_COPYDATA = 0x4A;
        private const uint MSGFLT_ADD = 1;

        public enum MessageFilterInfo : uint
        {
            None = 0, AlreadyAllowed = 1, AlreadyDisAllowed = 2, AllowedHigher = 3
        };

        public enum ChangeWindowMessageFilterExAction : uint
        {
            Reset = 0, Allow = 1, DisAllow = 2
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CHANGEFILTERSTRUCT
        {
            public uint size;
            public MessageFilterInfo info;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr ChangeWindowMessageFilter(uint message, uint dwFlag);

        [DllImport("user32")]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, ChangeWindowMessageFilterExAction action, ref CHANGEFILTERSTRUCT changeInfo);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var handle = new WindowInteropHelper(GetWindow(this)).Handle;

            var eh = new CHANGEFILTERSTRUCT();
            eh.size = (uint)Marshal.SizeOf(eh);

            for (uint i = 1; i <= WM_DROPFILES; i++)
            {
                ChangeWindowMessageFilterEx(handle, i, ChangeWindowMessageFilterExAction.Allow, ref eh);
            }
            var b1 = ChangeWindowMessageFilterEx(handle, 0x0049, ChangeWindowMessageFilterExAction.Allow, ref eh);
            var b2 = ChangeWindowMessageFilterEx(handle, WM_DROPFILES, ChangeWindowMessageFilterExAction.Allow, ref eh);
            var b3 = ChangeWindowMessageFilterEx(handle, WM_COPYDATA, ChangeWindowMessageFilterExAction.Allow, ref eh);

            //ChangeWindowMessageFilter(WM_DROPFILES, MSGFLT_ADD);
            //ChangeWindowMessageFilter(WM_COPYDATA, MSGFLT_ADD);
            //ChangeWindowMessageFilter(0x0049, MSGFLT_ADD);
        }

        private MainViewModel Model
        {
            get
            {
                return MainGrid.DataContext as MainViewModel;
            }
        }

        private void CreateIVCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var certificate = e.Parameter as CertificateModel;
            using (var cp = new AesCryptoServiceProvider())
            {
                cp.GenerateIV();
                if (certificate != null)
                {
                    try
                    {
                        certificate.AesEncryptor.Iv = cp.IV;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        private void CreateKeyCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var certificate = e.Parameter as CertificateModel;
            using (var cp = new AesCryptoServiceProvider())
            {
                cp.GenerateKey();
                if (certificate != null)
                {
                    try
                    {
                        certificate.AesEncryptor.Key = cp.Key;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        private void CommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var certificate = e.Parameter as CertificateModel;
            e.CanExecute = certificate != null;
        }

        private void CreateSelfSignedCertificateCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            m_lazyCertificateGenerator.Value.Show();
            m_lazyCertificateGenerator.Value.Focus();
        }

        private void CopyCommandParameterCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (e.Parameter?.ToString() ?? "") != "";
        }

        private void CopyCommandParameterCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(e.Parameter.ToString());
        }

        private void UIElement_OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            var certificateModel = (sender as GroupBox)?.DataContext as CertificateModel;

            if (certificateModel != null)
            {
                var data = e.Data.GetData(DataFormats.FileDrop);

                var eh = (string[])data;
                if (data != null)
                {
                    var xs = new XmlSerializer(typeof(ServiceConfiguration));
                    using (var fs = File.OpenRead(eh[0]))
                    {
                        var obj = (ServiceConfiguration)xs.Deserialize(fs);
                        var key = obj.RoleList.SelectMany(x => x.SettingList).FirstOrDefault(x => x.Name == "TeamViewer.Console.Credentials.Encryption.Key");
                        var iv = obj.RoleList.SelectMany(x => x.SettingList).FirstOrDefault(x => x.Name == "TeamViewer.Console.Credentials.Encryption.IV");

                        certificateModel.CertificateEncryptedKey.EncryptedValue = (byte[])Base64Converter.ConvertBack(key?.Value);
                        certificateModel.CertificateEncryptedIv.EncryptedValue = (byte[])Base64Converter.ConvertBack(iv?.Value);
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_lazyCertificateGenerator.IsValueCreated)
            {
                m_lazyCertificateGenerator.Value.CloseReally();
            }
        }
    }
}