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

                case Button btn when btn == LoadMapButton:
                    MapStatusText.Text = message;
                    MapStatusText.Foreground = Brushes.Red;
                    MapStatusText.Visibility = Visibility.Visible;
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

                case Button btn when btn == LoadMapButton:
                    MapStatusText.Visibility = Visibility.Collapsed;
                    btn.BorderBrush = defaultBorder;
                    break;
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
                    MapStatusText.Foreground = Brushes.Black;
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
                LocationTextBox.ShowTBError("Location not found on map!");
                validLocation = false;
            }
            if (validDestination && !map.ContainsCity(DestinationTextBox.Text))
            {
                DestinationTextBox.ShowTBError("Destination not found on map!");
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
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
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
