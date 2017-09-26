using Common.Common;
using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using OpenWeather.Service;
using Data.Services;
using Common.Dto;
using System.Collections.Generic;
using OpenWeather.Models;
using System.Deployment.Application;

namespace LucaHome.Pages
{
    public partial class MainPage : Page
    {
        private const string TAG = "MainPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly TemperatureService _temperatureService;

        //private string _temperatureBottomTitle;
        //private string _temperatureBottomData;

        private BirthdayAddPage _birthdayAddPage;
        private BirthdayPage _birthdayPage;
        //private BirthdayUpdatePage _birthdayUpdatePage;

        private CoinAddPage _coinAddPage;
        private CoinPage _coinPage;
        //private CoinUpdatePage _coinUpdatePage;

        private MagazinPage _magazinPage;
        //private MagazinListPage _magazinListPage;

        private MapPage _mapPage;

        private MenuPage _menuPage;
        //private MenuUpdatePage _menuUpdatePage;

        private MoviePage _moviePage;
        //private MovieUpdatePage _movieUpdatePage;

        private NovelPage _novelPage;
        //private NovelListPage _novelListPage;

        private ScheduleAddPage _scheduleAddPage;
        private SchedulePage _schedulePage;
        //private ScheduleUpdatePage _scheduleUpdatePage;

        private SecurityPage _securityPage;

        private SeriesPage _seriesPage;
        //private SeriesListPage _seriesListPage;

        private SettingsPage _settingsPage;

        private ShoppingEntryAddPage _shoppingEntryAddPage;
        private ShoppingListPage _shoppingListPage;

        private SpecialicedBookPage _specialicedBookPage;

        private TemperaturePage _temperaturePage;
        private WeatherPage _weatherPage;

        private WirelessSocketAddPage _wirelessSocketAddPage;
        private WirelessSocketPage _wirelessSocketPage;
        //private WirelessSocketUpdatePage _wirelessSocketUpdatePage;

        public MainPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _temperatureService = TemperatureService.Instance;

            InitializeComponent();

            BirthdayCard.ButtonAddCommand = new DelegateCommand(navigateToBirthdayAdd);
            CoinsCard.ButtonAddCommand = new DelegateCommand(navigateToCoinAdd);
            ScheduleCard.ButtonAddCommand = new DelegateCommand(navigateToScheduleAdd);
            ScheduleCard.ButtonMapCommand = new DelegateCommand(navigateToMap);
            ShoppingListCard.ButtonAddCommand = new DelegateCommand(navigateToShoppingAdd);
            SocketCard.ButtonAddCommand = new DelegateCommand(navigateToSocketAdd);
            SocketCard.ButtonMapCommand = new DelegateCommand(navigateToMap);
            TemperatureCard.ButtonMapCommand = new DelegateCommand(navigateToMap);
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.RemoveBackEntry();

            // Reset page variables
            _birthdayAddPage = null;
            _birthdayPage = null;
            //_birthdayUpdatePage = null;
            _coinAddPage = null;
            _coinPage = null;
            //_coinUpdatePage = null;
            _magazinPage = null;
            //_magazinListPage = null;
            _mapPage = null;
            _menuPage = null;
            //_menuUpdatePage = null;
            _moviePage = null;
            //_movieUpdatePage = null;
            _novelPage = null;
            //_novelListPage = null;
            _scheduleAddPage = null;
            _schedulePage = null;
            //_scheduleUpdatePage = null;
            _securityPage = null;
            _seriesPage = null;
            //_seriesListPage = null;
            _settingsPage = null;
            _shoppingEntryAddPage = null;
            _shoppingListPage = null;
            _specialicedBookPage = null;
            _temperaturePage = null;
            _weatherPage = null;
            _wirelessSocketAddPage = null;
            _wirelessSocketPage = null;
            //_wirelessSocketUpdatePage = null;

            // TODO Create and fix binding
            WeatherCard.BottomTitleText.Text = _openWeatherService.City;
            WeatherCard.BottomDataText.Text = string.Format("{0}°C", _openWeatherService.CurrentWeather.Temperature);
            _openWeatherService.OnCurrentWeatherDownloadFinished += _onCurrentWeatherDownloadFinished;

            // TODO Create and fix binding
            if (_temperatureService.TemperatureList.Count > 0)
            {
                TemperatureCard.BottomTitleText.Text = _temperatureService.TemperatureList[0]?.Area;
                TemperatureCard.BottomDataText.Text = _temperatureService.TemperatureList[0]?.TemperatureString;
            }
            _temperatureService.OnTemperatureDownloadFinished += _onTemperatureDownloadFinished;

