using Common.Common;
using Common.Tools;
using OpenWeather.Models;
using OpenWeather.Service;
using System;
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
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;

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
            
            _openWeatherService.OnForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished;

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
            
            _openWeatherService.OnForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
        }

        private void setList()
        {
            _logger.Debug("setList");

            WeatherList.Items.Clear();

            foreach (ForecastPartModel entry in _openWeatherService.ForecastWeather.List)
            {
                WeatherList.Items.Add(entry);
            }

            Wallpaper.ImageWallpaperSource = _openWeatherService.ForecastWeather.Wallpaper;
        }

        private void _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            _logger.Debug(string.Format("_forecastWeatherDownloadFinished with model {0} was successful: {1}", forecastWeather, success));
            Application.Current.Dispatcher.Invoke(new Action(() => { setList(); }));
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _openWeatherService.LoadForecastModel();
        }
    }
}
