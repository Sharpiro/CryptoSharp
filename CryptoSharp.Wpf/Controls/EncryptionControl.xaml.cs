using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CryptoSharp.Hashing;
using CryptoSharp.Symmetric;
using CryptoSharp.Wpf.Models;
using CryptoSharp.Wpf.ViewModels;
using CryptoSharp.Wpf.Windows;
using Microsoft.Win32;
using CryptoSharp.Models;

namespace CryptoSharp.Wpf.Controls
{
    public partial class EncryptionControl
    {
        private readonly EncryptionControlViewModel _viewModel;
        private readonly MessageBoxFacade _messageBox = new MessageBoxFacade();
        private readonly AesService _aesService = new AesService(new Sha256BitHasher(), new MDFive128BitHasher());

        public EncryptionControl(EncryptionControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = _viewModel = viewModel;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                var dialogResult = dialog.ShowDialog() ?? false;
                if (!dialogResult) return;
                _viewModel.FilePath = dialog.FileName;
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
        }

        private async void EncryptButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                _viewModel.OutputText = null;
                await Task.Run(() =>
                {
                    if (_viewModel.CryptoSource == CryptoSource.File)
                        EncryptFile();
                    else
                        EncryptText();
                });
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private async void DecryptButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                _viewModel.OutputText = null;
                await Task.Run(() =>
                {
                    if (_viewModel.CryptoSource == CryptoSource.File)
                        DecryptFile();
                    else
                        DecryptText();
                });
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void GenKeyButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                (byte[] key, byte[] iv) = _aesService.CreateKey();
                _viewModel.KeyString = Convert.ToBase64String(key);
                _viewModel.IVString = Convert.ToBase64String(iv);
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void GenKeyFromButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                var inputControl = new InputControl(_messageBox);
                var infoWindow = new Window()
                {
                    //WindowStyle = WindowStyle.None,
                    Title = "Enter a phrase to generate keys",
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Width = 275,
                    Height = 150,
                    Content = inputControl,
                    //Owner = this,
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize,
                    Icon = new BitmapImage(new Uri("pack://application:,,,/content/cryptolock.png"))
                };
                if (!infoWindow.ShowDialog() ?? false) return;

                (byte[] key, byte[] iv) = _aesService.CreateKey(inputControl.InputValue);
                _viewModel.KeyString = Convert.ToBase64String(key);
                _viewModel.IVString = Convert.ToBase64String(iv);
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void EncryptFile()
        {
            var fileInfo = new FileInfo(_viewModel.FilePath);
            if (!fileInfo.Exists) throw new FileNotFoundException($"No file found @ '{_viewModel.FilePath}'");
            var bytes = File.ReadAllBytes(_viewModel.FilePath);
            var cryptoBytes = _aesService.Encrypt(bytes, _viewModel.Key, _viewModel.IV);
            var parentDir = Directory.GetParent(_viewModel.FilePath);
            File.WriteAllBytes($"{fileInfo.FullName}.crypto", cryptoBytes);
            _messageBox.ShowInfo("Successfully encrypted file");
        }

        private void EncryptText()
        {
            if (string.IsNullOrEmpty(_viewModel.InputText)) throw new ArgumentException("Must provide input text");
            var bytes = Encoding.UTF8.GetBytes(_viewModel.InputText);
            var cryptoBytes = _aesService.Encrypt(bytes, _viewModel.Key, _viewModel.IV);
            _viewModel.OutputText = _viewModel.BytesStringDisplay == BytesDisplayType.Base64 ?
                Convert.ToBase64String(cryptoBytes) : cryptoBytes.Select(b => b.ToString("X2")).StringJoin(" ");
        }

        private void DecryptFile()
        {
            var fileInfo = new FileInfo(_viewModel.FilePath);
            if (!fileInfo.Exists) throw new FileNotFoundException($"No file found @ '{_viewModel.FilePath}'");
            var bytes = File.ReadAllBytes(_viewModel.FilePath);
            var cryptoBytes = _aesService.Decrypt(bytes, _viewModel.Key, _viewModel.IV);
            var parentDir = Directory.GetParent(_viewModel.FilePath);
            var path = fileInfo.Extension.ToLower().Equals(".crypto") ? fileInfo.FullName.Substring(0, fileInfo.FullName.Length - 7) : $"{fileInfo.FullName}.decrypted";
            File.WriteAllBytes(path, cryptoBytes);
            _messageBox.ShowInfo("Successfully decrypted file");
        }

        private void DecryptText()
        {
            if (string.IsNullOrEmpty(_viewModel.InputText)) throw new ArgumentException("Must provide input text");

            var cryptoBytes = _viewModel.BytesStringDisplay == BytesDisplayType.Base64 ?
                Convert.FromBase64String(_viewModel.InputText) : _viewModel.InputText.Split(new[] { " " }, StringSplitOptions.None).Select(hexString => Convert.ToByte(hexString, 16)).ToArray();
            var plainBytes = _aesService.Decrypt(cryptoBytes, _viewModel.Key, _viewModel.IV);
            var plainText = Encoding.UTF8.GetString(plainBytes);
            _viewModel.OutputText = plainText;
        }

        private void BytesDisplayType_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_viewModel.OutputText)) return;

                EncryptText();
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
        }
    }
}
