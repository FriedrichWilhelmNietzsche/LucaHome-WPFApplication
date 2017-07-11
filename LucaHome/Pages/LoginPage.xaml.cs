using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LucaHome.Pages
{
    public partial class LoginPage : Page
    {
        private const string TAG = "LoginPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly UserService _userService;

        private readonly Notifier _notifier;

        public LoginPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

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
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            });

            _notifier.ClearMessages();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} with arguments {1}", sender, routedEventArgs));
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} with arguments {1}", sender, routedEventArgs));
            _userService.OnUserCheckedFinished -= _onUserCheckedFinished;
            _notifier.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));

            string userName = UserNameTextBox.Text;
            if (userName == null || userName == string.Empty)
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

            _userService.OnUserCheckedFinished += _onUserCheckedFinished;

            _userService.ValidateUser(new UserDto(userName, password));
        }

        private void _onUserCheckedFinished(string response, bool success)
        {
            _logger.Debug(string.Format("_onUserCheckedFinished with response {0} was successful {1}", response, success));

            _userService.OnUserCheckedFinished -= _onUserCheckedFinished;

            if (!success)
            {
                _logger.Error(response);
                _notifier.ShowError(response);
                return;
            }

            _navigationService.GoBack();
        }
    }
}
