using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 * 
 * Toasts
 * https://github.com/raflop/ToastNotifications
 */

namespace LucaHome.Pages
{
    public partial class WirelessSocketPage : Page
    {
        private const string TAG = "WirelessSocketPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly WirelessSocketService _wirelessSocketService;

        private readonly Notifier _notifier;

        public WirelessSocketPage(NavigationService navigationService)
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
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _wirelessSocketService.OnWirelessSocketDownloadFinished += _wirelessSocketListDownloadFinished;
            _wirelessSocketService.OnSetWirelessSocketFinished += _setWirelessSocketFinished;

            if (_wirelessSocketService.WirelessSocketList == null)
            {
                _wirelessSocketService.LoadWirelessSocketList();
                return;
            }

            setList();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _wirelessSocketService.OnWirelessSocketDownloadFinished -= _wirelessSocketListDownloadFinished;
            _wirelessSocketService.OnSetWirelessSocketFinished -= _setWirelessSocketFinished;

            _notifier.Dispose();
        }

        private void setList()
        {
            _logger.Debug("setList");

            foreach (WirelessSocketDto entry in _wirelessSocketService.WirelessSocketList)
            {
                WirelessSocketList.Items.Add(entry);
            }
        }

        private void _wirelessSocketListDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success)
        {
            _logger.Debug(string.Format("_wirelessSocketListDownloadFinished with model {0} was successful: {1}", wirelessSocketList, success));
            setList();
        }

        private void _setWirelessSocketFinished(IList<WirelessSocketDto> wirelessSocketList, bool success)
        {
            _logger.Debug(string.Format("_setWirelessSocketFinished was successful: {0}", success));
            if (success)
            {
                _notifier.ShowSuccess("Successfully set socket");
            }
            else
            {
                _notifier.ShowError("Failed to set socket");
            }
        }

        private void WirelessSocketButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if(sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string socketName = (String)senderButton.Tag;
                _wirelessSocketService.ChangeWirelessSocketState(socketName);
            }
        }
    }
}
