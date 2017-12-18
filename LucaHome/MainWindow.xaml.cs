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
        private const string APP_ID = "guepardoapps.LucaHome";

        private int _failedUserCheck;
        private const int MAX_FAILED_USER_CHECK = 3;

        private const string TAG = "MainWindow";

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
        private readonly LoginPage _loginPage;

        public MainWindow()
        {
            InitializeComponent();

            _failedUserCheck = 0;

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
            _openWeatherService.SetWallpaperActive = _temperatureService.SetWallpaperActive;

            _bootPage = new BootPage(_navigationService);
            _loginPage = new LoginPage(_navigationService);

            // Check for user first
            if (!_userService.UserSaved())
            {
                _navigationService.Navigate(_loginPage);
            }
            else
            {
                _userService.OnUserCheckedFinished += _onUserCheckedFinished;
                _userService.ValidateUser();
            }
        }

        private void _onUserCheckedFinished(string response, bool success)
        {
            Logger.Instance.Debug(TAG, string.Format("_onUserCheckedFinished with response {0} and success {1}", response, success));

            _userService.OnUserCheckedFinished -= _onUserCheckedFinished;

            // Is entered user was validated we can go to the boot page
            if (success)
            {
                _failedUserCheck = 0;
                _navigationService.Navigate(_bootPage);
            }
            else
            {
                _failedUserCheck++;
                if (_failedUserCheck < MAX_FAILED_USER_CHECK)
                {
                    _userService.OnUserCheckedFinished += _onUserCheckedFinished;
                    _userService.ValidateUser();
                }
                else
                {
                    Logger.Instance.Information(TAG, "Too many failed checks! Navigating to LoginPage");
                    _navigationService.Navigate(_loginPage);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs cancelEventArgs)
        {
            _userService.OnUserCheckedFinished -= _onUserCheckedFinished;

            _birthdayService.Dispose();
            _coinService.Dispose();
            _libraryService.Dispose();
            _mapContentService.Dispose();
            _menuService.Dispose();
            _movieService.Dispose();
            _novelService.Dispose();
            _openWeatherService.Dispose();
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
