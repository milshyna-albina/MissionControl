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

namespace Travelling
{
    public partial class Page1 : Page
    {
        private CityGraph? map = null;
        private readonly Brush defaultBorder;
        private bool travelerCreatedSuccessfully = false;
        public Page1()
        {
            InitializeComponent();
            defaultBorder = LoadMapButton.BorderBrush;
        }

        public void ShowError(Control control, string message)
        {
            switch (control)
            {
                case InputTextBox box:
                    box.ShowTBError(message);
                    break;

                case Button btn when btn == LoadMapButton || btn == CreateTravelerButton:
                    if (btn == LoadMapButton)
                    {
                        MapStatusText.Text = message;
                        MapStatusText.Foreground = Brushes.Red;
                        MapStatusText.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TravelStatusText.Text = message;
                        TravelStatusText.Foreground = Brushes.Red;
                        TravelStatusText.Visibility = Visibility.Visible;
                    }
                    btn.BorderBrush = Brushes.Red;
                    break;

                default:
                    break;
            }
        }

        public void HideError(Control control)
        {
            switch (control)
            {
                case InputTextBox box:
                    box.HideTBError();
                    break;

                case Button btn when btn == LoadMapButton || btn == CreateTravelerButton:
                    if (btn == LoadMapButton)
                        MapStatusText.Visibility = Visibility.Collapsed;
                    else
                        TravelStatusText.Visibility = Visibility.Collapsed;

                    btn.BorderBrush = defaultBorder;
                    break;
            }
        }

        private void CreateTravelerButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string location = LocationTextBox.Text;

            if (!NameTextBox.Validate())
            {
                ShowError(CreateTravelerButton, "Traveler name cannot be empty!");
                return;
            }

            if (!LocationTextBox.Validate())
            {
                ShowError(CreateTravelerButton, "Traveler location cannot be empty!");
                return;
            }

            HideError(CreateTravelerButton);
            TravelStatusText.Text = $"Traveler {name} created!";
            TravelStatusText.Foreground = Brushes.Purple;
            TravelStatusText.Visibility = Visibility.Visible;
            travelerCreatedSuccessfully = true;
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
                    MapStatusText.Foreground = Brushes.Purple;
                    MapStatusText.Visibility = Visibility.Visible;
                    LoadMapButton.BorderBrush = defaultBorder;
                }
                catch (FileNotFoundException)
                {
                    ShowError(LoadMapButton, "Map file not found!");
                    map = null;
                }
                catch (Exception)
                {
                    ShowError(LoadMapButton, "Invalid format!");
                    map = null;
                }
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {

            if (!travelerCreatedSuccessfully)
            {
                ShowError(CreateTravelerButton, "Create traveler first!");
                return;
            }

            bool validName = NameTextBox.Validate();
            bool validLocation = LocationTextBox.Validate();
            bool validDestination = DestinationTextBox.Validate();

            if (map == null)
            {
                ShowError(LoadMapButton, "Map file not found!");
                return;
            }

            if (validLocation && !map.ContainsCity(LocationTextBox.Text))
            {
                LocationTextBox.ShowTBError("Location not in map!");
                validLocation = false;
            }
            if (validDestination && !map.ContainsCity(DestinationTextBox.Text))
            {
                DestinationTextBox.ShowTBError("Destination not in map!");
                validDestination = false;
            }

            if (!validName || !validLocation || !validDestination || map == null)
            {
                return;
            }

            HideError(LoadMapButton);
            HideError(NameTextBox);
            HideError(LocationTextBox);
            HideError(DestinationTextBox);

            Traveler traveler = new Traveler(NameTextBox.Text);
            traveler.SetLocation(LocationTextBox.Text);
            traveler.PlanRouteTo(DestinationTextBox.Text, map);
            Page3 page3 = new Page3(traveler, map);
            this.NavigationService.Navigate(page3);

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasName = !string.IsNullOrWhiteSpace(NameTextBox.Text);
            bool hasLocation = !string.IsNullOrWhiteSpace(LocationTextBox.Text);
            bool hasDestination = !string.IsNullOrWhiteSpace(DestinationTextBox.Text);

            if (hasName || hasLocation || hasDestination || map != null)
            {
                Window mainWindow = Window.GetWindow(this);

                if (mainWindow != null)
                {
                    mainWindow.IsEnabled = false;
                    mainWindow.Opacity = 0.5;
                }

                ReturnWarningWindow warningWindow = new ReturnWarningWindow();
                warningWindow.ShowDialog();

                if (mainWindow != null)
                {
                    mainWindow.Opacity = 1;
                    mainWindow.IsEnabled = true;
                }

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
