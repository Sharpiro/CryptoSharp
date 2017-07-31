using CryptoSharp.Hashing;
using CryptoSharp.Wpf.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

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

        private void EncrytpButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                if (!_viewModel.FileExists) throw new FileNotFoundException($"No file found @ '{_viewModel.FilePath}'");
                var bytes = File.ReadAllBytes(_viewModel.FilePath);
                var cryptoBytes = _aesService.Encrypt(bytes, _viewModel.Key, _viewModel.IV);
                var parentDir = Directory.GetParent(_viewModel.FilePath);
                File.WriteAllBytes($"{parentDir}/crypto.bin", cryptoBytes);
                _messageBox.ShowInfo("Successfully encrypted file");
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
                if (!_viewModel.FileExists) throw new FileNotFoundException($"No file found @ '{_viewModel.FilePath}'");
                var bytes = File.ReadAllBytes(_viewModel.FilePath);
                var cryptoBytes = _aesService.Decrypt(bytes, _viewModel.Key, _viewModel.IV);
                var parentDir = Directory.GetParent(_viewModel.FilePath);
                File.WriteAllBytes($"{parentDir}/plain.bin", cryptoBytes);
                _messageBox.ShowInfo("Successfully decrypted file");
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
                    ResizeMode = ResizeMode.NoResize
                };
                if (!infoWindow.ShowDialog() ?? false) return;

                (byte[] key, byte[] iv) = _aesService.CreateKey(inputControl.InputValue);
                _viewModel.KeyString = Convert.ToBase64String(key);
                _viewModel.IVString = Convert.ToBase64String(iv);
                _messageBox.ShowInfo($"Generated Key: '{_viewModel.KeyString}', IV: '{_viewModel.IVString}' from: '{inputControl.InputValue}'");
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
    }
}