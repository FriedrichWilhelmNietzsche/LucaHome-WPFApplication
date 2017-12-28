using Common.Dto;
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

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private string _wirelessSocketSearchKey = string.Empty;
        private IList<WirelessSocketDto> _wirelessSocketList;

        public WirelessSocketPage(NavigationService navigationService)
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
                WirelessSocketList = WirelessSocketService.Instance.FoundWirelessSockets(_wirelessSocketSearchKey);
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
            WirelessSocketService.Instance.OnWirelessSocketDownloadFinished += _wirelessSocketListDownloadFinished;
            WirelessSocketService.Instance.OnSetWirelessSocketFinished += _setWirelessSocketFinished;

            if (WirelessSocketService.Instance.WirelessSocketList == null)
            {
                WirelessSocketService.Instance.LoadWirelessSocketList();
                return;
            }

            WirelessSocketList = WirelessSocketService.Instance.WirelessSocketList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSocketService.Instance.OnWirelessSocketDownloadFinished -= _wirelessSocketListDownloadFinished;
            WirelessSocketService.Instance.OnSetWirelessSocketFinished -= _setWirelessSocketFinished;
            WirelessSocketService.Instance.OnDeleteWirelessSocketFinished -= _onWirelessSocketDeleteFinished;

            _notifier.Dispose();
        }

        private void WirelessSocketButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string socketName = (String)senderButton.Tag;
                WirelessSocketService.Instance.ChangeWirelessSocketState(socketName);
            }
        }

        private void ButtonUpdateWirelessSocket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int wirelessSocketId = (int)senderButton.Tag;
                WirelessSocketDto updateWirelessSocket = WirelessSocketService.Instance.GetByTypeId(wirelessSocketId);

                WirelessSocketUpdatePage wirelessSocketUpdatePage = new WirelessSocketUpdatePage(_navigationService, updateWirelessSocket);
                _navigationService.Navigate(wirelessSocketUpdatePage);
            }
        }

        private void ButtonDeleteWirelessSocket_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int wirelessSocketId = (int)senderButton.Tag;
                WirelessSocketDto deleteWirelessSocket = WirelessSocketService.Instance.GetByTypeId(wirelessSocketId);

                DeleteDialog wirelessSocketDeleteDialog = new DeleteDialog("Delete socket?",
                    string.Format("Socket: {0}\nArea: {1}\nCode: {2}", deleteWirelessSocket.Name, deleteWirelessSocket.Area, deleteWirelessSocket.Code));
                wirelessSocketDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wirelessSocketDeleteDialog.ShowDialog();

                var confirmDelete = wirelessSocketDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    WirelessSocketService.Instance.OnDeleteWirelessSocketFinished += _onWirelessSocketDeleteFinished;
                    WirelessSocketService.Instance.DeleteWirelessSocket(deleteWirelessSocket);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new WirelessSocketAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSocketService.Instance.LoadWirelessSocketList();
        }

        private void _wirelessSocketListDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            WirelessSocketList = WirelessSocketService.Instance.WirelessSocketList;
        }

        private void _setWirelessSocketFinished(bool success, string response)
        {
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
            WirelessSocketService.Instance.OnDeleteWirelessSocketFinished -= _onWirelessSocketDeleteFinished;
            WirelessSocketService.Instance.LoadWirelessSocketList();
        }
    }
}
