using System;
using System.Configuration;
using System.Windows;
using CryptoSharp.Symmetric;
using CryptoSharp.Wpf.ViewModels;
using CryptoSharp.Wpf.Windows;

namespace CryptoSharp.Wpf
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var messageBox = new MessageBoxFacade();
            try
            {
                (string key, string iv) = GetAesConfig();

                var viewModel = new EncryptionControlViewModel { KeyString = key, IVString = iv };
                //var window = new MainWindow(viewModel);
                var window = new PageOwner();
                window.Show();
            }
            catch (Exception ex)
            {
                messageBox.ShowError("Fatal Error...");
                messageBox.ShowError(ex);
                Shutdown(-1);
            }
        }

        private (string key, string iv) GetAesConfig()
        {
            var key64String = ConfigurationManager.AppSettings["Key"];
            var iv64String = ConfigurationManager.AppSettings["IV"];

            if (string.IsNullOrEmpty(key64String) && string.IsNullOrEmpty(iv64String))
            {
                (byte[] key, byte[] iv) = new AesService().CreateKey();
                return (Convert.ToBase64String(key), Convert.ToBase64String(iv));
            }

            if (string.IsNullOrEmpty(key64String) || string.IsNullOrEmpty(iv64String))
                throw new ConfigurationErrorsException("When providing keys in the config, you must specify either no data, or both a key and an iv");

            try
            {
                var _ = Convert.FromBase64String(key64String);
                var __ = Convert.FromBase64String(iv64String);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("The Key or IV found in the configuration was invalid", ex);
            }

            return (key64String, iv64String);
        }
    }
}