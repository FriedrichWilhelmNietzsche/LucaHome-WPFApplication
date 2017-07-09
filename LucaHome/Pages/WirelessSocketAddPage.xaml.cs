using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using LucaHome.Rules;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LucaHome.Pages
{
    public partial class WirelessSocketAddPage : Page
    {
        private const string TAG = "WirelessSocketAddPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly WirelessSocketService _wirelessSocketService;

        private readonly Notifier _notifier;

        private string _socketName = string.Empty;
        private string _socketArea = string.Empty;
        private string _socketCode = string.Empty;

        public WirelessSocketAddPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _wirelessSocketService = WirelessSocketService.Instance;

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

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            NameTextBox.Text = _socketName;
            AreaTextBox.Text = _socketArea;
            CodeTextBox.Text = _socketCode;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _wirelessSocketService.OnAddWirelessSocketFinished -= _onAddWirelessSocketFinished;
            _wirelessSocketService.OnWirelessSocketDownloadFinished -= _onWirelessSocketDownloadFinished;
        }

        private void SaveWirelessSocket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("SaveWirelessSocket_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            int id = _wirelessSocketService.WirelessSocketList.Count;
            string socketName = NameTextBox.Text;
            string socketArea = AreaTextBox.Text;
            string socketCode = CodeTextBox.Text;

            ValidationResult socketNameResult = new TextBoxLengthRule().Validate(socketName, null);
            if (!socketNameResult.IsValid)
            {
                _notifier.ShowError("Please enter a valid wireless socket name!");
                return;
            }

            ValidationResult socketAreaResult = new TextBoxLengthRule().Validate(socketArea, null);
            if (!socketAreaResult.IsValid)
            {
                _notifier.ShowError("Please enter a valid wireless socket area!");
                return;
            }

            ValidationResult socketCodeResult = new WirelessSocketCodeFormatRule().Validate(socketCode, null);
            if (!socketCodeResult.IsValid)
            {
                _notifier.ShowError("Please enter a valid wireless socket code!");
                return;
            }

            WirelessSocketDto newWirelessSocket = new WirelessSocketDto(id, socketName, socketArea, socketCode, false);

            _wirelessSocketService.OnAddWirelessSocketFinished += _onAddWirelessSocketFinished;
            _wirelessSocketService.AddWirelessSocket(newWirelessSocket);
        }

        private void _onAddWirelessSocketFinished(bool success)
        {
            _logger.Debug(string.Format("_onAddWirelessSocketFinished was successful {0}", success));

            _wirelessSocketService.OnAddWirelessSocketFinished -= _onAddWirelessSocketFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new wireless socket!");

                _wirelessSocketService.OnWirelessSocketDownloadFinished += _onWirelessSocketDownloadFinished;
                _wirelessSocketService.LoadWirelessSocketList();
            }
            else
            {
                _notifier.ShowError("Adding wireless socket failed!");
            }
        }

        private void _onWirelessSocketDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success)
        {
            _logger.Debug(string.Format("_onWirelessSocketDownloadFinished with model {0} was successful {1}", wirelessSocketList, success));

            _wirelessSocketService.OnWirelessSocketDownloadFinished -= _onWirelessSocketDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }
    }
}
