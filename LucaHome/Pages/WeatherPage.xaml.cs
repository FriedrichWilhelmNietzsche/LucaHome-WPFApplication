using Common.Common;
using Common.Tools;
using OpenWeather.Models;
using OpenWeather.Service;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 */

namespace LucaHome.Pages
{
    public partial class WeatherPage : Page
    {
        private const string TAG = "WeatherPage";
        private Logger _logger;

        private NavigationService _navigationService;

        public WeatherPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);
            _navigationService = navigationService;

            InitializeComponent();
        }

        // TODO Only for testing! Remove later!
        private void WeatherPage_Loaded(object sender, RoutedEventArgs e)
        {
            OpenWeatherService openWeatherService = new OpenWeatherService("Munich, DE");
            ForecastModel forecast = openWeatherService.GetForecastModel();

            foreach(ForecastPartModel entry in forecast.List)
            {
                WeatherList.Items.Add(entry);
            }
        }
    }
}
