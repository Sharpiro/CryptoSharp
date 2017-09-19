using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CryptoSharp.Hashing;
using CryptoSharp.Wpf.Models;
using CryptoSharp.Models;

namespace CryptoSharp.Wpf.ViewModels
{
    public class HashingControlViewModel : BaseViewModel
    {
        private CryptoSource _cryptoSource = CryptoSource.Text;
        private string _inputText;
        private string _outputText;
        private string _compareText;
        private HasherType _selectedHasherType = HasherType.MD5;
        private BytesDisplayType _selectedBytesDisplayType = BytesDisplayType.Hex;

        public string InputText
        {
            get => _inputText;
            set { _inputText = value; OnPropertyChanged(); OnPropertyChanged(nameof(FileExistsMarkPath)); }
        }
        public string OutputText
        {
            get => _outputText;
            set { _outputText = value; OnPropertyChanged(); OnPropertyChanged(nameof(CompareMatchesMarkPath)); }
        }
        public string CompareText
        {
            get => _compareText;
            set { _compareText = value; OnPropertyChanged(); OnPropertyChanged(nameof(CompareMatchesMarkPath)); }
        }
        public bool FileExists => File.Exists(_inputText);
        public CryptoSource CryptoSource
        {
            get => _cryptoSource;
            set { _cryptoSource = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsFileSource)); OnPropertyChanged(nameof(IsTextSource)); }
        }
        public bool IsFileSource => _cryptoSource == CryptoSource.File;
        public bool IsTextSource => _cryptoSource == CryptoSource.Text;
        public string FileExistsMarkPath => FileExists ? "/content/checkmark.png" : "/content/exmark.png";
        public string CompareMatchesMarkPath => CompareMatches ? "/content/checkmark.png" : "/content/exmark.png";
        public HasherType SelectedHasherType
        {
            get => _selectedHasherType;
            set { _selectedHasherType = value; OnPropertyChanged(); }
        }
        public BytesDisplayType SelectedBytesDisplayType
        {
            get => _selectedBytesDisplayType;
            set { _selectedBytesDisplayType = value; OnPropertyChanged(); }
        }
        public IEnumerable<HasherType> HasherTypes => Enum.GetValues(typeof(HasherType)).Cast<HasherType>();
        public IEnumerable<BytesDisplayType> BytesDisplayTypes => Enum.GetValues(typeof(BytesDisplayType)).Cast<BytesDisplayType>();
        public bool CompareMatches => _outputText?.Equals(_compareText, StringComparison.InvariantCultureIgnoreCase) ?? false;
    }
}