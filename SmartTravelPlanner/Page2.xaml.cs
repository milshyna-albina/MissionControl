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
    public partial class Page2 : Page
    {
        private Traveler? traveler = null;
        private CityGraph? map = null;
        private readonly Brush defaultBorder;
        public Page2()
        {
            InitializeComponent();
            defaultBorder = LoadDataButton.BorderBrush;
        }

        private void ShowError(Control control, string message)
        {
            switch (control)
            {
                case InputTextBox box:
                    box.ShowTBError(message);
                    break;

                case Button btn when btn == LoadDataButton:
                    TravelerStatusText.Text = message;
                    TravelerStatusText.Foreground = Brushes.Red;
                    TravelerStatusText.Visibility = Visibility.Visible;
                    btn.BorderBrush = Brushes.Red;
                    break;

                case Button btn when btn == LoadMapButton:
                    MapStatusText.Text = message;
                    MapStatusText.Foreground = Brushes.Red;
                    MapStatusText.Visibility = Visibility.Visible;
                    btn.BorderBrush = Brushes.Red;
                    break;

                case Button btn when btn == PlanRouteButton:
                    PlanRouteStatusText.Text = message;
                    PlanRouteStatusText.Foreground = Brushes.Red;
                    PlanRouteStatusText.Visibility = Visibility.Visible;
                    btn.BorderBrush = Brushes.Red;
                    break;

                default:
                    break;
            }
        }
              
        private void HideError(Control control)
        {
            switch (control)
            {
                case InputTextBox box:
                    box.HideTBError();
                    break;

                case Button btn when btn == LoadDataButton:
                    TravelerStatusText.Visibility = Visibility.Collapsed;
                    btn.BorderBrush = defaultBorder;
                    break;

                case Button btn when btn == LoadMapButton:
                    MapStatusText.Visibility = Visibility.Collapsed;
                    btn.BorderBrush = defaultBorder;
                    break;

                case Button btn when btn == PlanRouteButton:
                    PlanRouteStatusText.Visibility = Visibility.Collapsed;
                    btn.BorderBrush = defaultBorder;
                    break;
            }
        }

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Select file with data";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                try
                {
                    traveler = Traveler.LoadFromFile(path);

                    HideError(LoadDataButton);
                    HideError(PlanRouteButton);

                    TravelerStatusText.Text = $"Traveler {traveler.GetName()} loaded!";
                    TravelerStatusText.Foreground = Brushes.Purple;
                    TravelerStatusText.Visibility = Visibility.Visible;
                }
                catch (FileNotFoundException)
                {
                    ShowError(LoadDataButton, "Data file not found!");
                }
                catch (Exception )
                {
                    traveler = null;
                    ShowError(LoadDataButton, $"Error loading traveler!");
                }
            }
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
                    HideError(LoadMapButton);
                    HideError(PlanRouteButton);

                    MapStatusText.Foreground = Brushes.Purple;
                    MapStatusText.Visibility = Visibility.Visible;
                    LoadMapButton.BorderBrush = defaultBorder;
                }
                catch (FileNotFoundException)
                {
                    map = null;
                    ShowError(LoadMapButton, "Map file not found!");
                }
                catch (Exception)
                {
                    map = null;
                    ShowError(LoadMapButton, "Invalid format!");
                }
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;

            if (traveler == null)
            {
                ShowError(LoadDataButton, "Empty traveler!");
                hasError = true;
            }

            if (map == null)
            {
                ShowError(LoadMapButton, "Map file not found!");
                hasError = true;
            }

            if (hasError)
                return;

            string route = traveler.GetRoute();
            string[] cities = route.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);

            foreach (string city in cities)
            {
                if (!map.ContainsCity(city))
                {
                    ShowError(PlanRouteButton, $"There is no \"{city}\" in the map!");
                    return;
                }
            }
            
            HideError(LoadDataButton);
            HideError(LoadMapButton);
            PlanRouteButton.BorderBrush = defaultBorder;

            Page3 page3 = new Page3(traveler, map);
            this.NavigationService.Navigate(page3);
        }


        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (map != null || traveler is not null)
            {
                ReturnWarningWindow warningWindow = new ReturnWarningWindow();
                warningWindow.ShowDialog();

                if (warningWindow.IsConfirmed)
                {
                    NavigationService.Navigate(new Page0());
                }
            }
            else
            {
                if (NavigationService != null && NavigationService.CanGoBack)
                    NavigationService.GoBack();
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
