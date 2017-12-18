using Common.Dto;
using Data.Services;
using LucaHome.Rules;
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

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        public SettingsPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();
            DataContext = this;

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
                return UserService.Instance.User.Name;
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    string password = UserService.Instance.User.Passphrase;
                    UserDto newUser = new UserDto(value, password);

                    if (newUser != UserService.Instance.User)
                    {
                        UserService.Instance.ValidateUser(newUser);

                        string message = "Set new value for user name in settings";
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
                return UserService.Instance.User.Passphrase;
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    string userName = UserService.Instance.User.Name;
                    UserDto newUser = new UserDto(userName, value);

                    if (newUser != UserService.Instance.User)
                    {
                        UserService.Instance.ValidateUser(newUser);

                        string message = "Set new value for user password in settings";
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
                return TemperatureService.Instance.OpenWeatherCity;
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    string openWeatherCity = TemperatureService.Instance.OpenWeatherCity;

                    if (openWeatherCity != value)
                    {
                        TemperatureService.Instance.OpenWeatherCity = value;

                        OpenWeatherService.Instance.City = value;
                        OpenWeatherService.Instance.LoadCurrentWeather();
                        OpenWeatherService.Instance.LoadForecastModel();

                        string message = "Set new value for OpenWeatherCity in settings and OpenWeatherService";
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("OpenWeatherCity");
                }
            }
        }

        public bool SetWallpaperActive
        {
            get
            {
                return TemperatureService.Instance.SetWallpaperActive;
            }
            set
            {
                TemperatureService.Instance.SetWallpaperActive = value;
                OpenWeatherService.Instance.SetWallpaperActive = value;

                string message = "Set new value for SetWallpaperActive in settings and OpenWeatherService";
                _notifier.ShowInformation(message);

                OnPropertyChanged("SetWallpaperActive");
            }
        }

        public string HomeSSID
        {
            get
            {
                return NetworkService.Instance.HomeSSID;
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    string homeSSID = NetworkService.Instance.HomeSSID;
                    if (homeSSID != value)
                    {
                        NetworkService.Instance.HomeSSID = value;

                        string message = "Set new value for HomeSSID in settings";
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
                return NetworkService.Instance.ServerIpAddress;
            }
            set
            {
                if (value != null && value != string.Empty)
                {
                    string serverIpAddress = NetworkService.Instance.ServerIpAddress;

                    if (serverIpAddress != value)
                    {
                        NetworkService.Instance.ServerIpAddress = value;
                        string message = "Set new value for ServerIpAddress in settings";
                        _notifier.ShowInformation(message);
                    }

                    OnPropertyChanged("RaspberryPiServerIP");
                }
            }
        }

        public int MediaServerPort
        {
            get
            {
                return 0;
            }
            set
            {
                int newMediaServerPort;
                bool isNumber = int.TryParse(value.ToString(), out newMediaServerPort);

                if (isNumber)
                {
                    // TODO
                    OnPropertyChanged("MediaServerPort");
                }
            }
        }

        public int YoutubeSearchCount
        {
            get
            {
                return 0;
            }
            set
            {
                int newYoutubeSearchCount;
                bool isNumber = int.TryParse(value.ToString(), out newYoutubeSearchCount);

                if (isNumber)
                {
                    // TODO
                    OnPropertyChanged("YoutubeSearchCount");
                }
            }
        }

        private void UserNameTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                UserName = UserNameTextBox.Text;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                Password = PasswordBox.Text;
            }
        }

        private void OpenWeatherCityTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                OpenWeatherCity = OpenWeatherCityTextBox.Text;
            }
        }

        private void HomeSSIDTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                HomeSSID = HomeSSIDTextBox.Text;
            }
        }

        private void RaspberryPiServerIPTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                RaspberryPiServerIP = RaspberryPiServerIPTextBox.Text;
            }
        }

        private void MediaServerPortTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                MediaServerPort = int.Parse(MediaServerPortTextBox.Text);
            }
        }

        private void MediaServerPortTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !TextBoxIntegerRule.Validate(e.Text);
            base.OnPreviewTextInput(e);
        }

        private void YoutubeSearchCountTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                YoutubeSearchCount = int.Parse(YoutubeSearchCountTextBox.Text);
            }
        }

        private void YoutubeSearchCountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !TextBoxIntegerRule.Validate(e.Text);
            base.OnPreviewTextInput(e);
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
