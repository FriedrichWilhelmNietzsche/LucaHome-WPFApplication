using Common.Dto;
using Data.Services;
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

namespace LucaHome.Pages
{
    public partial class WirelessSocketUpdatePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "WirelessSocketUpdatePage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private WirelessSocketDto _updateWirelessSocket;

        public WirelessSocketUpdatePage(NavigationService navigationService, WirelessSocketDto updateWirelessSocket)
        {
            _navigationService = navigationService;

            _updateWirelessSocket = updateWirelessSocket;

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

        public string SocketName
        {
            get
            {
                return _updateWirelessSocket.Name;
            }
            set
            {
                _updateWirelessSocket.Name = value;
                OnPropertyChanged("SocketName");
            }
        }

        public string SocketArea
        {
            get
            {
                return _updateWirelessSocket.Area;
            }
            set
            {
                _updateWirelessSocket.Area = value;
                OnPropertyChanged("SocketArea");
            }
        }

        public string SocketCode
        {
            get
            {
                return _updateWirelessSocket.Code;
            }
            set
            {
                _updateWirelessSocket.Code = value;
                OnPropertyChanged("SocketCode");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSocketService.Instance.OnUpdateWirelessSocketFinished -= _onUpdateWirelessSocketFinished;
            WirelessSocketService.Instance.OnWirelessSocketDownloadFinished -= _onWirelessSocketDownloadFinished;
        }

        private void UpdateWirelessSocket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSocketService.Instance.OnUpdateWirelessSocketFinished += _onUpdateWirelessSocketFinished;
            WirelessSocketService.Instance.UpdateWirelessSocket(_updateWirelessSocket);
        }

        private void _onUpdateWirelessSocketFinished(bool success, string response)
        {
            WirelessSocketService.Instance.OnUpdateWirelessSocketFinished -= _onUpdateWirelessSocketFinished;

            if (success)
            {
                _notifier.ShowSuccess("Updated wireless socket!");

                WirelessSocketService.Instance.OnWirelessSocketDownloadFinished += _onWirelessSocketDownloadFinished;
                WirelessSocketService.Instance.LoadWirelessSocketList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating wireless socket failed!\n{0}", response));
            }
        }

        private void _onWirelessSocketDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            WirelessSocketService.Instance.OnWirelessSocketDownloadFinished -= _onWirelessSocketDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
