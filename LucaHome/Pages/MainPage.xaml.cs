using Common.Common;
using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System;
using System.ComponentModel;
using OpenWeather.Service;
using Data.Services;

namespace LucaHome.Pages
{
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MainPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly TemperatureService _temperatureService;

        private readonly BirthdayPage _birthdayPage;
        private readonly MenuPage _menuPage;
        private readonly MoviePage _moviePage;
        private readonly SettingsPage _settingsPage;
        private readonly ShoppingListPage _shoppingListPage;
        private readonly TemperaturePage _temperaturePage;
        private readonly WeatherPage _weatherPage;
        private readonly WirelessSocketPage _wirelessSocketPage;

        public MainPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _temperatureService = TemperatureService.Instance;

            _birthdayPage = new BirthdayPage(_navigationService);
            _menuPage = new MenuPage(_navigationService);
            _moviePage = new MoviePage(_navigationService);
            _settingsPage = new SettingsPage(_navigationService);
            _shoppingListPage = new ShoppingListPage(_navigationService);
            _temperaturePage = new TemperaturePage(_navigationService);
            _weatherPage = new WeatherPage(_navigationService);
            _wirelessSocketPage = new WirelessSocketPage(_navigationService);

            InitializeComponent();

            SocketCard.ButtonAddCommand = new DelegateCommand(navigateToSocketAdd);
            ShoppingListCard.ButtonAddCommand = new DelegateCommand(navigateToShoppingAdd);
            BirthdayCard.ButtonAddCommand = new DelegateCommand(navigateToBirthdayAdd);
            MovieCard.ButtonAddCommand = new DelegateCommand(navigateToMovieAdd);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.RemoveBackEntry();

            // TODO Create and fix binding
            WeatherCard.BottomTitleText.Text = _openWeatherService.City;
            WeatherCard.BottomDataText.Text = string.Format("{0}°C", _openWeatherService.CurrentWeather.Temperature);

            // TODO Create and fix binding
            TemperatureCard.BottomTitleText.Text = _temperatureService.TemperatureList[0]?.Area;
            TemperatureCard.BottomDataText.Text = _temperatureService.TemperatureList[0]?.TemperatureString;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} with arguments {1}", sender, routedEventArgs));

        }

        private void SocketCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_wirelessSocketPage);
        }

        private void navigateToSocketAdd()
        {
            _logger.Debug("navigateToSocketAdd");
            _navigationService.Navigate(new WirelessSocketAddPage(_navigationService));
        }

        private void WeatherCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_weatherPage);
        }

        private void TemperatureCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_temperaturePage);
        }

        private void MenuCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_menuPage);
        }

        private void ShoppingCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_shoppingListPage);
        }

        private void navigateToShoppingAdd()
        {
            _logger.Debug("navigateToShoppingAdd");
            _navigationService.Navigate(new ShoppingEntryAddPage(_navigationService));
        }

        private void BirthdayCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_birthdayPage);
        }

        private void navigateToBirthdayAdd()
        {
            _logger.Debug("navigateToBirthdayAdd");
            _navigationService.Navigate(new BirthdayAddPage(_navigationService));
        }

        private void MovieCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_moviePage);
        }

        private void navigateToMovieAdd()
        {
            _logger.Debug("navigateToMovieAdd");
            _navigationService.Navigate(new MovieAddPage(_navigationService));
        }

        private void SettingsCard_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with mouseButtonEventArgs {1}", sender, mouseButtonEventArgs));
            _navigationService.Navigate(_settingsPage);
        }
    }
}
