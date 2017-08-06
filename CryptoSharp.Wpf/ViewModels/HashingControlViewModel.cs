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
        private BytesDisplayType _bytesStringDisplay = BytesDisplayType.Base64;
        private HasherType _selectedHasherType = HasherType.SHA1;
        private HashDisplayType _selectedHashDisplayType = HashDisplayType.Base64;

        public string InputText
        {
            get => _inputText;
            set { _inputText = value; OnPropertyChanged(); OnPropertyChanged(nameof(MarkPath)); }
        }
        public bool FileExists => File.Exists(_inputText);
        public CryptoSource CryptoSource
        {
            get => _cryptoSource;
            set { _cryptoSource = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsFileSource)); OnPropertyChanged(nameof(IsTextSource)); }
        }
        public bool IsFileSource => _cryptoSource == CryptoSource.File;
        public bool IsTextSource => _cryptoSource == CryptoSource.Text;
        public string OutputText
        {
            get => _outputText;
            set { _outputText = value; OnPropertyChanged(); }
        }
        public string MarkPath => FileExists ? "/content/checkmark.png" : "/content/exmark.png";
        public BytesDisplayType BytesStringDisplay
        {
            get => _bytesStringDisplay;
            set { _bytesStringDisplay = value; OnPropertyChanged(); }
        }
        public HasherType SelectedHasherType
        {
            get => _selectedHasherType;
            set { _selectedHasherType = value; OnPropertyChanged(); }
        }
        public HashDisplayType SelectedHashDisplayType
        {
            get => _selectedHashDisplayType;
            set { _selectedHashDisplayType = value; OnPropertyChanged(); }
        }

        public IEnumerable<HasherType> HasherTypes => Enum.GetValues(typeof(HasherType)).Cast<HasherType>();
        public IEnumerable<HashDisplayType> HashDisplayTypes => Enum.GetValues(typeof(HashDisplayType)).Cast<HashDisplayType>();
    }
}