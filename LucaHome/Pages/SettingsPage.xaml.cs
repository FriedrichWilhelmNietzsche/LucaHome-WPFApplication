using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using OpenWeather.Service;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LucaHome.Pages
{
    public partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "SettingsPage";
        private readonly Logger _logger;

        private readonly OpenWeatherService _openWeatherService;
        private readonly NavigationService _navigationService;
        private readonly NetworkService _networkService;
        private readonly TemperatureService _temperatureService;
        private readonly UserService _userService;

        private readonly Notifier _notifier;

        public SettingsPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _openWeatherService = OpenWeatherService.Instance;
            _navigationService = navigationService;
            _networkService = NetworkService.Instance;
            _temperatureService = TemperatureService.Instance;
            _userService = UserService.Instance;

            InitializeComponent();

            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 15,
                    offsetY: 15);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(2),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(2));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            });

            _notifier.ClearMessages();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string UserName
        {
            get
            {
                return _userService.User.Name;
            }
            set
            {
                _logger.Debug(string.Format("UserName set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string password = _userService.User.Passphrase;
                    UserDto newUser = new UserDto(value, password);

                    if (newUser != _userService.User)
                    {
                        _userService.ValidateUser(newUser);

                        string message = "Set new value for user name in settings";
                        _logger.Debug(message);
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("UserName");
                }
            }
        }

        public string Password
        {
            get
            {
                return _userService.User.Passphrase;
            }
            set
            {
                _logger.Debug(string.Format("Password set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string userName = _userService.User.Name;
                    UserDto newUser = new UserDto(userName, value);

                    if (newUser != _userService.User)
                    {
                        _userService.ValidateUser(newUser);

                        string message = "Set new value for user password in settings";
                        _logger.Debug(message);
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("Password");
                }
            }
        }

        public string OpenWeatherCity
        {
            get
            {
                return _temperatureService.OpenWeatherCity;
            }
            set
            {
                _logger.Debug(string.Format("OpenWeatherCity set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string openWeatherCity = _temperatureService.OpenWeatherCity;

                    if (openWeatherCity != value)
                    {
                        _temperatureService.OpenWeatherCity = value;

                        _openWeatherService.City = value;
                        _openWeatherService.LoadCurrentWeather();
                        _openWeatherService.LoadForecastModel();

                        string message = "Set new value for OpenWeatherCity in settings and OpenWeatherService";
                        _logger.Debug(message);
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("OpenWeatherCity");
                }
            }
        }

        public string HomeSSID
        {
            get
            {
                return _networkService.HomeSSID;
            }
            set
            {
                _logger.Debug(string.Format("HomeSSID set with value {0}", value));

                if (value != null && value != string.Empty)
                {
                    string homeSSID = _networkService.HomeSSID;
                    if (homeSSID != value)
                    {
                        _networkService.HomeSSID = value;

                        string message = "Set new value for HomeSSID in settings";
                        _logger.Debug(message);
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("HomeSSID");
                }
            }
        }

        public string RaspberryPiServerIP
        {
            get
            {
                return _networkService.ServerIpAddress;
            }
            set
            {
                _logger.Debug(string.Format("ServerIpAddress set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string serverIpAddress = _networkService.ServerIpAddress;

                    if (serverIpAddress != value)
                    {
                        _networkService.ServerIpAddress = value;

                        string message = "Set new value for ServerIpAddress in settings";
                        _logger.Debug(message);
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("RaspberryPiServerIP");
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            UserNameTextBox.Text = _userService.User.Name;
            PasswordBox.Text = _userService.User.Passphrase;
            OpenWeatherCityTextBox.Text = _temperatureService.OpenWeatherCity;
            HomeSSIDTextBox.Text = _networkService.HomeSSID;
            RaspberryPiServerIPTextBox.Text = _networkService.ServerIpAddress;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
        }

        private void UserNameTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            _logger.Debug(string.Format("UserNameTextBox_KeyDown with sender {0} and keyEventArgs: {1}", sender, keyEventArgs));

            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                UserName = UserNameTextBox.Text;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            _logger.Debug(string.Format("PasswordBox_KeyDown with sender {0} and keyEventArgs: {1}", sender, keyEventArgs));

            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                Password = PasswordBox.Text;
            }
        }

        private void OpenWeatherCityTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            _logger.Debug(string.Format("OpenWeatherCityTextBox_KeyDown with sender {0} and keyEventArgs: {1}", sender, keyEventArgs));

            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                OpenWeatherCity = OpenWeatherCityTextBox.Text;
            }
        }

        private void HomeSSIDTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            _logger.Debug(string.Format("HomeSSIDTextBox_KeyDown with sender {0} and keyEventArgs: {1}", sender, keyEventArgs));

            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                HomeSSID = HomeSSIDTextBox.Text;
            }
        }

        private void RaspberryPiServerIPTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            _logger.Debug(string.Format("RaspberryPiServerIPTextBox_KeyDown with sender {0} and keyEventArgs: {1}", sender, keyEventArgs));

            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                RaspberryPiServerIP = RaspberryPiServerIPTextBox.Text;
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }
    }
}
