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
    public partial class BirthdayAddPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "BirthdayAddPage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private BirthdayDto _newBirthday;

        public BirthdayAddPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            _newBirthday = new BirthdayDto(BirthdayService.Instance.BirthdayList.Count, "", true, false, DateTime.Now);

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
                return _newBirthday.Name;
            }
            set
            {
                _newBirthday.Name = value;
                OnPropertyChanged("BirthdayName");
            }
        }

        public DateTime BirthdayDate
        {
            get
            {
                return _newBirthday.Birthday;
            }
            set
            {
                _newBirthday.Birthday = value;
                OnPropertyChanged("BirthdayDate");
            }
        }

        public bool BirthdayRemindMe
        {
            get
            {
                return _newBirthday.RemindMe;
            }
            set
            {
                _newBirthday.RemindMe = value;
                OnPropertyChanged("BirthdayRemindMe");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            BirthdayService.Instance.OnBirthdayAddFinished -= _onBirthdayAddFinished;
            BirthdayService.Instance.OnBirthdayDownloadFinished -= _onBirthdayDownloadFinished;
        }

        private void SaveBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            BirthdayService.Instance.OnBirthdayAddFinished += _onBirthdayAddFinished;
            BirthdayService.Instance.AddBirthday(_newBirthday);
        }

        private void _onBirthdayAddFinished(bool success, string response)
        {
            BirthdayService.Instance.OnBirthdayAddFinished -= _onBirthdayAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new birthday!");

                BirthdayService.Instance.OnBirthdayDownloadFinished += _onBirthdayDownloadFinished;
                BirthdayService.Instance.LoadBirthdayList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding birthday failed!\n{0}", response));
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
