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
    public partial class WirelessSocketAddPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "WirelessSocketAddPage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private WirelessSocketDto _newWirelessSocket;

        public WirelessSocketAddPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            _newWirelessSocket = new WirelessSocketDto(WirelessSocketService.Instance.WirelessSocketList.Count, "", "", "", false, new DateTime(), "");

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
                return _newWirelessSocket.Name;
            }
            set
            {
                _newWirelessSocket.Name = value;
                OnPropertyChanged("SocketName");
            }
        }

        public string SocketArea
        {
            get
            {
                return _newWirelessSocket.Area;
            }
            set
            {
                _newWirelessSocket.Area = value;
                OnPropertyChanged("SocketArea");
            }
        }

        public string SocketCode
        {
            get
            {
                return _newWirelessSocket.Code;
            }
            set
            {
                _newWirelessSocket.Code = value;
                OnPropertyChanged("SocketCode");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSocketService.Instance.OnAddWirelessSocketFinished -= _onAddWirelessSocketFinished;
            WirelessSocketService.Instance.OnWirelessSocketDownloadFinished -= _onWirelessSocketDownloadFinished;
        }

        private void SaveWirelessSocket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSocketService.Instance.OnAddWirelessSocketFinished += _onAddWirelessSocketFinished;
            WirelessSocketService.Instance.AddWirelessSocket(_newWirelessSocket);
        }

        private void _onAddWirelessSocketFinished(bool success, string response)
        {
            WirelessSocketService.Instance.OnAddWirelessSocketFinished -= _onAddWirelessSocketFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new wireless socket!");

                WirelessSocketService.Instance.OnWirelessSocketDownloadFinished += _onWirelessSocketDownloadFinished;
                WirelessSocketService.Instance.LoadWirelessSocketList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding wireless socket failed!\n{0}", response));
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
