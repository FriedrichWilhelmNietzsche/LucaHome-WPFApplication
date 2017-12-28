using Common.Dto;
using Common.Enums;
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
    public partial class MeterEditPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MeterEditPage";

        private readonly NavigationService _navigationService;
        private MeterDataDto _editMeterData;
        private EditAction _editAction;

        private readonly Notifier _notifier;

        public MeterEditPage(NavigationService navigationService, MeterDataDto editMeterData, EditAction editAction)
        {
            _navigationService = navigationService;
            _editMeterData = editMeterData;
            _editAction = editAction;

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

        public string MeterType
        {
            get
            {
                return _editMeterData.Type;
            }
            set
            {
                _editMeterData.Type = value;
                OnPropertyChanged("MeterType");
            }
        }

        public string MeterArea
        {
            get
            {
                return _editMeterData.Area;
            }
            set
            {
                _editMeterData.Area = value;
                OnPropertyChanged("MeterArea");
            }
        }

        public string MeterId
        {
            get
            {
                return _editMeterData.MeterId;
            }
            set
            {
                _editMeterData.MeterId = value;
                OnPropertyChanged("MeterId");
            }
        }

        public double MeterValue
        {
            get
            {
                return _editMeterData.Value;
            }
            set
            {
                string newValueString = value.ToString();
                double newValue = 0;
                bool parseNewValueSuccess = double.TryParse(newValueString, out newValue);
                if (parseNewValueSuccess)
                {
                    _editMeterData.Value = newValue;
                    OnPropertyChanged("MeterValue");
                }
            }
        }

        public DateTime SaveDate
        {
            get
            {
                return _editMeterData.SaveDate;
            }
            set
            {
                _editMeterData.SaveDate = value;
                OnPropertyChanged("MeterSaveDate");
            }
        }

        public string MeterImageName
        {
            get
            {
                return _editMeterData.ImageName;
            }
            set
            {
                _editMeterData.ImageName = value;
                OnPropertyChanged("MeterImageName");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MeterDataService.Instance.OnMeterDataAddFinished -= _onMeterDataAddFinished;
            MeterDataService.Instance.OnMeterDataUpdateFinished -= _onMeterDataUpdateFinished;
            MeterDataService.Instance.OnMeterDataDownloadFinished -= _onMeterDataDownloadFinished;
        }

        private void SaveMeterData_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_editAction == EditAction.ADD)
            {
                MeterDataService.Instance.OnMeterDataAddFinished += _onMeterDataAddFinished;
                MeterDataService.Instance.AddMeterData(_editMeterData);
            }
            else if (_editAction == EditAction.UPDATE)
            {
                MeterDataService.Instance.OnMeterDataUpdateFinished += _onMeterDataUpdateFinished;
                MeterDataService.Instance.UpdateMeterData(_editMeterData);
            }
            else
            {
                _notifier.ShowError(string.Format("Invalid action!", _editAction));
                _navigationService.GoBack();
            }
        }

        private void _onMeterDataAddFinished(bool success, string response)
        {
            MeterDataService.Instance.OnMeterDataAddFinished -= _onMeterDataAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new meter data!");

                MeterDataService.Instance.OnMeterDataDownloadFinished += _onMeterDataDownloadFinished;
                MeterDataService.Instance.LoadMeterDataList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding meter data failed!\n{0}", response));
            }
        }

        private void _onMeterDataUpdateFinished(bool success, string response)
        {
            MeterDataService.Instance.OnMeterDataUpdateFinished -= _onMeterDataUpdateFinished;

            if (success)
            {
                _notifier.ShowSuccess("Updated meter data!");

                MeterDataService.Instance.OnMeterDataDownloadFinished += _onMeterDataDownloadFinished;
                MeterDataService.Instance.LoadMeterDataList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating meter data failed!\n{0}", response));
            }
        }

        private void _onMeterDataDownloadFinished(IList<MeterDataDto> meterDataList, bool success, string response)
        {
            MeterDataService.Instance.OnMeterDataDownloadFinished -= _onMeterDataDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
