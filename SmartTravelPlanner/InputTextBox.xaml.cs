using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Travelling
{
    public partial class InputTextBox : UserControl
    {
        public InputTextBox()
        {
            InitializeComponent();
        }

        private string placeholder;
        private string header;

        public string Placeholder
        {
            get { return placeholder; }
            set 
            { 
                placeholder = value;
                tbPlaceholder.Text = placeholder;
            }
        }

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                tbName.Text = header;
            }
        }

        public void ShowTBError(string message)
        {
            tbError.Text = message;
            tbError.Visibility = Visibility.Visible;
            borderInput.BorderBrush = Brushes.Red;
            borderInput.BorderThickness = new Thickness(3);
        }

        public void HideTBError()
        {
            tbError.Visibility = Visibility.Collapsed;
            borderInput.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7D619B"));
            borderInput.BorderThickness = new Thickness(3);
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                ShowTBError("This field cannot be empty!");
                return false;
            }

            HideTBError();
            return true;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbPlaceholder.Visibility = string.IsNullOrEmpty(txtInput.Text) ? Visibility.Visible : Visibility.Hidden;

            if (!string.IsNullOrWhiteSpace(txtInput.Text))
            {
                HideTBError();
            }
        }

        public string Text => txtInput.Text;
    }
}
