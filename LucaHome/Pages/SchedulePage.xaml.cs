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
    public partial class SchedulePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "SchedulePage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private string _scheduleSearchKey = string.Empty;
        private IList<ScheduleDto> _scheduleList = new List<ScheduleDto>();

        public SchedulePage(NavigationService navigationService)
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
                ScheduleList = ScheduleService.Instance.FoundSchedules(_scheduleSearchKey);
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
            ScheduleService.Instance.OnScheduleDownloadFinished += _scheduleListDownloadFinished;
            ScheduleService.Instance.OnSetScheduleFinished += _setScheduleFinished;

            if (ScheduleService.Instance.ScheduleList == null)
            {
                ScheduleService.Instance.LoadScheduleList();
                return;
            }

            ScheduleList = ScheduleService.Instance.ScheduleList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ScheduleService.Instance.OnScheduleDownloadFinished -= _scheduleListDownloadFinished;
            ScheduleService.Instance.OnSetScheduleFinished -= _setScheduleFinished;

            _notifier.Dispose();
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string scheduleName = (String)senderButton.Tag;
                ScheduleService.Instance.ChangeScheduleState(scheduleName);
            }
        }

        private void ButtonUpdateSchedule_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int scheduleId = (int)senderButton.Tag;
                ScheduleDto updateSchedule = ScheduleService.Instance.GetById(scheduleId);

                ScheduleUpdatePage scheduleUpdatePage = new ScheduleUpdatePage(_navigationService, updateSchedule);
                _navigationService.Navigate(scheduleUpdatePage);
            }
        }

        private void ButtonDeleteSchedule_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int scheduleId = (int)senderButton.Tag;
                ScheduleDto deleteSchedule = ScheduleService.Instance.GetById(scheduleId);

                DeleteDialog scheduleDeleteDialog = new DeleteDialog("Delete socket?",
                    string.Format("Schedule: {0}", deleteSchedule.Name));
                scheduleDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                scheduleDeleteDialog.ShowDialog();

                var confirmDelete = scheduleDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    ScheduleService.Instance.DeleteSchedule(deleteSchedule);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new ScheduleAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            ScheduleService.Instance.LoadScheduleList();
        }

        private void _scheduleListDownloadFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            ScheduleList = ScheduleService.Instance.ScheduleList;
        }

        private void _setScheduleFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
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
