using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CryptoSharp.Hashing;
using CryptoSharp.Wpf.ViewModels;
using Microsoft.Win32;
using CryptoSharp.Models;

namespace CryptoSharp.Wpf.Controls
{
    public partial class HashingUserControl
    {
        private readonly HashingControlViewModel _viewModel;
        private readonly MessageBoxFacade _messageBox = new MessageBoxFacade();
        private IHasher _hasher;
        private readonly HasherFactory _hasherFactory = new HasherFactory();

        public HashingUserControl(HashingControlViewModel viewModel)
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
                _viewModel.InputText = dialog.FileName;
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
        }

        private async void InputTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                await Task.Run(() =>
                {
                    if (_viewModel.IsTextSource)
                        HashText();
                    else
                        HashFile();
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

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.InputText = null;
                _viewModel.OutputText = null;
            }
            catch (Exception ex)
            {
                _messageBox.ShowError(ex);
            }
        }

        private void HashText()
        {
            if (string.IsNullOrEmpty(_viewModel.InputText))
            {
                _viewModel.OutputText = null;
                return;
            }

            try
            {
                var saltBytes = GetSaltBytes();
                _viewModel.InputText = _viewModel.InputText.Replace("\\0", "\0").Replace("\\r\\n", "\r\n");
                var inputBytes = Encoding.UTF8.GetBytes(_viewModel.InputText).Concat(saltBytes).ToArray();
                var hashedBytes = _hasher.CreateHash(inputBytes);
                _viewModel.OutputText = GetFormattedHash(hashedBytes);
            }
            catch (Exception)
            {
                _viewModel.OutputText = null;
                throw;
            }
        }

        private void HashFile()
        {
            if (!File.Exists(_viewModel.InputText))
            {
                _viewModel.OutputText = null;
                return;
            }

            try
            {
            var saltBytes = GetSaltBytes();
            var inputBytes = File.ReadAllBytes(_viewModel.InputText).Concat(saltBytes).ToArray();
            var hashedBytes = _hasher.CreateHash(inputBytes);
            if (hashedBytes.Length > 100) throw new InvalidOperationException("Max hash size is 100");
            _viewModel.OutputText = GetFormattedHash(hashedBytes);
            }
            catch (Exception)
            {
                _viewModel.OutputText = null;
                throw;
            }
        }

        private byte[] GetSaltBytes()
        {
            if (!_viewModel.UseSalt || string.IsNullOrEmpty(_viewModel.SaltText)) return new byte[0];
            //if (_viewModel.UseSalt && string.IsNullOrEmpty(_viewModel.SaltText))
            //    throw new InvalidOperationException("Must provide a valid salt value when 'Salt' option is enabled");
            if (_viewModel.SaltText.Length % 2 != 0) throw new InvalidOperationException("Salt must be an even number of hex symbols");

            try
            {
                return Enumerable.Range(0, _viewModel.SaltText.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(_viewModel.SaltText.Substring(x, 2), 16))
                    .ToArray();
            }
            catch
            {
                throw new InvalidOperationException("The salt is not valid hexadecimal where each byte is represented by exactly 2 symbols");
            }
        }

        private string GetFormattedHash(byte[] bytes)
        {
            switch (_viewModel.SelectedBytesDisplayType)
            {
                case BytesDisplayType.Base64: return Convert.ToBase64String(bytes);
                case BytesDisplayType.Hex: return bytes.Select(b => b.ToString("X2")).StringJoin(" ");
                case BytesDisplayType.HexSquished: return bytes.Select(b => b.ToString("X2")).StringJoin("");
                case BytesDisplayType.Guid: return new Guid(bytes.Take(16).ToArray()).ToString();
                default: throw new ArgumentOutOfRangeException($"Unable to determine hash display type for: '{_viewModel.SelectedBytesDisplayType}'");
            }
        }

        private async void HasherType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                await Task.Run(() =>
                {
                    _hasher = _hasherFactory.CreateHasher(_viewModel.SelectedHasherType);
                    if (_viewModel.IsTextSource) HashText();
                    else HashFile();
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

        private async void BytesDisplayType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                await Task.Run(() =>
                {
                    if (_viewModel.IsTextSource) HashText();
                    else HashFile();
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

        private async void SaltCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                await Task.Run(() =>
                {
                    if (_viewModel.IsTextSource) HashText();
                    else HashFile();
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

        private async void SaltTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                IsEnabled = false;
                await Task.Run(() =>
                {
                    if (_viewModel.IsTextSource) HashText();
                    else HashFile();
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
    }
}