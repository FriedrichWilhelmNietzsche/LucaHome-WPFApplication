using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using LucaHome.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 * https://stackoverflow.com/questions/2796470/wpf-create-a-dialog-prompt
 * https://stackoverflow.com/questions/1545258/changing-the-start-up-location-of-a-wpf-window
 */

namespace LucaHome.Pages
{
    public partial class BirthdayPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "BirthdayPage";
        private readonly Logger _logger;

        private readonly BirthdayService _birthdayService;
        private readonly NavigationService _navigationService;

        private string _birthdaySearchKey = string.Empty;
        private IList<BirthdayDto> _birthdayList = new List<BirthdayDto>();

        private readonly BirthdayAddPage _birthdayAddPage;

        public BirthdayPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _birthdayService = BirthdayService.Instance;
            _navigationService = navigationService;

            _birthdayAddPage = new BirthdayAddPage(_navigationService);

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string BirthdaySearchKey
        {
            get
            {
                return _birthdaySearchKey;
            }
            set
            {
                _birthdaySearchKey = value;
                OnPropertyChanged("BirthdaySearchKey");

                if (_birthdaySearchKey != string.Empty)
                {
                    BirthdayList = _birthdayService.FoundBirthdays(_birthdaySearchKey);
                }
                else
                {
                    BirthdayList = _birthdayService.BirthdayList;
                }
            }
        }

        public IList<BirthdayDto> BirthdayList
        {
            get
            {
                return _birthdayList;
            }
            set
            {
                _birthdayList = value;
                OnPropertyChanged("BirthdayList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _birthdayService.OnBirthdayDownloadFinished += _birthdayListDownloadFinished;

            if (_birthdayService.BirthdayList == null)
            {
                _birthdayService.LoadBirthdayList();
                return;
            }

            BirthdayList = _birthdayService.BirthdayList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _birthdayService.OnBirthdayDownloadFinished -= _birthdayListDownloadFinished;
        }

        private void ButtonUpdateBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonUpdateBirthday_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int birthdayId = (int)senderButton.Tag;
                BirthdayDto updateBirthday = _birthdayService.GetById(birthdayId);
                _logger.Warning(string.Format("Updating birthday {0}!", updateBirthday));

                BirthdayUpdatePage birthdayUpdatePage = new BirthdayUpdatePage(_navigationService, updateBirthday);
                _navigationService.Navigate(birthdayUpdatePage);
            }
        }

        private void ButtonDeleteBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonDeleteBirthday_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int birthdayId = (int)senderButton.Tag;
                BirthdayDto deleteBirthday = _birthdayService.GetById(birthdayId);
                _logger.Warning(string.Format("Asking for deleting birthday {0}!", deleteBirthday));

                DeleteDialog birthdayDeleteDialog = new DeleteDialog("Delete birthday?",
                    string.Format("Birthday: {0}\nAge: {1}\nDate: {2}", deleteBirthday.Name, deleteBirthday.Age, deleteBirthday.Birthday));
                birthdayDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                birthdayDeleteDialog.ShowDialog();

                var confirmDelete = birthdayDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    _birthdayService.DeleteBirthday(deleteBirthday);
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

            _navigationService.Navigate(_birthdayAddPage);
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _birthdayService.LoadBirthdayList();
        }

        private void _birthdayListDownloadFinished(IList<BirthdayDto> birthdayList, bool success, string response)
        {
            _logger.Debug(string.Format("_birthdayListDownloadFinished with model {0} was successful: {1}", birthdayList, success));
            BirthdayList = _birthdayService.BirthdayList;
        }
    }
}
