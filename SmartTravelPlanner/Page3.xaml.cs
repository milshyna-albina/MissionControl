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
            var route = traveler.GetRoute();
            var citiesList = new List<string>(route.Split(" -> ", StringSplitOptions.RemoveEmptyEntries));
            int totalDistance = map.GetPathDistance(citiesList);
            bool hasRoute = totalDistance > 0 && !string.IsNullOrWhiteSpace(route);

            NoRoutePanel.Visibility = hasRoute ? Visibility.Collapsed : Visibility.Visible;
            RouteBorder.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            RouteListBox.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            TotalDistanceTextBlock.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            AllCitiesTextBlock.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            SaveButton.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            ClearRouteButton.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            ModifyButton.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;

            if (hasRoute)
            {
                foreach (var city in citiesList)
                {
                    RouteListBox.Items.Add(city);
                }
                TotalDistanceTextBlock.Text = $"Total distance: {totalDistance} km";
            }
            else
            {
                RouteTextBlock.Text = string.Empty;
                TotalDistanceTextBlock.Text = string.Empty;
            }
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
                    SaveStatusTextBlock.Foreground = Brushes.Purple;
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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (traveler == null)
            {
                ClearStatusTextBlock.Text = "Nothing to be cleared!";
                ClearStatusTextBlock.Foreground = Brushes.Red;
                ClearStatusTextBlock.Visibility = Visibility.Visible;
                return;
            }

            traveler.ClearRoute();
            RouteTextBlock.Text = "";
            RouteListBox.Items.Clear();
            RouteListBox.Visibility = Visibility.Collapsed;
            AllCitiesTextBlock.Visibility = Visibility.Collapsed;
            TotalDistanceTextBlock.Visibility = Visibility.Collapsed;
            RouteBorder.Visibility = Visibility.Collapsed;
            ClearedRoutePanel.Visibility = Visibility.Visible;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            string route = traveler.GetRoute();
            bool hasRoute = !string.IsNullOrWhiteSpace(route);

            if (!hasRoute)
            {
                ModifyButton.BorderBrush = Brushes.Red;
                ModifyStatusTextBlock.Text = "Cannot modify: route is empty!";
                ModifyStatusTextBlock.Foreground = Brushes.Red;
                ModifyStatusTextBlock.Visibility = Visibility.Visible;
                return;
            }
            ModifyButton.BorderBrush = Brushes.Black;
            ModifyStatusTextBlock.Visibility = Visibility.Collapsed;
            Traveler experimentalTraveler = (Traveler)traveler.Clone();
            Page4 page4 = new Page4(experimentalTraveler, map);
            this.NavigationService.Navigate(page4);
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

            ExitWindow exitWindow = new ExitWindow();

            exitWindow.ShowDialog();

            if (exitWindow.IsConfirmed)
            {
                Application.Current.Shutdown();
            }
        }


    }

}
