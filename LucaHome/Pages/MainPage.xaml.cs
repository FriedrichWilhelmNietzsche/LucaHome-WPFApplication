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
        private Logger _logger;

        private NavigationService _navigationService;

        private WeatherPage _weatherPage;

        public MainPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);
            _navigationService = navigationService;

            _weatherPage = new WeatherPage(_navigationService);

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
        }

        private void Birthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
        }

        private void Movie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
        }

        private void Temperature_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
        }

        private void Settings_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
        }
    }
}
