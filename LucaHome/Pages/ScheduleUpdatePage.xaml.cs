using Common.Common;
using Common.Dto;
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
    public partial class ScheduleUpdatePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "ScheduleUpdatePage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly ScheduleService _schedulService;
        private readonly WirelessSocketService _wirelessSocketService;

        private readonly Notifier _notifier;

        private readonly IList<WirelessSocketDto> _wirelessSocketList;
        private ScheduleDto _updateSchedule;

        public ScheduleUpdatePage(NavigationService navigationService, ScheduleDto updateSchedule)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _schedulService = ScheduleService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            _wirelessSocketList = _wirelessSocketService.WirelessSocketList;

            _updateSchedule = updateSchedule;

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

        public CollectionView WeekdayList
        {
            get
            {
                IList<Weekday> weekdayList = new List<Weekday>();
                foreach (Weekday entry in Enum.GetValues(typeof(Weekday)))
                {
                    if (entry > Weekday.Null)
                    {
                        weekdayList.Add(entry);
                    }
                }
                return new CollectionView(weekdayList);
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

        public CollectionView ActionList
        {
            get
            {
                IList<SocketAction> actionList = new List<SocketAction>();
                foreach (SocketAction entry in Enum.GetValues(typeof(SocketAction)))
                {
                    if (entry > SocketAction.Null)
                    {
                        actionList.Add(entry);
                    }
                }
                return new CollectionView(actionList);
            }
        }

        public string ScheduleName
        {
            get
            {
                return _updateSchedule.Name;
            }
            set
            {
                _updateSchedule.Name = value;
                OnPropertyChanged("ScheduleName");
            }
        }

        public string ScheduleInformation
        {
            get
            {
                return _updateSchedule.Information;
            }
            set
            {
                _updateSchedule.Information = value;
                OnPropertyChanged("ScheduleInformation");
            }
        }

        public string ScheduleSocket
        {
            get
            {
                return _updateSchedule.Socket != null ? _updateSchedule.Socket.Name : string.Empty;
            }
            set
            {
                WirelessSocketDto scheduleSocket = _wirelessSocketService.GetByName(value);
                _updateSchedule.Socket = scheduleSocket;
                OnPropertyChanged("ScheduleSocket");
            }
        }

        public Weekday ScheduleWeekday
        {
            get
            {
                return _updateSchedule.WeekDay;
            }
            set
            {
                _updateSchedule.WeekDay = value;
                OnPropertyChanged("ScheduleWeekday");
            }
        }

        public DateTime ScheduleTime
        {
            get
            {
                return _updateSchedule.Time;
            }
            set
            {
                _updateSchedule.Time = value;
                OnPropertyChanged("ScheduleTime");
            }
        }

        public SocketAction ScheduleAction
        {
            get
            {
                return _updateSchedule.Action;
            }
            set
            {
                _updateSchedule.Action = value;
                OnPropertyChanged("ScheduleAction");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _schedulService.OnUpdateScheduleFinished -= _onUpdateScheduleFinished;
            _schedulService.OnScheduleDownloadFinished -= _onScheduleDownloadFinished;
        }

        private void UpdateSchedule_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("UpdateSchedule_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _schedulService.OnUpdateScheduleFinished += _onUpdateScheduleFinished;
            _schedulService.UpdateSchedule(_updateSchedule);
        }

        private void _onUpdateScheduleFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_onUpdateScheduleFinished was successful {0}", success));
            _schedulService.OnUpdateScheduleFinished -= _onUpdateScheduleFinished;

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
