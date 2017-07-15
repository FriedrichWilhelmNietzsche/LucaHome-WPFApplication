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

namespace LucaHome.Pages
{
    public partial class MainPage : Page
    {
        private const string TAG = "MainPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly TemperatureService _temperatureService;

        private string _temperatureBottomTitle;
        private string _temperatureBottomData;

        public MainPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _temperatureService = TemperatureService.Instance;

            InitializeComponent();

            BirthdayCard.ButtonAddCommand = new DelegateCommand(navigateToBirthdayAdd);
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

            // TODO Create and fix binding
            WeatherCard.BottomTitleText.Text = _openWeatherService.City;
            WeatherCard.BottomDataText.Text = string.Format("{0}°C", _openWeatherService.CurrentWeather.Temperature);
            _openWeatherService.OnCurrentWeatherDownloadFinished += _onCurrentWeatherDownloadFinished;

            // TODO Create and fix binding
            TemperatureCard.BottomTitleText.Text = _temperatureService.TemperatureList[0]?.Area;
            TemperatureCard.BottomDataText.Text = _temperatureService.TemperatureList[0]?.TemperatureString;
            _temperatureService.OnTemperatureDownloadFinished += _onTemperatureDownloadFinished;
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
            _navigationService.Navigate(new WirelessSocketPage(_navigationService));
        }

        private void navigateToSocketAdd()
        {
            _logger.Debug("navigateToSocketAdd");
            _navigationService.Navigate(new WirelessSocketAddPage(_navigationService));
        }

        private void navigateToMap()
        {
            _logger.Debug("navigateToMap");
            _navigationService.Navigate(new MapPage(_navigationService));
        }

        private void ScheduleCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("ScheduleCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new SchedulePage(_navigationService));
        }

        private void navigateToScheduleAdd()
        {
            _logger.Debug("navigateToScheduleAdd");
            _navigationService.Navigate(new ScheduleAddPage(_navigationService));
        }

        private void WeatherCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("WeatherCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new WeatherPage(_navigationService));
        }

        private void TemperatureCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("TemperatureCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new TemperaturePage(_navigationService));
        }

        private void MenuCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("MenuCard_MouseUp: Received click of sender {0} with arguments {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new MenuPage(_navigationService));
        }

        private void ShoppingCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("ShoppingCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new ShoppingListPage(_navigationService));
        }

        private void navigateToShoppingAdd()
        {
            _logger.Debug("navigateToShoppingAdd");
            _navigationService.Navigate(new ShoppingEntryAddPage(_navigationService));
        }

        private void BirthdayCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("BirthdayCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new BirthdayPage(_navigationService));
        }

        private void navigateToBirthdayAdd()
        {
            _logger.Debug("navigateToBirthdayAdd");
            _navigationService.Navigate(new BirthdayAddPage(_navigationService));
        }

        private void MovieCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("MovieCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new MoviePage(_navigationService));
        }

        private void SettingsCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("SettingsCard_MouseUp: Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(new SettingsPage(_navigationService));
        }
    }
}
