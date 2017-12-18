using Common.Dto;
using Common.Tools;
using Data.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LucaHome.Pages
{
    public partial class LoginPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "LoginPage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private UserDto _newUser;

        public LoginPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            _newUser = new UserDto("", "");

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
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

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
                return _newUser.Name;
            }
            set
            {
                _newUser.Name = value;
                OnPropertyChanged("UserName");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UserService.Instance.OnUserCheckedFinished -= _onUserCheckedFinished;
            _notifier.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_newUser.Name == null || _newUser.Name == string.Empty)
            {
                _notifier.ShowWarning("Please enter a username");
                return;
            }

            string password = PasswordBox.Password;
            if (password == null || password == string.Empty)
            {
                _notifier.ShowWarning("Please enter a password");
                return;
            }
            _newUser.Passphrase = password;

            UserService.Instance.OnUserCheckedFinished += _onUserCheckedFinished;
            UserService.Instance.ValidateUser(_newUser);
        }

        private void _onUserCheckedFinished(string response, bool success)
        {
            UserService.Instance.OnUserCheckedFinished -= _onUserCheckedFinished;

            if (!success)
            {
                Logger.Instance.Error(TAG, response);
                _notifier.ShowError(response);
                return;
            }

            _navigationService.Navigate(new BootPage(_navigationService));
        }
    }
}
