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

        private readonly NavigationService _navigationService;

        private Uri _wallpaperSource = new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_dummy.png", UriKind.Relative);
        private string _weatherSearchKey = string.Empty;

        public WeatherPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

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
                    ForecastModel foundForecastModel = OpenWeatherService.Instance.FoundForecastEntries(_weatherSearchKey);
                    WallpaperSource = foundForecastModel.Wallpaper;
                    WeatherList = foundForecastModel.List;
                }
                else
                {
                    WallpaperSource = OpenWeatherService.Instance.ForecastWeather.Wallpaper;
                    WeatherList = OpenWeatherService.Instance.ForecastWeather.List;
                }
            }
        }

        public IList<ForecastPartModel> WeatherList
        {
            get
            {
                return OpenWeatherService.Instance.ForecastWeather.List;
            }
            set
            {
                OnPropertyChanged("WeatherList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenWeatherService.Instance.OnForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished;

            if (OpenWeatherService.Instance.ForecastWeather == null)
            {
                OpenWeatherService.Instance.LoadForecastModel();
                return;
            }

            WallpaperSource = OpenWeatherService.Instance.ForecastWeather.Wallpaper;
            WeatherList = OpenWeatherService.Instance.ForecastWeather.List;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenWeatherService.Instance.OnForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
        }

        private void _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                WallpaperSource = OpenWeatherService.Instance.ForecastWeather.Wallpaper;
                WeatherList = OpenWeatherService.Instance.ForecastWeather.List;
            }));
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenWeatherService.Instance.LoadForecastModel();
        }
    }
}
