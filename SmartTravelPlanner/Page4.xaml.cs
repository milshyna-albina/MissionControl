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
    public partial class Page4 : Page
    {
        private Traveler originalTraveler;
        private Traveler traveler;
        private CityGraph map;

        public Page4(Traveler originalTraveler, CityGraph map)
        {
            InitializeComponent();
            this.originalTraveler = originalTraveler;
            this.traveler = (Traveler)originalTraveler.Clone();
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
            bool hasRoute = citiesList.Count > 0 && !string.IsNullOrWhiteSpace(route);

            NoRoutePanel.Visibility = hasRoute ? Visibility.Collapsed : Visibility.Visible;
            RouteBorder.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            RouteListBox.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            TotalDistanceTextBlock.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            AllCitiesTextBlock.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            SaveButton.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            ClearRouteButton.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            AddCityButton.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            RemoveCityButton.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;
            CityTextBox.Visibility = hasRoute ? Visibility.Visible : Visibility.Collapsed;

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


        private void AddCityButton_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text.Trim();

            if (string.IsNullOrEmpty(city))
            {
                CityTextBox.ShowTBError("Please enter a city name.");
                return;
            }

            if (!map.ContainsCity(city))
            {
                CityTextBox.ShowTBError("City not found on map!");
                return;
            }

            if (traveler.route.Count == 0)
            {
                traveler.SetLocation("");
            }

            if (traveler.route.Any(c => string.Equals(c, city, StringComparison.OrdinalIgnoreCase)))
            {
                CityTextBox.ShowTBError("City is already in the route!");
                return;
            }

            string startCity;
            if (traveler.route.Count == 0 || string.IsNullOrEmpty(traveler.currentLocation))
            {
                ClearedRoutePanel.Visibility = Visibility.Collapsed;
                traveler.AddCity(city);
                traveler.SetLocation(city);
                traveler.currentLocation = city;
                DisplayRoute();
                CityTextBox.Text = "";
                CityTextBox.HideTBError();
                return;
            }
            else
            {
                startCity = traveler.route.Last();
            }


            var path = map.FindShortestPath(startCity, city);

            if (path == null)
            {
                CityTextBox.ShowTBError("City is unreachable from current route!");
                return;
            }

            List<string> newCities;
            if (traveler.route.Count == 0)
            {
                newCities = path.ToList();
            }
            else
            {
                newCities = path.Skip(1).Where(c => !traveler.route.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            foreach (var c in newCities)
            {
                traveler.AddCity(c);
            }
            DisplayRoute();
            CityTextBox.Text = "";
        }

        private void RemoveCityButton_Click(object sender, RoutedEventArgs e)
        {
            string cityToRemove = CityTextBox.Text.Trim();

            if (string.IsNullOrEmpty(cityToRemove))
            {
                CityTextBox.ShowTBError("Please enter a city name.");
                return;
            }

            if (!traveler.route.Any(c => string.Equals(c, cityToRemove, StringComparison.OrdinalIgnoreCase)))
            {
                CityTextBox.ShowTBError("City not in route!");
                return;
            }

            if (traveler.route.Count <= 2)
            {
                CityTextBox.ShowTBError("Cannot remove city: route too short.");
                return;
            }

            string firstCity = traveler.route.First();
            string lastCity = traveler.route.Last();

            if (string.Equals(cityToRemove, firstCity, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(cityToRemove, lastCity, StringComparison.OrdinalIgnoreCase))
            {
                traveler.route = traveler.route.Where(c => !string.Equals(c, cityToRemove, StringComparison.OrdinalIgnoreCase)).ToList();
                if (traveler.GetLocation().Equals(cityToRemove, StringComparison.OrdinalIgnoreCase))
                {
                    if (traveler.route.Count > 0)
                    {
                        traveler.SetLocation(traveler.route.First());
                    }
                    else
                    {
                        traveler.SetLocation("");
                    }
                }
                DisplayRoute();
                CityTextBox.Text = "";
                CityTextBox.HideTBError();
                return;
            }

            var newPath = map.FindShortestPathAvoidCity(firstCity, lastCity, cityToRemove);

            if (newPath == null)
            {
                CityTextBox.ShowTBError("Cannot remove city: route would be broken!");
                return;
            }
            if (traveler.GetLocation().Equals(cityToRemove, StringComparison.OrdinalIgnoreCase))
            {
                if (traveler.route.Count > 0)
                {
                    traveler.SetLocation(traveler.route.First());
                }
                else
                {
                    traveler.SetLocation("");
                }
            }

                traveler.route = new List<string>(newPath);
            DisplayRoute();
            CityTextBox.Text = "";
            CityTextBox.HideTBError();
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


        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                Window mainWindow = Window.GetWindow(this);

                if (mainWindow != null)
                {
                    mainWindow.IsEnabled = false;
                    mainWindow.Opacity = 0.5;
                }

                WarningWindow warningWindow = new WarningWindow();
                warningWindow.ShowDialog();

                if (mainWindow != null)
                {
                    mainWindow.Opacity = 1;
                    mainWindow.IsEnabled = true;
                }

                if (warningWindow.IsConfirmed)
                {
                    NavigationService.Navigate(new Page3(originalTraveler, map));
                }
            }
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
