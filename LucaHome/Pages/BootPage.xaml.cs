using Common.Common;
using Common.Tools;
using OpenWeather.Models;
using OpenWeather.Service;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class BootPage : Page
    {
        private const string TAG = "BootPage";
        private Logger _logger;

        private const int MAX_DOWNLOAD_COUNT = 2;
        private int _downloadCount = 0;

        private OpenWeatherService _openWeatherService;
        private NavigationService _navigationService;

        public BootPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _openWeatherService = OpenWeatherService.Instance;
            _navigationService = navigationService;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _openWeatherService.CurrentWeatherDownloadFinished += _currentWeatherDownloadFinished;
            _openWeatherService.ForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished;

            _openWeatherService.LoadCurrentWeather();
            _openWeatherService.LoadForecastModel();
        }

        private string _currentWeatherDownloadFinished(WeatherModel currentWeather, bool success)
        {
            _logger.Debug(string.Format("_currentWeatherDownloadFinished with model {0} was successful: {1}", currentWeather, success));
            _downloadCount++;
            checkDownloadCount();
            return success ? "1" : "0";
        }

        private string _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            _logger.Debug(string.Format("_forecastWeatherDownloadFinished with model {0} was successful: {1}", forecastWeather, success));
            _downloadCount++;
            checkDownloadCount();
            return success ? "1" : "0";
        }

        private void checkDownloadCount()
        {
            _logger.Debug("checkDownloadCount");

            if (_downloadCount >= MAX_DOWNLOAD_COUNT)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { _navigationService.Navigate(new MainPage(_navigationService)); }));
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _openWeatherService.CurrentWeatherDownloadFinished -= _currentWeatherDownloadFinished;
            _openWeatherService.ForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
        }
    }
}
