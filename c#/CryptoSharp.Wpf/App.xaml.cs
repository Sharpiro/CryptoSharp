﻿using System;
using System.Configuration;
using System.Windows;
using CryptoSharp.Symmetric;
using CryptoSharp.Wpf.ViewModels;
using CryptoSharp.Wpf.Windows;
using CryptoSharp.Wpf.Controls;

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
                var encryptionControl = new EncryptionControl(viewModel);
                var window = new PageOwner(encryptionControl);

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
                return (null, null);

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