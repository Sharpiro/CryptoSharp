using CryptoSharp.Hashing;
using CryptoSharp.Wpf.Models;
using CryptoSharp.Wpf.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using CryptoSharp.Symmetric;

namespace CryptoSharp.Wpf.Windows
{
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly MessageBoxFacade _messageBox = new MessageBoxFacade();
        private readonly AesService _aesService = new AesService(new Sha256Hasher(), new MDFive128BitHasher());

        public MainWindow(MainWindowViewModel viewModel)
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

        private void EncryptButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                _viewModel.OutputText = null;
                if (_viewModel.CryptoSource == CryptoSource.File)
                    EncryptFile();
                else
                    EncryptText();
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

        private void DecryptButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                _viewModel.OutputText = null;
                if (_viewModel.CryptoSource == CryptoSource.File)
                    DecryptFile();
                else
                    DecryptText();
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
                    Owner = this,
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
            var crypto64 = Convert.ToBase64String(cryptoBytes);
            //var hexString = cryptoBytes.Select(b => b.ToString("X")).StringJoin(" ");
            _viewModel.OutputText = crypto64;
            //_viewModel.OutputText = hexString;
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
            var bytes = Convert.FromBase64String(_viewModel.InputText);
            var plainBytes = _aesService.Decrypt(bytes, _viewModel.Key, _viewModel.IV);
            var plainText = Encoding.UTF8.GetString(plainBytes);
            _viewModel.OutputText = plainText;
        }
    }
}