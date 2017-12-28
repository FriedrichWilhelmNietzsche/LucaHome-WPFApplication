using Common.Dto;
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
    public partial class BirthdayUpdatePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "BirthdayUpdatePage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private BirthdayDto _updateBirthday;

        public BirthdayUpdatePage(NavigationService navigationService, BirthdayDto updateBirthday)
        {
            _navigationService = navigationService;

            _updateBirthday = updateBirthday;

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

        public string BirthdayName
        {
            get
            {
                return _updateBirthday.Name;
            }
            set
            {
                _updateBirthday.Name = value;
                OnPropertyChanged("BirthdayName");
            }
        }

        public string BirthdayGroup
        {
            get
            {
                return _updateBirthday.Group;
            }
            set
            {
                _updateBirthday.Group = value;
                OnPropertyChanged("BirthdayGroup");
            }
        }

        public DateTime BirthdayDate
        {
            get
            {
                return _updateBirthday.Birthday;
            }
            set
            {
                _updateBirthday.Birthday = value;
                OnPropertyChanged("BirthdayDate");
            }
        }

        public bool BirthdayRemindMe
        {
            get
            {
                return _updateBirthday.RemindMe;
            }
            set
            {
                _updateBirthday.RemindMe = value;
                OnPropertyChanged("BirthdayRemindMe");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            BirthdayService.Instance.OnBirthdayUpdateFinished -= _onBirthdayUpdateFinished;
            BirthdayService.Instance.OnBirthdayDownloadFinished -= _onBirthdayDownloadFinished;
        }

        private void UpdateBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            BirthdayService.Instance.OnBirthdayAddFinished += _onBirthdayUpdateFinished;
            BirthdayService.Instance.UpdateBirthday(_updateBirthday);
        }

        private void _onBirthdayUpdateFinished(bool success, string response)
        {
            BirthdayService.Instance.OnBirthdayAddFinished -= _onBirthdayUpdateFinished;

            if (success)
            {
                _notifier.ShowSuccess("Updated birthday!");

                BirthdayService.Instance.OnBirthdayDownloadFinished += _onBirthdayDownloadFinished;
                BirthdayService.Instance.LoadBirthdayList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating birthday failed!\n{0}", response));
            }
        }

        private void _onBirthdayDownloadFinished(IList<BirthdayDto> birthdayList, bool success, string response)
        {
            BirthdayService.Instance.OnBirthdayDownloadFinished -= _onBirthdayDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
