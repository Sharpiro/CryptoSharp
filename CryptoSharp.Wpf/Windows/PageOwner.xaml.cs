using CryptoSharp.Wpf.Controls;
using CryptoSharp.Wpf.ViewModels;

namespace CryptoSharp.Wpf.Windows
{
    public partial class PageOwner
    {
        private readonly PageOwnerViewModel _viewModel = new PageOwnerViewModel();

        public PageOwner()
        {
            InitializeComponent();
            DataContext = _viewModel;
            EncryptionTab.Content = new EncryptionControl(new EncryptionControlViewModel());
            HashingTab.Content = new HashingUserControl(new HashingControlViewModel());
        }
    }
}