using Common.Common;
using Common.Dto;
using Common.Enums;
using Common.Tools;
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
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly ScheduleService _schedulService;
        private readonly WirelessSocketService _wirelessSocketService;
        private readonly WirelessSwitchService _wirelessSwitchService;

        private readonly Notifier _notifier;

        private readonly IList<WirelessSocketDto> _wirelessSocketList;
        private readonly IList<WirelessSwitchDto> _wirelessSwitchList;
        private ScheduleDto _newSchedule;

        public ScheduleAddPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _schedulService = ScheduleService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;
            _wirelessSwitchService = WirelessSwitchService.Instance;

            _wirelessSocketList = _wirelessSocketService.WirelessSocketList;
            _wirelessSwitchList = _wirelessSwitchService.WirelessSwitchList;

            _newSchedule = new ScheduleDto(_schedulService.ScheduleList.Count, "", null, null, DateTime.Now, WirelessAction.Null, true);

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
                foreach (WirelessSocketDto entry in _wirelessSocketList)
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
                foreach (WirelessSwitchDto entry in _wirelessSwitchList)
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
                WirelessSocketDto scheduleSocket = _wirelessSocketService.GetByName(value);
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
                WirelessSwitchDto scheduleSwitch = _wirelessSwitchService.GetByName(value);
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
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _schedulService.OnAddScheduleFinished -= _onAddScheduleFinished;
            _schedulService.OnScheduleDownloadFinished -= _onScheduleDownloadFinished;
        }

        private void SaveSchedule_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("SaveSchedule_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _schedulService.OnAddScheduleFinished += _onAddScheduleFinished;
            _schedulService.AddSchedule(_newSchedule);
        }

        private void _onAddScheduleFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_onAddScheduleFinished was successful {0}", success));
            _schedulService.OnAddScheduleFinished -= _onAddScheduleFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new schedule!");

                _schedulService.OnScheduleDownloadFinished += _onScheduleDownloadFinished;
                _schedulService.LoadScheduleList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding wireless socket failed!\n{0}", response));
            }
        }

        private void _onScheduleDownloadFinished(IList<ScheduleDto> scheduleDto, bool success, string response)
        {
            _logger.Debug(string.Format("_onScheduleDownloadFinished with model {0} was successful {1}", scheduleDto, success));
            _schedulService.OnScheduleDownloadFinished -= _onScheduleDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }
    }
}
