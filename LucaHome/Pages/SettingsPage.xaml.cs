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

        private readonly AppSettingsService _appSettingsService;
        private readonly OpenWeatherService _openWeatherService;

        private readonly Notifier _notifier;

        public SettingsPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _appSettingsService = AppSettingsService.Instance;
            _openWeatherService = OpenWeatherService.Instance;

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
                return _appSettingsService.User.Name;
            }
            set
            {
                _logger.Debug(string.Format("UserName set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string password = _appSettingsService.User.Passphrase;
                    UserDto newUser = new UserDto(value, password);

                    if (newUser != _appSettingsService.User)
                    {
                        _appSettingsService.User = newUser;

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
                return _appSettingsService.User.Passphrase;
            }
            set
            {
                _logger.Debug(string.Format("Password set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string userName = _appSettingsService.User.Name;
                    UserDto newUser = new UserDto(userName, value);

                    if (newUser != _appSettingsService.User)
                    {
                        _appSettingsService.User = newUser;

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
                return _appSettingsService.OpenWeatherCity;
            }
            set
            {
                _logger.Debug(string.Format("OpenWeatherCity set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string openWeatherCity = _appSettingsService.OpenWeatherCity;

                    if (openWeatherCity != value)
                    {
                        _appSettingsService.OpenWeatherCity = value;

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
                return _appSettingsService.HomeSSID;
            }
            set
            {
                _logger.Debug(string.Format("HomeSSID set with value {0}", value));

                if (value != null && value != string.Empty)
                {
                    string homeSSID = _appSettingsService.HomeSSID;
                    if (homeSSID != value)
                    {
                        _appSettingsService.HomeSSID = value;

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
                return _appSettingsService.ServerIpAddress;
            }
            set
            {
                _logger.Debug(string.Format("ServerIpAddress set with value {0}", value));
                if (value != null && value != string.Empty)
                {
                    string serverIpAddress = _appSettingsService.ServerIpAddress;

                    if (serverIpAddress != value)
                    {
                        _appSettingsService.ServerIpAddress = value;

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
                return _appSettingsService.ServerPort;
            }
            set
            {
                _logger.Debug(string.Format("ServerPort set with value {0}", value));
                int serverPort = _appSettingsService.ServerPort;

                if (serverPort != value)
                {
                    _appSettingsService.ServerPort = value;

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

            UserNameTextBox.Text = _appSettingsService.User.Name;
            PasswordBox.Text = _appSettingsService.User.Passphrase;
            OpenWeatherCityTextBox.Text = _appSettingsService.OpenWeatherCity;
            HomeSSIDTextBox.Text = _appSettingsService.HomeSSID;
            RaspberryPiServerIPTextBox.Text = _appSettingsService.ServerIpAddress;
            RaspberryPiServerPortTextBox.Text = _appSettingsService.ServerPort.ToString();
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
                int raspberryPort = _appSettingsService.ServerPort;
                int.TryParse(RaspberryPiServerPortTextBox.Text, out raspberryPort);
                RaspberryPiServerPort = raspberryPort;
            }
        }
    }
}
