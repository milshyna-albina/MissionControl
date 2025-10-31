using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class Page3 : Page
    {
        private Traveler traveler;
        private CityGraph map;

        public Page3(Traveler traveler, CityGraph map)
        {
            InitializeComponent();
            this.traveler = traveler;
            this.map = map;
            DisplayRoute();
        }

        private void DisplayRoute()
        {
            RouteTextBlock.Text = traveler.GetRoute();
            RouteListBox.Items.Clear();
            foreach (var city in traveler.GetRoute().Split(" -> "))
            {
                RouteListBox.Items.Add(city);
            }
            var citiesList = new List<string>(traveler.GetRoute().Split(" -> "));
            int totalDistance = map.GetPathDistance(citiesList);
            TotalDistanceTextBlock.Text = $"Total distance: {totalDistance} km";
 
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Save route as";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                try
                {
                    traveler.SaveToFile(path);
                    SaveStatusTextBlock.Text = "Route successfully saved!";
                    SaveStatusTextBlock.Foreground = Brushes.Green;
                    SaveStatusTextBlock.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    SaveStatusTextBlock.Text = "Error saving route!";
                    SaveStatusTextBlock.Foreground = Brushes.Red;
                    SaveStatusTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        private void LoadMapButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Select Traveler JSON file";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                try
                {
                    Traveler traveler = Traveler.LoadFromFile(path);

      
                    LoadStatusTextBlock.Text = $"Traveler '{traveler.GetName()}' loaded successfully!";

                    string route = traveler.GetRoute();
                    RouteTextBlock.Text = $"Current Location: {traveler.GetLocation()}\nRoute: {route}";
                }
                catch (FileNotFoundException ex)
                {
                    LoadStatusTextBlock.Text = $"Error: {ex.Message}";
                }
                catch (FileLoadException ex)
                {
                    LoadStatusTextBlock.Text = $"Error: {ex.Message}";
                }
                catch (Exception ex)
                {
                    LoadStatusTextBlock.Text = $"Unexpected error: {ex.Message}";
                }
            }
        }


        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                NavigationService.Navigate(new Page0());
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "ARE U LEAVING US???",     
                "how dare u...",         
                MessageBoxButton.YesNo,       
                MessageBoxImage.Question);     

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown(); 
            }
        }


    }

}
