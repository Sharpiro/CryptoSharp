using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace CryptoSharp.Wpf.Controls
{
    public partial class InputControl
    {
        private readonly MessageBoxFacade _messageBoxFacade;
        public string InputValue { get; set; }

        public InputControl(MessageBoxFacade messageBoxFacade)
        {
            InitializeComponent();
            Background = Brushes.LightGray;

            _messageBoxFacade = messageBoxFacade ?? throw new ArgumentNullException(nameof(messageBoxFacade));
            Loaded += (sender, args) => { InputTextBox.Focus(); };
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as Window;
                if (parent == null) throw new InvalidOperationException("Could not get parent window of input control");

                InputValue = InputTextBox.Text;
                if (string.IsNullOrEmpty(InputValue)) throw new InvalidDataException("You must enter a value");
                parent.DialogResult = true;
            }
            catch (Exception ex)
            {
                _messageBoxFacade.ShowError(ex);
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as Window;
                if (parent == null) throw new InvalidOperationException("Could not get parent window of input control");

                parent.DialogResult = false;
            }
            catch (Exception ex)
            {
                _messageBoxFacade.ShowError(ex);
            }
        }
    }
}