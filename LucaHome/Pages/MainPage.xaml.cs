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

        private readonly NavigationService _navigationService;

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

        private MediaMirrorPage _mediaMirrorPage;

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

        private WirelessSwitchAddPage _wirelessSwitchAddPage;
        private WirelessSwitchPage _wirelessSwitchPage;
        //private WirelessSwitchUpdatePage _wirelessSwitchUpdatePage;

        public MainPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();

            BirthdayCard.ButtonAddCommand = new DelegateCommand(navigateToBirthdayAdd);
            CoinsCard.ButtonAddCommand = new DelegateCommand(navigateToCoinAdd);
            ScheduleCard.ButtonAddCommand = new DelegateCommand(navigateToScheduleAdd);
            ScheduleCard.ButtonMapCommand = new DelegateCommand(navigateToMap);
            ShoppingListCard.ButtonAddCommand = new DelegateCommand(navigateToShoppingAdd);
            SocketCard.ButtonAddCommand = new DelegateCommand(navigateToSocketAdd);
            SocketCard.ButtonMapCommand = new DelegateCommand(navigateToMap);
            SwitchCard.ButtonAddCommand = new DelegateCommand(navigateToSwitchAdd);
            TemperatureCard.ButtonMapCommand = new DelegateCommand(navigateToMap);
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
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
            _wirelessSwitchAddPage = null;
            _wirelessSwitchPage = null;
            //_wirelessSwitchUpdatePage = null;

            // TODO Create and fix binding
            WeatherCard.BottomTitleText.Text = OpenWeatherService.Instance.City;
            WeatherCard.BottomDataText.Text = string.Format("{0}°C", OpenWeatherService.Instance.CurrentWeather.Temperature);
            OpenWeatherService.Instance.OnCurrentWeatherDownloadFinished += _onCurrentWeatherDownloadFinished;

            // TODO Create and fix binding
            if (TemperatureService.Instance.TemperatureList.Count > 0)
            {
                TemperatureCard.BottomTitleText.Text = TemperatureService.Instance.TemperatureList[0]?.Area;
                TemperatureCard.BottomDataText.Text = TemperatureService.Instance.TemperatureList[0]?.TemperatureString;
            }
            TemperatureService.Instance.OnTemperatureDownloadFinished += _onTemperatureDownloadFinished;

            readApplicationVersion();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenWeatherService.Instance.OnCurrentWeatherDownloadFinished -= _onCurrentWeatherDownloadFinished;
            TemperatureService.Instance.OnTemperatureDownloadFinished -= _onTemperatureDownloadFinished;
        }

        private void _onCurrentWeatherDownloadFinished(WeatherModel currentWeather, bool success)
        {
            WeatherCard.BottomTitleText.Text = OpenWeatherService.Instance.City;
            WeatherCard.BottomDataText.Text = string.Format("{0}°C", OpenWeatherService.Instance.CurrentWeather.Temperature);
        }

        private void _onTemperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success, string response)
        {
            TemperatureCard.BottomTitleText.Text = TemperatureService.Instance.TemperatureList[0]?.Area;
            TemperatureCard.BottomDataText.Text = TemperatureService.Instance.TemperatureList[0]?.TemperatureString;
        }

        private void SocketCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _wirelessSocketPage = new WirelessSocketPage(_navigationService);
            _navigationService.Navigate(_wirelessSocketPage);
        }

        private void navigateToSocketAdd()
        {
            _wirelessSocketAddPage = new WirelessSocketAddPage(_navigationService);
            _navigationService.Navigate(_wirelessSocketAddPage);
        }

        private void navigateToMap()
        {
            _mapPage = new MapPage(_navigationService);
            _navigationService.Navigate(_mapPage);
        }

        private void SwitchCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _wirelessSwitchPage = new WirelessSwitchPage(_navigationService);
            _navigationService.Navigate(_wirelessSwitchPage);
        }

        private void navigateToSwitchAdd()
        {
            _wirelessSwitchAddPage = new WirelessSwitchAddPage(_navigationService);
            _navigationService.Navigate(_wirelessSwitchAddPage);
        }

        private void ScheduleCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _schedulePage = new SchedulePage(_navigationService);
            _navigationService.Navigate(_schedulePage);
        }

        private void navigateToScheduleAdd()
        {
            _scheduleAddPage = new ScheduleAddPage(_navigationService);
            _navigationService.Navigate(_scheduleAddPage);
        }

        private void WeatherCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _weatherPage = new WeatherPage(_navigationService);
            _navigationService.Navigate(_weatherPage);
        }

        private void TemperatureCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _temperaturePage = new TemperaturePage(_navigationService);
            _navigationService.Navigate(_temperaturePage);
        }

        private void MenuCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _menuPage = new MenuPage(_navigationService);
            _navigationService.Navigate(_menuPage);
        }

        private void ShoppingCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _shoppingListPage = new ShoppingListPage(_navigationService);
            _navigationService.Navigate(_shoppingListPage);
        }

        private void navigateToShoppingAdd()
        {
            _shoppingEntryAddPage = new ShoppingEntryAddPage(_navigationService);
            _navigationService.Navigate(_shoppingEntryAddPage);
        }

        private void BirthdayCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _birthdayPage = new BirthdayPage(_navigationService);
            _navigationService.Navigate(_birthdayPage);
        }

        private void navigateToBirthdayAdd()
        {
            _birthdayAddPage = new BirthdayAddPage(_navigationService);
            _navigationService.Navigate(_birthdayAddPage);
        }

        private void CoinsCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _coinPage = new CoinPage(_navigationService);
            _navigationService.Navigate(_coinPage);
        }

        private void navigateToCoinAdd()
        {
            _coinAddPage = new CoinAddPage(_navigationService);
            _navigationService.Navigate(_coinAddPage);
        }

        private void MovieCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _moviePage = new MoviePage(_navigationService);
            _navigationService.Navigate(_moviePage);
        }

        private void SeriesCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _seriesPage = new SeriesPage(_navigationService);
            _navigationService.Navigate(_seriesPage);
        }

        private void MagazinCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _magazinPage = new MagazinPage(_navigationService);
            _navigationService.Navigate(_magazinPage);
        }

        private void NovelCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _novelPage = new NovelPage(_navigationService);
            _navigationService.Navigate(_novelPage);
        }

        private void SpecialicedBookCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _specialicedBookPage = new SpecialicedBookPage(_navigationService);
            _navigationService.Navigate(_specialicedBookPage);
        }

        private void MediaMirrorCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _mediaMirrorPage = new MediaMirrorPage(_navigationService);
            _navigationService.Navigate(_mediaMirrorPage);
        }

        private void SecurityCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _securityPage = new SecurityPage(_navigationService);
            _navigationService.Navigate(_securityPage);
        }

        private void SettingsCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _settingsPage = new SettingsPage(_navigationService);
            _navigationService.Navigate(_settingsPage);
        }

        private void readApplicationVersion()
        {
            string version = "";

            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    version = "Debug Mode";
                }
                else
                {
                    version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
            }
            catch (InvalidDeploymentException exception)
            {
                Logger.Instance.Error(TAG, exception.Message);
            }

            VersionTextBlock.Text = version;
        }
    }
}
