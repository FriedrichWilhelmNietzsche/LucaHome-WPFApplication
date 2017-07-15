using Common.Common;
using Common.Dto;
using Common.Tools;
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
    public partial class SchedulePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "SchedulePage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly ScheduleService _scheduleService;

        private readonly Notifier _notifier;

        private string _scheduleSearchKey = string.Empty;
        private IList<ScheduleDto> _scheduleList = new List<ScheduleDto>();

        private readonly ScheduleAddPage _scheduleAddPage;

        public SchedulePage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _scheduleService = ScheduleService.Instance;

            _scheduleAddPage = new ScheduleAddPage(_navigationService);

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

        public string ScheduleSearchKey
        {
            get
            {
                return _scheduleSearchKey;
            }
            set
            {
                _scheduleSearchKey = value;
                OnPropertyChanged("ScheduleSearchKey");

                if (_scheduleSearchKey != string.Empty)
                {
                    ScheduleList = _scheduleService.FoundSchedules(_scheduleSearchKey);
                }
                else
                {
                    ScheduleList = _scheduleService.ScheduleList;
                }
            }
        }

        public IList<ScheduleDto> ScheduleList
        {
            get
            {
                return _scheduleList;
            }
            set
            {
                _scheduleList = value;
                OnPropertyChanged("ScheduleList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _scheduleService.OnScheduleDownloadFinished += _scheduleListDownloadFinished;
            _scheduleService.OnSetScheduleFinished += _setScheduleFinished;

            if (_scheduleService.ScheduleList == null)
            {
                _scheduleService.LoadScheduleList();
                return;
            }

            ScheduleList = _scheduleService.ScheduleList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _scheduleService.OnScheduleDownloadFinished -= _scheduleListDownloadFinished;
            _scheduleService.OnSetScheduleFinished -= _setScheduleFinished;

            _notifier.Dispose();
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string scheduleName = (String)senderButton.Tag;
                _scheduleService.ChangeScheduleState(scheduleName);
            }
        }

        private void ButtonUpdateSchedule_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonUpdateSchedule_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int scheduleId = (int)senderButton.Tag;
                ScheduleDto updateSchedule = _scheduleService.GetById(scheduleId);
                _logger.Warning(string.Format("Updating schedule {0}!", updateSchedule));

                ScheduleUpdatePage scheduleUpdatePage = new ScheduleUpdatePage(_navigationService, updateSchedule);
                _navigationService.Navigate(scheduleUpdatePage);
            }
        }

        private void ButtonDeleteSchedule_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonDeleteSchedule_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int scheduleId = (int)senderButton.Tag;
                ScheduleDto deleteSchedule = _scheduleService.GetById(scheduleId);
                _logger.Warning(string.Format("Asking for deleting schedule {0}!", deleteSchedule));

                DeleteDialog scheduleDeleteDialog = new DeleteDialog("Delete socket?",
                    string.Format("Schedule: {0}\nInformation: {1}", deleteSchedule.Name, deleteSchedule.Information));
                scheduleDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                scheduleDeleteDialog.ShowDialog();

                var confirmDelete = scheduleDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    _scheduleService.DeleteSchedule(deleteSchedule);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonAdd_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.Navigate(_scheduleAddPage);
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _scheduleService.LoadScheduleList();
        }

        private void _scheduleListDownloadFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            _logger.Debug(string.Format("_scheduleListDownloadFinished with model {0} was successful: {1}", scheduleList, success));
            ScheduleList = _scheduleService.ScheduleList;
        }

        private void _setScheduleFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            _logger.Debug(string.Format("_setScheduleFinished was successful: {0}", success));
            if (success)
            {
                _notifier.ShowSuccess("Successfully set socket");
            }
            else
            {
                _notifier.ShowError(string.Format("Failed to set schedule\n{0}", response));
            }
        }
    }
}
