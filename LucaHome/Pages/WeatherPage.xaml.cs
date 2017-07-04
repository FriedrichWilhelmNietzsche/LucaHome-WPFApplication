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
        private OpenWeatherService _openWeatherService;

        public WeatherPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;

            InitializeComponent();
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            
            _openWeatherService.ForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished;

            if (_openWeatherService.ForecastWeather == null)
            {
                _openWeatherService.LoadForecastModel();
                return;
            }

            setList();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            
            _openWeatherService.ForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
        }

        private void setList()
        {
            _logger.Debug("setList");

            foreach (ForecastPartModel entry in _openWeatherService.ForecastWeather.List)
            {
                WeatherList.Items.Add(entry);
            }
        }

        private string _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            _logger.Debug(string.Format("_forecastWeatherDownloadFinished with model {0} was successful: {1}", forecastWeather, success));
            setList();
            return success ? "1" : "0";
        }
    }
}
