using CryptoSharp.Wpf.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CryptoSharp.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _filePath;
        private string _keyString;
        private string _ivString;
        private CryptoSource _cryptoSource = CryptoSource.Text;
        private string _inputText;
        private string _outputText;

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

        public byte[] Key => Convert.FromBase64String(_keyString);
        public byte[] IV => Convert.FromBase64String(_ivString);
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
        public string Title => $"CryptoSharp-{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}";
        public string MarkPath => FileExists ? "/content/checkmark.png" : "/content/exmark.png";
    }
}