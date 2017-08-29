using System;
using CryptoSharp.Wpf.Controls;
using CryptoSharp.Wpf.ViewModels;

namespace CryptoSharp.Wpf.Windows
{
    public partial class PageOwner
    {
        private readonly PageOwnerViewModel _viewModel = new PageOwnerViewModel();

        public PageOwner(EncryptionControl encryptionControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            EncryptionTab.Content = encryptionControl ?? throw new ArgumentNullException(nameof(encryptionControl));
            HashingTab.Content = new HashingUserControl(new HashingControlViewModel());
        }
    }
}