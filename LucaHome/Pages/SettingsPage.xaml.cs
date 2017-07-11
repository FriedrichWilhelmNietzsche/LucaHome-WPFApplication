using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Controller;
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

        private readonly AppSettingsController _appSettingsController;
        private readonly OpenWeatherService _openWeatherService;
        private readonly NavigationService _navigationService;
        private readonly UserService _userService;

        private readonly Notifier _notifier;

        public SettingsPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _appSettingsController = AppSettingsController.Instance;
            _openWeatherService = OpenWeatherService.Instance;
            _navigationService = navigationService;
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
                return _appSettingsController.User.Name;
            }
            set
            {
                _logger.Debug(string.Format("UserName set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string password = _appSettingsController.User.Passphrase;
                    UserDto newUser = new UserDto(value, password);

                    if (newUser != _appSettingsController.User)
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
                return _appSettingsController.User.Passphrase;
            }
            set
            {
                _logger.Debug(string.Format("Password set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string userName = _appSettingsController.User.Name;
                    UserDto newUser = new UserDto(userName, value);

                    if (newUser != _appSettingsController.User)
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
                return _appSettingsController.OpenWeatherCity;
            }
            set
            {
                _logger.Debug(string.Format("OpenWeatherCity set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string openWeatherCity = _appSettingsController.OpenWeatherCity;

                    if (openWeatherCity != value)
                    {
                        _appSettingsController.OpenWeatherCity = value;

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
                return _appSettingsController.HomeSSID;
            }
            set
            {
                _logger.Debug(string.Format("HomeSSID set with value {0}", value));

                if (value != null && value != string.Empty)
                {
                    string homeSSID = _appSettingsController.HomeSSID;
                    if (homeSSID != value)
                    {
                        _appSettingsController.HomeSSID = value;

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
                return _appSettingsController.ServerIpAddress;
            }
            set
            {
                _logger.Debug(string.Format("ServerIpAddress set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string serverIpAddress = _appSettingsController.ServerIpAddress;

                    if (serverIpAddress != value)
                    {
                        _appSettingsController.ServerIpAddress = value;

                        string message = "Set new value for ServerIpAddress in settings";
                        _logger.Debug(message);
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("RaspberryPiServerIP");
                }
            }
        }

        public int RaspberryPiServerPort
        {
            get
            {
                return _appSettingsController.ServerPort;
            }
            set
            {
                _logger.Debug(string.Format("ServerPort set with value {0}", value));
                int serverPort = _appSettingsController.ServerPort;

                if (serverPort != value)
                {
                    _appSettingsController.ServerPort = value;

                    string message = "Set new value for ServerPort in settings";
                    _logger.Debug(message);
                    _notifier.ShowInformation(message);
                }

                OnPropertyChanged("RaspberryPiServerPort");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            UserNameTextBox.Text = _appSettingsController.User.Name;
            PasswordBox.Text = _appSettingsController.User.Passphrase;
            OpenWeatherCityTextBox.Text = _appSettingsController.OpenWeatherCity;
            HomeSSIDTextBox.Text = _appSettingsController.HomeSSID;
            RaspberryPiServerIPTextBox.Text = _appSettingsController.ServerIpAddress;
            RaspberryPiServerPortTextBox.Text = _appSettingsController.ServerPort.ToString();
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

        private void RaspberryPiServerPortTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            _logger.Debug(string.Format("RaspberryPiServerPortTextBox_KeyDown with sender {0} and keyEventArgs: {1}", sender, keyEventArgs));

            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                int raspberryPort = _appSettingsController.ServerPort;
                int.TryParse(RaspberryPiServerPortTextBox.Text, out raspberryPort);
                RaspberryPiServerPort = raspberryPort;
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }
    }
}
