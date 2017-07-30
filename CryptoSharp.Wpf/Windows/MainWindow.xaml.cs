using CryptoSharp.Hashing;
using CryptoSharp.Wpf.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace CryptoSharp.Wpf.Windows
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel = new MainWindowViewModel();
        private readonly MessageBoxFacade _messageBox = new MessageBoxFacade();
        private readonly AesService _aesService = new AesService(new Sha256Hasher());

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel;
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

        private void BrowseButton_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (!_viewModel.FileExists) return;
                var bytes = File.ReadAllBytes(_viewModel.FilePath);
                var aesData = _aesService.CreateKey();
                var cryptoBytes = _aesService.Encrypt(bytes, aesData.Key, aesData.IV);
                var parentDir = Directory.GetParent(_viewModel.FilePath);
                File.WriteAllBytes($"{parentDir}/crypto.bin", cryptoBytes);
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
        }
    }
}