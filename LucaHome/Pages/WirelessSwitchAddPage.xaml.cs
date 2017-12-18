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
    public partial class WirelessSwitchAddPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "WirelessSwitchAddPage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private WirelessSwitchDto _addWirelessSwitch;

        public WirelessSwitchAddPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            _addWirelessSwitch = new WirelessSwitchDto(WirelessSwitchService.Instance.WirelessSwitchList.Count, "", "", 0, '0', false, false, new DateTime(), "");

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
                return _addWirelessSwitch.Name;
            }
            set
            {
                _addWirelessSwitch.Name = value;
                OnPropertyChanged("SwitchName");
            }
        }

        public string SwitchArea
        {
            get
            {
                return _addWirelessSwitch.Area;
            }
            set
            {
                _addWirelessSwitch.Area = value;
                OnPropertyChanged("SwitchArea");
            }
        }

        public char SwitchKeyCode
        {
            get
            {
                return _addWirelessSwitch.KeyCode;
            }
            set
            {
                _addWirelessSwitch.KeyCode = value;
                OnPropertyChanged("SwitchKeyCode");
            }
        }

        public int SwitchRemoteId
        {
            get
            {
                return _addWirelessSwitch.RemoteId;
            }
            set
            {
                _addWirelessSwitch.RemoteId = value;
                OnPropertyChanged("SwitchRemoteId");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchAddFinished -= _onAddWirelessSwitchFinished;
            WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished -= _onWirelessSwitchDownloadFinished;
        }

        private void AddWirelessSwitch_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchAddFinished += _onAddWirelessSwitchFinished;
            WirelessSwitchService.Instance.AddWirelessSwitch(_addWirelessSwitch);
        }

        private void _onAddWirelessSwitchFinished(bool success, string response)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchAddFinished -= _onAddWirelessSwitchFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added wireless switch!");

                WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished += _onWirelessSwitchDownloadFinished;
                WirelessSwitchService.Instance.LoadWirelessSwitchList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding wireless switch failed!\n{0}", response));
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
