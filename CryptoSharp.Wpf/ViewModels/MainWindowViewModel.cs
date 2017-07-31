using System;
using System.IO;

namespace CryptoSharp.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _filePath;
        private string _keyString;
        private string _ivString;

        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; OnPropertyChanged(); OnPropertyChanged(nameof(FileExists)); }
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
    }
}