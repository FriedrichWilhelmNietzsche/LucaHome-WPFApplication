using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using LucaHome.Rules;
using System;
using System.Collections.Generic;
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
    public partial class ScheduleAddPage : Page
    {
        private const string TAG = "ScheduleAddPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly ScheduleService _schedulService;
        private readonly WirelessSocketService _wirelessSocketService;

        private readonly Notifier _notifier;

        private readonly IList<WirelessSocketDto> _wirelessSocketList;

        private string _scheduleName = string.Empty;
        private string _scheduleInformation = string.Empty;

        public ScheduleAddPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _schedulService = ScheduleService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            _wirelessSocketList = _wirelessSocketService.WirelessSocketList;

            InitializeComponent();

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

        public CollectionView WeekdayList
        {
            get
            {
                IList<string> weekdayList = new List<string>();
                foreach (Weekday entry in Enum.GetValues(typeof(Weekday)))
                {
                    if (entry > Weekday.Null)
                    {
                        weekdayList.Add(entry.ToString());
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
                IList<string> actionList = new List<string>();
                foreach (SocketAction entry in Enum.GetValues(typeof(SocketAction)))
                {
                    if (entry > SocketAction.Null)
                    {
                        actionList.Add(entry.ToString());
                    }
                }
                return new CollectionView(actionList);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            NameTextBox.Text = _scheduleName;
            InformationTextBox.Text = _scheduleInformation;
            SocketComboBox.ItemsSource = SocketList;
            WeekdayComboBox.ItemsSource = WeekdayList;
            TimePicker.SelectedTime = DateTime.Now;
            ActionComboBox.ItemsSource = ActionList;

            SocketComboBox.SelectedIndex = 1;
            WeekdayComboBox.SelectedIndex = (int)DateTime.Now.DayOfWeek;
            ActionComboBox.SelectedIndex = 1;
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

            int id = _schedulService.ScheduleList.Count;
            string scheduleName = NameTextBox.Text;

            ValidationResult scheduleNameResult = new TextBoxLengthRule().Validate(scheduleName, null);
            if (!scheduleNameResult.IsValid)
            {
                _notifier.ShowError("Please enter a valid schedule name!");
                return;
            }

            string scheduleInformation = InformationTextBox.Text;

            string socketName = SocketComboBox.Text;
            ValidationResult socketNameResult = new TextBoxLengthRule().Validate(socketName, null);
            if (!socketNameResult.IsValid)
            {
                _notifier.ShowError("Please select a socket!");
                return;
            }

            WirelessSocketDto socket = _wirelessSocketService.GetByName(socketName);

            int scheduleWeekdayIndex = WeekdayComboBox.SelectedIndex;
            Weekday scheduleWeekday = (Weekday)scheduleWeekdayIndex;

            DateTime? selectedScheduleTime = TimePicker.SelectedTime;
            DateTime scheduleTime = DateTime.Now;
            if (selectedScheduleTime != null)
            {
                scheduleTime = (DateTime)selectedScheduleTime;
            }

            int socketActionIndex = ActionComboBox.SelectedIndex;
            SocketAction socketAction = (SocketAction)socketActionIndex;

            ScheduleDto newSchedule = new ScheduleDto(id, scheduleName, scheduleInformation, socket, scheduleWeekday, scheduleTime, socketAction, true);

            _schedulService.OnAddScheduleFinished += _onAddScheduleFinished;
            _schedulService.AddSchedule(newSchedule);
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
