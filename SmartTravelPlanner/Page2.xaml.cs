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
    public partial class Page2 : Page
    {
        private Traveler traveler;
        private CityGraph map;

        public Page2(Traveler traveler, CityGraph map)
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
            TotalDistanceTextBlock.Text = $"Total Distance: {totalDistance} km";
 
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
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
