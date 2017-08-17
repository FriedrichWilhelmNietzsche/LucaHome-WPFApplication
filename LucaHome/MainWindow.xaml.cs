using Common.Common;
using Common.Tools;
using Data.Services;
using LucaHome.Pages;
using OpenWeather.Models;
using OpenWeather.Service;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;

namespace LucaHome
{
    public partial class MainWindow : Window
    {
        private const string APP_ID = "guepardoapps.LucaHome";

        private const string TAG = "MainWindow";
        private readonly Logger _logger;

        private readonly BirthdayService _birthdayService;
        private readonly CoinService _coinService;
        private readonly LibraryService _libraryService;
        private readonly MapContentService _mapContentService;
        private readonly MenuService _menuService;
        private readonly MovieService _movieService;
        private readonly NavigationService _navigationService;
        private readonly NovelService _novelService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly ScheduleService _scheduleService;
        private readonly SecurityService _securityService;
        private readonly SeriesService _seriesService;
        private readonly ShoppingListService _shoppingListService;
        private readonly SpecialicedBookService _specialicedBookService;
        private readonly TemperatureService _temperatureService;
        private readonly UserService _userService;
        private readonly WirelessSocketService _wirelessSocketService;

        private readonly BootPage _bootPage;

        public MainWindow()
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();

            _birthdayService = BirthdayService.Instance;
            _coinService = CoinService.Instance;
            _libraryService = LibraryService.Instance;
            _mapContentService = MapContentService.Instance;
            _menuService = MenuService.Instance;
            _movieService = MovieService.Instance;
            _navigationService = _mainFrame.NavigationService;
            _novelService = NovelService.Instance;
            _openWeatherService = OpenWeatherService.Instance;
            _scheduleService = ScheduleService.Instance;
            _securityService = SecurityService.Instance;
            _seriesService = SeriesService.Instance;
            _shoppingListService = ShoppingListService.Instance;
            _specialicedBookService = SpecialicedBookService.Instance;
            _temperatureService = TemperatureService.Instance;
            _userService = UserService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            _openWeatherService.City = _temperatureService.OpenWeatherCity;

            _openWeatherService.OnForecastWeatherDownloadFinished += _onForecastWeatherDownloadFinished;

            // Check for user first
            if (!_userService.UserSaved())
            {
                _logger.Debug("Not yet entered an user! Navigating to LoginPage");
                _navigationService.Navigate(new LoginPage(_navigationService));
            }
            else
            {
                _userService.OnUserCheckedFinished += _onUserCheckedFinished;
                _userService.ValidateUser();
            }
        }

        private void _onForecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            _logger.Debug(string.Format("_onForecastWeatherDownloadFinished with model {0} was successful: {1}", forecastWeather, success));

            // TODO
        }

        private void _onUserCheckedFinished(string response, bool success)
        {
            _logger.Debug(string.Format("_onUserCheckedFinished with response {0} was successful: {1}", response, success));
            _userService.OnUserCheckedFinished -= _onUserCheckedFinished;

            // Is entered user was validated we can go to the boot page
            if (success)
            {
                _logger.Debug("Validated user! Navigating to BootPage");
                _navigationService.Navigate(new BootPage(_navigationService));
            }
            else
            {
                _logger.Debug("Failed to validate user! Navigating to LoginPage");
                _navigationService.Navigate(new LoginPage(_navigationService));
            }
        }

        private void Window_Closing(object sender, CancelEventArgs cancelEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and cancelEventArgs: {1}", sender, cancelEventArgs));

            _userService.OnUserCheckedFinished -= _onUserCheckedFinished;

            _birthdayService.Dispose();
            _coinService.Dispose();
            _libraryService.Dispose();
            _mapContentService.Dispose();
            _menuService.Dispose();
            _movieService.Dispose();
            _novelService.Dispose();
            _scheduleService.Dispose();
            _securityService.Dispose();
            _seriesService.Dispose();
            _shoppingListService.Dispose();
            _specialicedBookService.Dispose();
            _temperatureService.Dispose();
            _userService.Dispose();
            _wirelessSocketService.Dispose();
        }
    }
}
