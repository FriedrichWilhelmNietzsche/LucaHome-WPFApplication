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
        private readonly LibraryService _libraryService;
        private readonly MapContentService _mapContentService;
        private readonly MenuService _menuService;
        private readonly MovieService _movieService;
        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly ScheduleService _scheduleService;
        private readonly SecurityService _securityService;
        private readonly ShoppingListService _shoppingListService;
        private readonly TemperatureService _temperatureService;
        private readonly WirelessSocketService _wirelessSocketService;

        public BootPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _birthdayService = BirthdayService.Instance;
            _coinService = CoinService.Instance;
            _libraryService = LibraryService.Instance;
            _mapContentService = MapContentService.Instance;
            _menuService = MenuService.Instance;
            _movieService = MovieService.Instance;
            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _scheduleService = ScheduleService.Instance;
            _securityService = SecurityService.Instance;
            _shoppingListService = ShoppingListService.Instance;
            _temperatureService = TemperatureService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _birthdayService.OnBirthdayDownloadFinished += _birthdayDownloadFinished;
            _coinService.OnCoinConversionDownloadFinished += _onCoinConversionDownloadFinished;
            _libraryService.OnMagazinListDownloadFinished += _onMagazinListDownloadFinished;
            _menuService.OnListedMenuDownloadFinished += _onListedMenuDownloadFinished;
            _movieService.OnMovieDownloadFinished += _movieDownloadFinished;
            _openWeatherService.OnCurrentWeatherDownloadFinished += _currentWeatherDownloadFinished;
            _openWeatherService.OnForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished;
            _securityService.OnSecurityDownloadFinished += _onSecurityDownloadFinished;
            _shoppingListService.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished;
            _wirelessSocketService.OnWirelessSocketDownloadFinished += _wirelessSocketDownloadFinished;

            _birthdayService.LoadBirthdayList();
            _coinService.LoadCoinConversionList();
            _libraryService.LoadMagazinList();
            _menuService.LoadListedMenuList();
            _movieService.LoadMovieList();
            _openWeatherService.LoadCurrentWeather();
            _openWeatherService.LoadForecastModel();
            _securityService.LoadSecurity();
            _shoppingListService.LoadShoppingList();
            _wirelessSocketService.LoadWirelessSocketList();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _downloadCount = 0;

            _birthdayService.OnBirthdayDownloadFinished -= _birthdayDownloadFinished;
            _coinService.OnCoinConversionDownloadFinished -= _onCoinConversionDownloadFinished;
            _coinService.OnCoinDownloadFinished -= _onCoinDownloadFinished;
            _libraryService.OnMagazinListDownloadFinished -= _onMagazinListDownloadFinished;
            _mapContentService.OnMapContentDownloadFinished -= _mapContentDownloadFinished;
            _menuService.OnListedMenuDownloadFinished -= _onListedMenuDownloadFinished;
            _menuService.OnMenuDownloadFinished -= _onMenuDownloadFinished;
            _movieService.OnMovieDownloadFinished -= _movieDownloadFinished;
            _openWeatherService.OnCurrentWeatherDownloadFinished -= _currentWeatherDownloadFinished;
            _openWeatherService.OnForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
            _scheduleService.OnScheduleDownloadFinished -= _scheduleDownloadFinished;
            _securityService.OnSecurityDownloadFinished -= _onSecurityDownloadFinished;
            _shoppingListService.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
            _temperatureService.OnTemperatureDownloadFinished -= _temperatureDownloadFinished;
            _wirelessSocketService.OnWirelessSocketDownloadFinished -= _wirelessSocketDownloadFinished;
        }

        private void _birthdayDownloadFinished(IList<BirthdayDto> birthdayList, bool success, string response)
        {
            _logger.Debug(string.Format("_birthdayDownloadFinished with model {0} was successful: {1}", birthdayList, success));
            checkDownloadCount();
        }

        private void _currentWeatherDownloadFinished(WeatherModel currentWeather, bool success)
        {
            _logger.Debug(string.Format("_currentWeatherDownloadFinished with model {0} was successful: {1}", currentWeather, success));
            checkDownloadCount();

            // Start download of temperatures after downloading current weather
            _temperatureService.OnTemperatureDownloadFinished += _temperatureDownloadFinished;
            _temperatureService.LoadTemperatureList();
        }

        private void _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            _logger.Debug(string.Format("_forecastWeatherDownloadFinished with model {0} was successful: {1}", forecastWeather, success));
            checkDownloadCount();
        }

        private void _mapContentDownloadFinished(IList<MapContentDto> mapContentList, bool success, string response)
        {
            _logger.Debug(string.Format("_mapContentDownloadFinished with model {0} was successful: {1}", mapContentList, success));
            checkDownloadCount();
        }

        private void _movieDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            _logger.Debug(string.Format("_movieDownloadFinished with model {0} was successful: {1}", movieList, success));
            checkDownloadCount();
        }

        private void _onCoinConversionDownloadFinished(IList<KeyValuePair<string, double>> coinConversionList, bool success, string response)
        {
            _logger.Debug(string.Format("_onCoinConversionDownloadFinished with model {0} was successful: {1}", coinConversionList, success));
            checkDownloadCount();

            // Start download of coins after downloading coin conversion
            _coinService.OnCoinDownloadFinished += _onCoinDownloadFinished;
            _coinService.LoadCoinList();
        }

        private void _onCoinDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            _logger.Debug(string.Format("_onCoinDownloadFinished with model {0} was successful: {1}", coinList, success));
            checkDownloadCount();
        }

        private void _onListedMenuDownloadFinished(IList<ListedMenuDto> listedMenuList, bool success, string response)
        {
            _logger.Debug(string.Format("_onListedMenuDownloadFinished with model {0} was successful: {1}", listedMenuList, success));
            checkDownloadCount();

            // Start download of menu after downloading listed menu entries
            _menuService.OnMenuDownloadFinished += _onMenuDownloadFinished;
            _menuService.LoadMenuList();
        }

        private void _onMagazinListDownloadFinished(IList<MagazinDirDto> magazinList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMagazinListDownloadFinished with model {0} was successful: {1}", magazinList, success));
            checkDownloadCount();
        }

        private void _onMenuDownloadFinished(IList<MenuDto> menuList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMenuDownloadFinished with model {0} was successful: {1}", menuList, success));
            checkDownloadCount();
        }

        private void _onSecurityDownloadFinished(SecurityDto security, bool success, string response)
        {
            _logger.Debug(string.Format("_onSecurityDownloadFinished with model {0} was successful: {1}", security, success));
            checkDownloadCount();
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            _logger.Debug(string.Format("_onShoppingListDownloadFinished with model {0} was successful: {1}", shoppingList, success));
            checkDownloadCount();
        }

        private void _scheduleDownloadFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            _logger.Debug(string.Format("_scheduleDownloadFinished with model {0} was successful: {1}", scheduleList, success));
            checkDownloadCount();

            // Start download of mapcontent after downloading wirelesssockets AND schedules
            _mapContentService.OnMapContentDownloadFinished += _mapContentDownloadFinished;
            _mapContentService.LoadMapContentList();
        }

        private void _temperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success, string response)
        {
            _logger.Debug(string.Format("_temperatureDownloadFinished with model {0} was successful: {1}", temperatureList, success));
            checkDownloadCount();
        }

        private void _wirelessSocketDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            _logger.Debug(string.Format("_wirelessSocketDownloadFinished with model {0} was successful: {1}", wirelessSocketList, success));
            checkDownloadCount();

            // Start download of schedules after downloading wirelesssockets
            _scheduleService.OnScheduleDownloadFinished += _scheduleDownloadFinished;
            _scheduleService.LoadScheduleList();
        }

        private void checkDownloadCount()
        {
            _downloadCount++;
            _logger.Debug(string.Format("checkDownloadCount: Download {0}/{1}", _downloadCount, Constants.DOWNLOAD_STEPS));

            if (_downloadCount >= Constants.DOWNLOAD_STEPS)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { _navigationService.Navigate(new MainPage(_navigationService)); }));
            }
        }
    }
}
