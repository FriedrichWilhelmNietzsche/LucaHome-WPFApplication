using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using LucaHome.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class WirelessSocketPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "WirelessSocketPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly WirelessSocketService _wirelessSocketService;

        private readonly Notifier _notifier;

        private string _wirelessSocketSearchKey = string.Empty;
        private IList<WirelessSocketDto> _wirelessSocketList = new List<WirelessSocketDto>();

        private readonly WirelessSocketAddPage _wirelessSocketAddPage;

        public WirelessSocketPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _wirelessSocketService = WirelessSocketService.Instance;

            _wirelessSocketAddPage = new WirelessSocketAddPage(_navigationService);

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

        public string WirelessSocketSearchKey
        {
            get
            {
                return _wirelessSocketSearchKey;
            }
            set
            {
                _wirelessSocketSearchKey = value;
                OnPropertyChanged("WirelessSocketSearchKey");

                if (_wirelessSocketSearchKey != string.Empty)
                {
                    WirelessSocketList = _wirelessSocketService.FoundWirelessSockets(_wirelessSocketSearchKey);
                }
                else
                {
                    WirelessSocketList = _wirelessSocketService.WirelessSocketList;
                }
            }
        }

        public IList<WirelessSocketDto> WirelessSocketList
        {
            get
            {
                return _wirelessSocketList;
            }
            set
            {
                _wirelessSocketList = value;
                OnPropertyChanged("WirelessSocketList");
            }
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

            WirelessSocketList = _wirelessSocketService.WirelessSocketList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _wirelessSocketService.OnWirelessSocketDownloadFinished -= _wirelessSocketListDownloadFinished;
            _wirelessSocketService.OnSetWirelessSocketFinished -= _setWirelessSocketFinished;
            _wirelessSocketService.OnDeleteWirelessSocketFinished -= _onWirelessSocketDeleteFinished;

            _notifier.Dispose();
        }

        private void WirelessSocketButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string socketName = (String)senderButton.Tag;
                _wirelessSocketService.ChangeWirelessSocketState(socketName);
            }
        }

        private void ButtonUpdateWirelessSocket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonUpdateWirelessSocket_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int wirelessSocketId = (int)senderButton.Tag;
                WirelessSocketDto updateWirelessSocket = _wirelessSocketService.GetById(wirelessSocketId);
                _logger.Warning(string.Format("Updating wireless socket {0}!", updateWirelessSocket));

                WirelessSocketUpdatePage wirelessSocketUpdatePage = new WirelessSocketUpdatePage(_navigationService, updateWirelessSocket);
                _navigationService.Navigate(wirelessSocketUpdatePage);
            }
        }

        private void ButtonDeleteWirelessSocket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonDeleteWirelessSocket_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int wirelessSocketId = (int)senderButton.Tag;
                WirelessSocketDto deleteWirelessSocket = _wirelessSocketService.GetById(wirelessSocketId);
                _logger.Warning(string.Format("Asking for deleting wireless socket {0}!", deleteWirelessSocket));

                DeleteDialog wirelessSocketDeleteDialog = new DeleteDialog("Delete socket?",
                    string.Format("Socket: {0}\nArea: {1}\nCode: {2}", deleteWirelessSocket.Name, deleteWirelessSocket.Area, deleteWirelessSocket.Code));
                wirelessSocketDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wirelessSocketDeleteDialog.ShowDialog();

                var confirmDelete = wirelessSocketDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    _wirelessSocketService.OnDeleteWirelessSocketFinished += _onWirelessSocketDeleteFinished;
                    _wirelessSocketService.DeleteWirelessSocket(deleteWirelessSocket);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonAdd_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.Navigate(_wirelessSocketAddPage);
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _wirelessSocketService.LoadWirelessSocketList();
        }

        private void _wirelessSocketListDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            _logger.Debug(string.Format("_wirelessSocketListDownloadFinished with model {0} was successful: {1}", wirelessSocketList, success));
            WirelessSocketList = _wirelessSocketService.WirelessSocketList;
        }

        private void _setWirelessSocketFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            _logger.Debug(string.Format("_setWirelessSocketFinished was successful: {0}", success));
            if (success)
            {
                _notifier.ShowSuccess("Successfully set socket");
            }
            else
            {
                _notifier.ShowError(string.Format("Failed to set socket\n{0}", response));
            }
        }

        private void _onWirelessSocketDeleteFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_onWirelessSocketDeleteFinished with response {0} was successful: {1}", response, success));
            _wirelessSocketService.OnDeleteWirelessSocketFinished -= _onWirelessSocketDeleteFinished;
            _wirelessSocketService.LoadWirelessSocketList();
        }
    }
}
