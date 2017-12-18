using Common.Dto;
using Data.Services;
using LucaHome.Rules;
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
    public partial class WirelessSwitchUpdatePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "WirelessSwitchUpdatePage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private WirelessSwitchDto _updateWirelessSwitch;

        public WirelessSwitchUpdatePage(NavigationService navigationService, WirelessSwitchDto updateWirelessSwitch)
        {
            _navigationService = navigationService;

            _updateWirelessSwitch = updateWirelessSwitch;

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

        public string SwitchName
        {
            get
            {
                return _updateWirelessSwitch.Name;
            }
            set
            {
                _updateWirelessSwitch.Name = value;
                OnPropertyChanged("SwitchName");
            }
        }

        public string SwitchArea
        {
            get
            {
                return _updateWirelessSwitch.Area;
            }
            set
            {
                _updateWirelessSwitch.Area = value;
                OnPropertyChanged("SwitchArea");
            }
        }

        public char SwitchKeyCode
        {
            get
            {
                return _updateWirelessSwitch.KeyCode;
            }
            set
            {
                _updateWirelessSwitch.KeyCode = value;
                OnPropertyChanged("SwitchKeyCode");
            }
        }

        public int SwitchRemoteId
        {
            get
            {
                return _updateWirelessSwitch.RemoteId;
            }
            set
            {
                _updateWirelessSwitch.RemoteId = value;
                OnPropertyChanged("SwitchRemoteId");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchUpdateFinished -= _onUpdateWirelessSwitchFinished;
            WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished -= _onWirelessSwitchDownloadFinished;
        }

        private void UpdateWirelessSwitch_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchUpdateFinished += _onUpdateWirelessSwitchFinished;
            WirelessSwitchService.Instance.UpdateWirelessSwitch(_updateWirelessSwitch);
        }

        private void _onUpdateWirelessSwitchFinished(bool success, string response)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchUpdateFinished -= _onUpdateWirelessSwitchFinished;

            if (success)
            {
                _notifier.ShowSuccess("Updated wireless switch!");

                WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished += _onWirelessSwitchDownloadFinished;
                WirelessSwitchService.Instance.LoadWirelessSwitchList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating wireless switch failed!\n{0}", response));
            }
        }

        private void _onWirelessSwitchDownloadFinished(IList<WirelessSwitchDto> wirelessSwitchList, bool success, string response)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished -= _onWirelessSwitchDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void Button_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !TextBoxIntegerRule.Validate(e.Text);
            base.OnPreviewTextInput(e);
        }
    }
}
