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
    public partial class WirelessSwitchPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "WirelessSwitchPage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private string _wirelessSwitchSearchKey = string.Empty;

        public WirelessSwitchPage(NavigationService navigationService)
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

        public string WirelessSwitchSearchKey
        {
            get
            {
                return _wirelessSwitchSearchKey;
            }
            set
            {
                _wirelessSwitchSearchKey = value;
                OnPropertyChanged("WirelessSwitchSearchKey");
                WirelessSwitchList = WirelessSwitchService.Instance.FoundWirelessSwitches(_wirelessSwitchSearchKey);
            }
        }

        public IList<WirelessSwitchDto> WirelessSwitchList
        {
            get
            {
                return WirelessSwitchService.Instance.WirelessSwitchList;
            }
            set
            {
                OnPropertyChanged("WirelessSocketList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished += _wirelessSwitchListDownloadFinished;
            WirelessSwitchService.Instance.OnWirelessSwitchToggleFinished += _toggleWirelessSwitchFinished;

            if (WirelessSwitchService.Instance.WirelessSwitchList == null)
            {
                WirelessSwitchService.Instance.LoadWirelessSwitchList();
                return;
            }

            WirelessSwitchList = WirelessSwitchService.Instance.WirelessSwitchList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished -= _wirelessSwitchListDownloadFinished;
            WirelessSwitchService.Instance.OnWirelessSwitchToggleFinished -= _toggleWirelessSwitchFinished;
            WirelessSwitchService.Instance.OnWirelessSwitchDeleteFinished -= _onWirelessSwitchDeleteFinished;

            _notifier.Dispose();
        }

        private void WirelessSwitchButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                string switchName = (String)senderButton.Tag;
                WirelessSwitchService.Instance.ToggleWirelessSwitch(switchName);
            }
        }

        private void ButtonUpdateWirelessSwitch_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int wirelessSwitchId = (int)senderButton.Tag;
                WirelessSwitchDto updateWirelessSwitch = WirelessSwitchService.Instance.GetById(wirelessSwitchId);

                WirelessSwitchUpdatePage wirelessSwitchUpdatePage = new WirelessSwitchUpdatePage(_navigationService, updateWirelessSwitch);
                _navigationService.Navigate(wirelessSwitchUpdatePage);
            }
        }

        private void ButtonDeleteWirelessSwitch_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int wirelessSwitchId = (int)senderButton.Tag;
                WirelessSwitchDto deleteWirelessSwitch = WirelessSwitchService.Instance.GetById(wirelessSwitchId);

                DeleteDialog wirelessSwitchDeleteDialog = new DeleteDialog("Delete switch?",
                    string.Format("Socket: {0}\nArea: {1}\nKeyCode: {2}", deleteWirelessSwitch.Name, deleteWirelessSwitch.Area, deleteWirelessSwitch.KeyCode));
                wirelessSwitchDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wirelessSwitchDeleteDialog.ShowDialog();

                var confirmDelete = wirelessSwitchDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    WirelessSwitchService.Instance.OnWirelessSwitchDeleteFinished += _onWirelessSwitchDeleteFinished;
                    WirelessSwitchService.Instance.DeleteWirelessSwitch(deleteWirelessSwitch);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new WirelessSwitchAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSwitchService.Instance.LoadWirelessSwitchList();
        }

        private void _wirelessSwitchListDownloadFinished(IList<WirelessSwitchDto> wirelessSwitchList, bool success, string response)
        {
            WirelessSwitchList = WirelessSwitchService.Instance.WirelessSwitchList;
        }

        private void _toggleWirelessSwitchFinished(bool success, string response)
        {
            if (success)
            {
                _notifier.ShowSuccess("Successfully toggled switch");
            }
            else
            {
                _notifier.ShowError(string.Format("Failed to toggle switch\n{0}", response));
            }
        }

        private void _onWirelessSwitchDeleteFinished(bool success, string response)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchDeleteFinished -= _onWirelessSwitchDeleteFinished;
            WirelessSwitchService.Instance.LoadWirelessSwitchList();
        }
    }
}
