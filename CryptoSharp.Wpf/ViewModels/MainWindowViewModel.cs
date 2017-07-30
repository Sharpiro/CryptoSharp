using System.IO;

namespace CryptoSharp.Wpf.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; OnPropertyChanged(); OnPropertyChanged(nameof(FileExists)); }
        }

        public bool FileExists => File.Exists(_filePath);
    }
}