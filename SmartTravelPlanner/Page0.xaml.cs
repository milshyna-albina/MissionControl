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
    public partial class Page0 : Page
    {
        public Page0()
        {
            InitializeComponent();
        }
        private void ManualEntryButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1());
        }
        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page2());
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = Window.GetWindow(this);

            if (mainWindow != null)
            {
                mainWindow.IsEnabled = false;
                mainWindow.Opacity = 0.5;
            }

            ExitWindow exitWindow = new ExitWindow();
            exitWindow.ShowDialog();

            if (mainWindow != null)
            {
                mainWindow.Opacity = 1;
                mainWindow.IsEnabled = true;
            }

            if (exitWindow.IsConfirmed)
            {
                Application.Current.Shutdown(); 
            }
        }
    }
}
