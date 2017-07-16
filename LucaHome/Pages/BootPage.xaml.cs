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

        private readonly BirthdayService _birthdayService;
        private readonly CoinService _coinService;
        private readonly MapContentService _mapContentService;
        private readonly MenuService _menuService;
        private readonly MovieService _movieService;
        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly ScheduleService _scheduleService;
        private readonly ShoppingListService _shoppingListService;
        private readonly TemperatureService _temperatureService;
        private readonly WirelessSocketService _wirelessSocketService;

        public BootPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _birthdayService = BirthdayService.Instance;
            _coinService = CoinService.Instance;
            _mapContentService = MapContentService.Instance;
            _menuService = MenuService.Instance;
            _movieService = MovieService.Instance;
            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _scheduleService = ScheduleService.Instance;
            _shoppingListService = ShoppingListService.Instance;
            _temperatureService = TemperatureService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _birthdayService.OnBirthdayDownloadFinished += _birthdayDownloadFinished;

            _coinService.OnCoinDownloadFinished += _onCoinDownloadFinished;

            _menuService.OnMenuDownloadFinished += _onMenuDownloadFinished;

            _openWeatherService.OnCurrentWeatherDownloadFinished += _currentWeatherDownloadFinished;
            _openWeatherService.OnForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished;

            _shoppingListService.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished;

            _wirelessSocketService.OnWirelessSocketDownloadFinished += _wirelessSocketDownloadFinished;

            _birthdayService.LoadBirthdayList();

            _coinService.LoadCoinList();

            _menuService.LoadMenuList();

            _openWeatherService.LoadCurrentWeather();
            _openWeatherService.LoadForecastModel();

            _shoppingListService.LoadShoppingList();

            _wirelessSocketService.LoadWirelessSocketList();
        }

        private void _birthdayDownloadFinished(IList<BirthdayDto> birthdayList, bool success, string response)
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

        private void _mapContentDownloadFinished(IList<MapContentDto> mapContentList, bool success, string response)
        {
            _logger.Debug(string.Format("_mapContentDownloadFinished with model {0} was successful: {1}", mapContentList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _movieDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            _logger.Debug(string.Format("_movieDownloadFinished with model {0} was successful: {1}", movieList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _onCoinDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            _logger.Debug(string.Format("_onCoinDownloadFinished with model {0} was successful: {1}", coinList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _onMenuDownloadFinished(IList<MenuDto> menuList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMenuDownloadFinished with model {0} was successful: {1}", menuList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            _logger.Debug(string.Format("_onShoppingListDownloadFinished with model {0} was successful: {1}", shoppingList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _scheduleDownloadFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            _logger.Debug(string.Format("_scheduleDownloadFinished with model {0} was successful: {1}", scheduleList, success));
            _downloadCount++;
            checkDownloadCount();

            // Start download of mapcontent after downloading wirelesssockets AND schedules
            _mapContentService.OnMapContentDownloadFinished += _mapContentDownloadFinished;
            _mapContentService.LoadMapContentList();
        }

        private void _temperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success, string response)
        {
            _logger.Debug(string.Format("_temperatureDownloadFinished with model {0} was successful: {1}", temperatureList, success));
            _downloadCount++;
            checkDownloadCount();
        }

        private void _wirelessSocketDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            _logger.Debug(string.Format("_wirelessSocketDownloadFinished with model {0} was successful: {1}", wirelessSocketList, success));
            _downloadCount++;
            checkDownloadCount();

            // Start download of movies after downloading wirelesssockets
            _movieService.OnMovieDownloadFinished += _movieDownloadFinished;
            _movieService.LoadMovieList();

            // Start download of schedules after downloading wirelesssockets
            _scheduleService.OnScheduleDownloadFinished += _scheduleDownloadFinished;
            _scheduleService.LoadScheduleList();
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
            _coinService.OnCoinDownloadFinished -= _onCoinDownloadFinished;
            _menuService.OnMenuDownloadFinished -= _onMenuDownloadFinished;
            _movieService.OnMovieDownloadFinished -= _movieDownloadFinished;
            _openWeatherService.OnCurrentWeatherDownloadFinished -= _currentWeatherDownloadFinished;
            _openWeatherService.OnForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
            _scheduleService.OnScheduleDownloadFinished -= _scheduleDownloadFinished;
            _shoppingListService.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
            _temperatureService.OnTemperatureDownloadFinished -= _temperatureDownloadFinished;
            _wirelessSocketService.OnWirelessSocketDownloadFinished -= _wirelessSocketDownloadFinished;
        }
    }
}
