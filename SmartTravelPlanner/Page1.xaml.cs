using Microsoft.Win32;
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
using System.IO;
using Travelling;

namespace SmartTravelPlanner
{
    public partial class Page1 : Page
    {
        private CityGraph? map = null;
        private readonly Brush defaultBorder;
        public Page1()
        {
            InitializeComponent();
            defaultBorder = LoadMapButton.BorderBrush;
        }

        private void LoadMapButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt";
            dialog.Title = "Select map file";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                try
                {
                    map = CityGraph.LoadFromFile(path);
                    MapStatusText.Text = "Map successfully loaded!";
                    MapStatusText.Foreground = Brushes.Black;
                    MapStatusText.Visibility = Visibility.Visible;
                    LoadMapButton.BorderBrush = defaultBorder;
                }
                catch (FileNotFoundException)
                {
                    LoadMapButton.BorderBrush = Brushes.Red;
                    MapStatusText.Text = "Map file not found!";
                    MapStatusText.Foreground = Brushes.Red;
                    MapStatusText.Visibility = Visibility.Visible;
                    map = null;
                }
                catch (Exception)
                {
                    LoadMapButton.BorderBrush = Brushes.Red;
                    MapStatusText.Text = "Invalid format!";
                    MapStatusText.Foreground = Brushes.Red;
                    MapStatusText.Visibility = Visibility.Visible;
                    map = null;
                }
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            bool validName = NameTextBox.Validate();
            bool validLocation = LocationTextBox.Validate();

            if (map == null)
            {
                MapStatusText.Text = "Map file not found!";
                MapStatusText.Foreground = Brushes.Red;
                MapStatusText.Visibility = Visibility.Visible;
                LoadMapButton.BorderBrush = Brushes.Red;
            }

            if (!validName || !validLocation || map == null)
            {
                return;
            }

            MapStatusText.Visibility = Visibility.Collapsed;
            LoadMapButton.BorderBrush = defaultBorder;
            this.NavigationService.Navigate(new Page2());
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы Альбина Малышина?",
                "придется признать...",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
