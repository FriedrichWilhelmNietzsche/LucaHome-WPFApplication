using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using OpenWeather.Models;
using OpenWeather.Service;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class BootPage : Page
    {
        private const string TAG = "BootPage";
        private readonly Logger _logger;
        
        private int _downloadCount = 0;

        private readonly AppSettingsService _appSettingsService;
        private readonly BirthdayService _birthdayService;
        private readonly MovieService _movieService;
        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly TemperatureService _temperatureService;
        private readonly WirelessSocketService _wirelessSocketService;

        public BootPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _appSettingsService = AppSettingsService.Instance;
            _birthdayService = BirthdayService.Instance;
            _movieService = MovieService.Instance;
            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _temperatureService = TemperatureService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            if (!_appSettingsService.EnteredUserData)
            {
                _logger.Debug("Not yet entered an user! Navigating to LoginPage");
                _navigationService.Navigate(new LoginPage(_navigationService));
            }
            else
            {
                _birthdayService.OnBirthdayDownloadFinished += _birthdayDownloadFinished;

                _openWeatherService.OnCurrentWeatherDownloadFinished += _currentWeatherDownloadFinished;
                _openWeatherService.OnForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished;

                _wirelessSocketService.OnWirelessSocketDownloadFinished += _wirelessSocketDownloadFinished;

                _birthdayService.LoadBirthdayList();

                _openWeatherService.LoadCurrentWeather();
                _openWeatherService.LoadForecastModel();

                _wirelessSocketService.LoadWirelessSocketList();
            }
        }

        private void _birthdayDownloadFinished(IList<BirthdayDto> birthdayList, bool success)
        {
            _logger.Debug(string.Format("_birthdayDownloadFinished with model {0} was successful: {1}", birthdayList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _currentWeatherDownloadFinished(WeatherModel currentWeather, bool success)
        {
            _logger.Debug(string.Format("_currentWeatherDownloadFinished with model {0} was successful: {1}", currentWeather, success));
            _downloadCount++;
            checkDownloadCount();

            // Start download of temperatures after downloading current weather
            _temperatureService.OnTemperatureDownloadFinished += _temperatureDownloadFinished;
            _temperatureService.LoadTemperatureList();
        }

        private void _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            _logger.Debug(string.Format("_forecastWeatherDownloadFinished with model {0} was successful: {1}", forecastWeather, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _movieDownloadFinished(IList<MovieDto> movieList, bool success)
        {
            _logger.Debug(string.Format("_movieDownloadFinished with model {0} was successful: {1}", movieList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _temperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success)
        {
            _logger.Debug(string.Format("_temperatureDownloadFinished with model {0} was successful: {1}", temperatureList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _wirelessSocketDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success)
        {
            _logger.Debug(string.Format("_wirelessSocketDownloadFinished with model {0} was successful: {1}", wirelessSocketList, success));
            _downloadCount++;
            checkDownloadCount();

            // Start download of movies after downloading wirelesssockets
            _movieService.OnMovieDownloadFinished += _movieDownloadFinished;
            _movieService.LoadMovieList();
        }

        private void checkDownloadCount()
        {
            _logger.Debug("checkDownloadCount");

            if (_downloadCount >= Constants.DOWNLOAD_STEPS)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { _navigationService.Navigate(new MainPage(_navigationService)); }));
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _downloadCount = 0;

            _birthdayService.OnBirthdayDownloadFinished -= _birthdayDownloadFinished;

            _movieService.OnMovieDownloadFinished -= _movieDownloadFinished;

            _openWeatherService.OnCurrentWeatherDownloadFinished -= _currentWeatherDownloadFinished;
            _openWeatherService.OnForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;

            _temperatureService.OnTemperatureDownloadFinished -= _temperatureDownloadFinished;

            _wirelessSocketService.OnWirelessSocketDownloadFinished -= _wirelessSocketDownloadFinished;
        }
    }
}
