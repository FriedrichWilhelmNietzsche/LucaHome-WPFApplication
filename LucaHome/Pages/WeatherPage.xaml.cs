using Common.Common;
using Common.Tools;
using OpenWeather.Models;
using OpenWeather.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 */

namespace LucaHome.Pages
{
    public partial class WeatherPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "WeatherPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;

        private Uri _wallpaperSource = new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_dummy.png", UriKind.Relative);
        private string _weatherSearchKey = string.Empty;
        private IList<ForecastPartModel> _forecasteList = new List<ForecastPartModel>();

        public WeatherPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Uri WallpaperSource
        {
            get
            {
                return _wallpaperSource;
            }
            set
            {
                _wallpaperSource = value;
                Wallpaper.ImageWallpaperSource = _wallpaperSource;
                OnPropertyChanged("WallpaperSource");
            }
        }

        public string WeatherSearchKey
        {
            get
            {
                return _weatherSearchKey;
            }
            set
            {
                _weatherSearchKey = value;
                OnPropertyChanged("WeatherSearchKey");

                if (_weatherSearchKey != string.Empty)
                {
                    ForecastModel foundForecastModel = _openWeatherService.FoundForecastEntries(_weatherSearchKey);
                    WallpaperSource = foundForecastModel.Wallpaper;
                    WeatherList = foundForecastModel.List;
                }
                else
                {
                    WallpaperSource = _openWeatherService.ForecastWeather.Wallpaper;
                    WeatherList = _openWeatherService.ForecastWeather.List;
                }
            }
        }

        public IList<ForecastPartModel> WeatherList
        {
            get
            {
                return _forecasteList;
            }
            set
            {
                _forecasteList = value;
                OnPropertyChanged("WeatherList");
            }
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

            WallpaperSource = _openWeatherService.ForecastWeather.Wallpaper;
            WeatherList = _openWeatherService.ForecastWeather.List;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _openWeatherService.OnForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
        }

        private void _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            _logger.Debug(string.Format("_forecastWeatherDownloadFinished with model {0} was successful: {1}", forecastWeather, success));
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                WallpaperSource = _openWeatherService.ForecastWeather.Wallpaper;
                WeatherList = _openWeatherService.ForecastWeather.List;
            }));
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
