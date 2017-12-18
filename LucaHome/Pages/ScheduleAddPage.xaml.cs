using Common.Dto;
using Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static Common.Dto.ScheduleDto;

namespace LucaHome.Pages
{
    public partial class ScheduleAddPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "ScheduleAddPage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private ScheduleDto _newSchedule;

        public ScheduleAddPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            _newSchedule = new ScheduleDto(ScheduleService.Instance.ScheduleList.Count, "", null, null, DateTime.Now, WirelessAction.Null, true);

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

        public CollectionView DayOfWeekList
        {
            get
            {
                IList<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
                foreach (DayOfWeek entry in Enum.GetValues(typeof(DayOfWeek)))
                {
                    dayOfWeekList.Add(entry);
                }
                return new CollectionView(dayOfWeekList);
            }
        }

        public CollectionView SocketList
        {
            get
            {
                IList<string> socketList = new List<string>();
                foreach (WirelessSocketDto entry in WirelessSocketService.Instance.WirelessSocketList)
                {
                    socketList.Add(entry.Name);
                }
                return new CollectionView(socketList);
            }
        }

        public CollectionView SwitchList
        {
            get
            {
                IList<string> switchList = new List<string>();
                foreach (WirelessSwitchDto entry in WirelessSwitchService.Instance.WirelessSwitchList)
                {
                    switchList.Add(entry.Name);
                }
                return new CollectionView(switchList);
            }
        }

        public CollectionView ActionList
        {
            get
            {
                IList<WirelessAction> wirelessActionList = new List<WirelessAction>();
                foreach (WirelessAction entry in Enum.GetValues(typeof(WirelessAction)))
                {
                    if (entry > WirelessAction.Null)
                    {
                        wirelessActionList.Add(entry);
                    }
                }
                return new CollectionView(wirelessActionList);
            }
        }

        public string ScheduleName
        {
            get
            {
                return _newSchedule.Name;
            }
            set
            {
                _newSchedule.Name = value;
                OnPropertyChanged("ScheduleName");
            }
        }

        public string ScheduleSocket
        {
            get
            {
                return _newSchedule.Socket != null ? _newSchedule.Socket.Name : string.Empty;
            }
            set
            {
                WirelessSocketDto scheduleSocket = WirelessSocketService.Instance.GetByName(value);
                _newSchedule.Socket = scheduleSocket;
                OnPropertyChanged("ScheduleSocket");
            }
        }

        public string ScheduleSwitch
        {
            get
            {
                return _newSchedule.WirelessSwitch != null ? _newSchedule.WirelessSwitch.Name : string.Empty;
            }
            set
            {
                WirelessSwitchDto scheduleSwitch = WirelessSwitchService.Instance.GetByName(value);
                _newSchedule.WirelessSwitch = scheduleSwitch;
                OnPropertyChanged("ScheduleSwitch");
            }
        }

        public DayOfWeek ScheduleWeekday
        {
            get
            {
                return _newSchedule.Time.DayOfWeek;
            }
            set
            {
                DateTime scheduleDateTime = _newSchedule.Time;

                int scheduleDayOfWeekInteger = (int)scheduleDateTime.DayOfWeek;
                int newScheduleDayOfWeekInteger = (int)value;

                int difference = scheduleDayOfWeekInteger - newScheduleDayOfWeekInteger;

                DateTime newScheduleDateTime = scheduleDateTime.AddDays(difference);
                _newSchedule.Time = newScheduleDateTime;

                OnPropertyChanged("ScheduleWeekday");
            }
        }

        public DateTime ScheduleTime
        {
            get
            {
                return _newSchedule.Time;
            }
            set
            {
                _newSchedule.Time = value;
                OnPropertyChanged("ScheduleTime");
            }
        }

        public WirelessAction ScheduleAction
        {
            get
            {
                return _newSchedule.Action;
            }
            set
            {
                _newSchedule.Action = value;
                OnPropertyChanged("ScheduleAction");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ScheduleService.Instance.OnAddScheduleFinished -= _onAddScheduleFinished;
            ScheduleService.Instance.OnScheduleDownloadFinished -= _onScheduleDownloadFinished;
        }

        private void SaveSchedule_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            ScheduleService.Instance.OnAddScheduleFinished += _onAddScheduleFinished;
            ScheduleService.Instance.AddSchedule(_newSchedule);
        }

        private void _onAddScheduleFinished(bool success, string response)
        {
            ScheduleService.Instance.OnAddScheduleFinished -= _onAddScheduleFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new schedule!");

                ScheduleService.Instance.OnScheduleDownloadFinished += _onScheduleDownloadFinished;
                ScheduleService.Instance.LoadScheduleList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding wireless socket failed!\n{0}", response));
            }
        }

        private void _onScheduleDownloadFinished(IList<ScheduleDto> scheduleDto, bool success, string response)
        {
            ScheduleService.Instance.OnScheduleDownloadFinished -= _onScheduleDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
