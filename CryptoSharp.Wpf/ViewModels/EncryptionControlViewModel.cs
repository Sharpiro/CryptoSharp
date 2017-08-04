using CryptoSharp.Wpf.Models;
using System;
using System.IO;

namespace CryptoSharp.Wpf.ViewModels
{
    public class EncryptionControlViewModel : BaseViewModel
    {
        private string _filePath;
        private string _keyString;
        private string _ivString;
        private CryptoSource _cryptoSource = CryptoSource.Text;
        private string _inputText;
        private string _outputText;
        private BytesStringDisplay _bytesStringDisplay = BytesStringDisplay.Base64;

        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; OnPropertyChanged(); OnPropertyChanged(nameof(FileExists)); OnPropertyChanged(nameof(MarkPath)); }
        }
        public string KeyString
        {
            get => _keyString;
            set { _keyString = value; OnPropertyChanged(); OnPropertyChanged(nameof(Key)); }
        }
        public string IVString
        {
            get => _ivString;
            set { _ivString = value; OnPropertyChanged(); OnPropertyChanged(nameof(IV)); }
        }

        public byte[] Key => string.IsNullOrEmpty(_keyString) ? null : Convert.FromBase64String(_keyString);
        public byte[] IV => string.IsNullOrEmpty(_ivString) ? null : Convert.FromBase64String(_ivString);
        public bool FileExists => File.Exists(_filePath);
        public CryptoSource CryptoSource
        {
            get => _cryptoSource;
            set { _cryptoSource = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsFileSource)); OnPropertyChanged(nameof(IsTextSource)); }
        }
        public bool IsFileSource => _cryptoSource == CryptoSource.File;
        public bool IsTextSource => _cryptoSource == CryptoSource.Text;
        public string InputText
        {
            get => _inputText;
            set { _inputText = value; OnPropertyChanged(); }
        }
        public string OutputText
        {
            get => _outputText;
            set { _outputText = value; OnPropertyChanged(); }
        }
        public string MarkPath => FileExists ? "/content/checkmark.png" : "/content/exmark.png";
        public BytesStringDisplay BytesStringDisplay
        {
            get => _bytesStringDisplay;
            set { _bytesStringDisplay = value; OnPropertyChanged(); }
        }
    }
}