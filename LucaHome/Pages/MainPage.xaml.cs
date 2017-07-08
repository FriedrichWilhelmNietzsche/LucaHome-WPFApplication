using Common.Common;
using Common.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class MainPage : Page
    {
        private const string TAG = "MainPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;

        private readonly BirthdayPage _birthdayPage;
        private readonly MoviePage _moviePage;
        private readonly SettingsPage _settingsPage;
        private readonly TemperaturePage _temperaturePage;
        private readonly WeatherPage _weatherPage;
        private readonly WirelessSocketPage _wirelessSocketPage;

        public MainPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);
            _navigationService = navigationService;

            _birthdayPage = new BirthdayPage(_navigationService);
            _moviePage = new MoviePage(_navigationService);
            _settingsPage = new SettingsPage(_navigationService);
            _temperaturePage = new TemperaturePage(_navigationService);
            _weatherPage = new WeatherPage(_navigationService);
            _wirelessSocketPage = new WirelessSocketPage(_navigationService);

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.RemoveBackEntry();
        }

        private void OpenWeather_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.Navigate(_weatherPage);
        }

        private void Socket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.Navigate(_wirelessSocketPage);
        }

        private void Birthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.Navigate(_birthdayPage);
        }

        private void Movie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.Navigate(_moviePage);
        }

        private void Temperature_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.Navigate(_temperaturePage);
        }

        private void Settings_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            _navigationService.Navigate(_settingsPage);
        }
    }
}
