using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using LucaHome.Rules;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LucaHome.Pages
{
    public partial class BirthdayAddPage : Page
    {
        private const string TAG = "BirthdayAddPage";
        private readonly Logger _logger;

        private readonly BirthdayService _birthdayService;
        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private string _birthdayName = string.Empty;
        private DateTime _birthdayDate = DateTime.Now;

        public BirthdayAddPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _birthdayService = BirthdayService.Instance;
            _navigationService = navigationService;

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

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            NameTextBox.Text = _birthdayName;
            BirthdayDatePicker.SelectedDate = _birthdayDate;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _birthdayService.OnBirthdayAddFinished -= _onBirthdayAddFinished;
            _birthdayService.OnBirthdayDownloadFinished -= _onBirthdayDownloadFinished;
        }

        private void SaveBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("SaveBirthday_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            string birthdayName = NameTextBox.Text;

            ValidationResult birthdayNameResult = new TextBoxLengthRule().Validate(birthdayName, null);
            if (!birthdayNameResult.IsValid)
            {
                _notifier.ShowError("Please enter a valid birthday name!");
                return;
            }

            int id = _birthdayService.BirthdayList.Count;
            DateTime? birthdayDate = BirthdayDatePicker.SelectedDate;

            if (birthdayDate.HasValue)
            {
                BirthdayDto newBirthday = new BirthdayDto(id, birthdayName, (DateTime)birthdayDate);
                _logger.Debug(string.Format("Trying to add new birthday {0}", newBirthday));

                _birthdayService.OnBirthdayAddFinished += _onBirthdayAddFinished;
                _birthdayService.AddBirthday(newBirthday);
            }
        }

        private void _onBirthdayAddFinished(bool success)
        {
            _logger.Debug(string.Format("_onBirthdayAddFinished was successful {0}", success));

            _birthdayService.OnBirthdayAddFinished -= _onBirthdayAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new birthday!");

                _birthdayService.OnBirthdayDownloadFinished += _onBirthdayDownloadFinished;
                _birthdayService.LoadBirthdayList();
            }
            else
            {
                _notifier.ShowError("Adding birthday failed!");
            }
        }

        private void _onBirthdayDownloadFinished(IList<BirthdayDto> birthdayList, bool success)
        {
            _logger.Debug(string.Format("_onBirthdayDownloadFinished with model {0} was successful {1}", birthdayList, success));

            _birthdayService.OnBirthdayDownloadFinished -= _onBirthdayDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }
    }
}
