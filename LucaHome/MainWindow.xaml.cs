using Common.Common;
using Common.Tools;
using Data.Services;
using LucaHome.Pages;
using OpenWeather.Service;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;

namespace LucaHome
{
    public partial class MainWindow : Window
    {
        private const string TAG = "MainWindow";
        private readonly Logger _logger;

        private readonly BirthdayService _birthdayService;
        private readonly MapContentService _mapContentService;
        private readonly MenuService _menuService;
        private readonly MovieService _movieService;
        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly ScheduleService _scheduleService;
        private readonly ShoppingListService _shoppingListService;
        private readonly TemperatureService _temperatureService;
        private readonly UserService _userService;
        private readonly WirelessSocketService _wirelessSocketService;

        private readonly BootPage _bootPage;

        public MainWindow()
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();

            _birthdayService = BirthdayService.Instance;
            _mapContentService = MapContentService.Instance;
            _menuService = MenuService.Instance;
            _movieService = MovieService.Instance;
            _navigationService = _mainFrame.NavigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _scheduleService = ScheduleService.Instance;
            _shoppingListService = ShoppingListService.Instance;
            _temperatureService = TemperatureService.Instance;
            _userService = UserService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            _openWeatherService.City = _temperatureService.OpenWeatherCity;

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
            _mapContentService.Dispose();
            _menuService.Dispose();
            _movieService.Dispose();
            _scheduleService.Dispose();
            _shoppingListService.Dispose();
            _temperatureService.Dispose();
            _userService.Dispose();
            _wirelessSocketService.Dispose();
        }
    }
}