            readApplicationVersion();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} with arguments {1}", sender, routedEventArgs));
            _openWeatherService.OnCurrentWeatherDownloadFinished -= _onCurrentWeatherDownloadFinished;
            _temperatureService.OnTemperatureDownloadFinished -= _onTemperatureDownloadFinished;
        }

        private void _onCurrentWeatherDownloadFinished(WeatherModel currentWeather, bool success)
        {
            _logger.Debug(string.Format("_onCurrentWeatherDownloadFinished with model {0} was successful: {1}", currentWeather, success));
            WeatherCard.BottomTitleText.Text = _openWeatherService.City;
            WeatherCard.BottomDataText.Text = string.Format("{0}°C", _openWeatherService.CurrentWeather.Temperature);
        }

        private void _onTemperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success, string response)
        {
            _logger.Debug(string.Format("_onTemperatureDownloadFinished with model {0} was successful: {1}", temperatureList, success));
            TemperatureCard.BottomTitleText.Text = _temperatureService.TemperatureList[0]?.Area;
            TemperatureCard.BottomDataText.Text = _temperatureService.TemperatureList[0]?.TemperatureString;
        }

        private void SocketCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("SocketCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _wirelessSocketPage = new WirelessSocketPage(_navigationService);
            _navigationService.Navigate(_wirelessSocketPage);
        }

        private void navigateToSocketAdd()
        {
            _logger.Debug("navigateToSocketAdd");
            _wirelessSocketAddPage = new WirelessSocketAddPage(_navigationService);
            _navigationService.Navigate(_wirelessSocketAddPage);
        }

        private void navigateToMap()
        {
            _logger.Debug("navigateToMap");
            _mapPage = new MapPage(_navigationService);
            _navigationService.Navigate(_mapPage);
        }

        private void ScheduleCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("ScheduleCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _schedulePage = new SchedulePage(_navigationService);
            _navigationService.Navigate(_schedulePage);
        }

        private void navigateToScheduleAdd()
        {
            _logger.Debug("navigateToScheduleAdd");
            _scheduleAddPage = new ScheduleAddPage(_navigationService);
            _navigationService.Navigate(_scheduleAddPage);
        }

        private void WeatherCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("WeatherCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _weatherPage = new WeatherPage(_navigationService);
            _navigationService.Navigate(_weatherPage);
        }

        private void TemperatureCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("TemperatureCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _temperaturePage = new TemperaturePage(_navigationService);
            _navigationService.Navigate(_temperaturePage);
        }

        private void MenuCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("MenuCard_MouseUp: Received click of sender {0} with arguments {1}", sender, mouseButtonEventArgs));
            _menuPage = new MenuPage(_navigationService);
            _navigationService.Navigate(_menuPage);
        }

        private void ShoppingCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("ShoppingCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _shoppingListPage = new ShoppingListPage(_navigationService);
            _navigationService.Navigate(_shoppingListPage);
        }

        private void navigateToShoppingAdd()
        {
            _logger.Debug("navigateToShoppingAdd");
            _shoppingEntryAddPage = new ShoppingEntryAddPage(_navigationService);
            _navigationService.Navigate(_shoppingEntryAddPage);
        }

        private void BirthdayCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("BirthdayCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _birthdayPage = new BirthdayPage(_navigationService);
            _navigationService.Navigate(_birthdayPage);
        }

        private void navigateToBirthdayAdd()
        {
            _logger.Debug("navigateToBirthdayAdd");
            _birthdayAddPage = new BirthdayAddPage(_navigationService);
            _navigationService.Navigate(_birthdayAddPage);
        }

        private void CoinsCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("CoinsCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _coinPage = new CoinPage(_navigationService);
            _navigationService.Navigate(_coinPage);
        }

        private void navigateToCoinAdd()
        {
            _logger.Debug("navigateToCoinAdd");
            _coinAddPage = new CoinAddPage(_navigationService);
            _navigationService.Navigate(_coinAddPage);
        }

        private void MovieCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("MovieCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _moviePage = new MoviePage(_navigationService);
            _navigationService.Navigate(_moviePage);
        }

        private void SeriesCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("SeriesCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _seriesPage = new SeriesPage(_navigationService);
            _navigationService.Navigate(_seriesPage);
        }

        private void MagazinCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("MagazinCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _magazinPage = new MagazinPage(_navigationService);
            _navigationService.Navigate(_magazinPage);
        }

        private void NovelCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("NovelCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _novelPage = new NovelPage(_navigationService);
            _navigationService.Navigate(_novelPage);
        }

        private void SpecialicedBookCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("SpecialicedBookCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _specialicedBookPage = new SpecialicedBookPage(_navigationService);
            _navigationService.Navigate(_specialicedBookPage);
        }

        private void SecurityCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("SecurityCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _securityPage = new SecurityPage(_navigationService);
            _navigationService.Navigate(_securityPage);
        }

        private void SettingsCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("SettingsCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _settingsPage = new SettingsPage(_navigationService);
            _navigationService.Navigate(_settingsPage);
        }

        private void readApplicationVersion()
        {
            string version = "";

            try
            {
                version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (InvalidDeploymentException exception)
            {
                _logger.Error(exception.Message);
            }

            VersionTextBlock.Text = version;
        }
    }
}
