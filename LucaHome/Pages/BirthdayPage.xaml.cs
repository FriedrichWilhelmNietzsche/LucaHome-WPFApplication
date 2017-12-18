using Common.Dto;
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

        private readonly NavigationService _navigationService;

        private string _birthdaySearchKey = string.Empty;
        private IList<BirthdayDto> _birthdayList = new List<BirthdayDto>();

        public BirthdayPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

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
                BirthdayList = BirthdayService.Instance.FoundBirthdays(_birthdaySearchKey);
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
            BirthdayService.Instance.OnBirthdayDownloadFinished += _birthdayListDownloadFinished;

            if (BirthdayService.Instance.BirthdayList == null)
            {
                BirthdayService.Instance.LoadBirthdayList();
                return;
            }

            BirthdayList = BirthdayService.Instance.BirthdayList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            BirthdayService.Instance.OnBirthdayDownloadFinished -= _birthdayListDownloadFinished;
        }

        private void ButtonRemindMeBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is CheckBox)
            {
                CheckBox senderCheckBox = (CheckBox)sender;

                int birthdayId = (int)senderCheckBox.Tag;
                BirthdayDto updateBirthday = BirthdayService.Instance.GetById(birthdayId);

                updateBirthday.RemindMe = (bool)senderCheckBox.IsChecked;
                BirthdayService.Instance.UpdateBirthday(updateBirthday);
            }
        }

        private void ButtonUpdateBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int birthdayId = (int)senderButton.Tag;
                BirthdayDto updateBirthday = BirthdayService.Instance.GetById(birthdayId);

                BirthdayUpdatePage birthdayUpdatePage = new BirthdayUpdatePage(_navigationService, updateBirthday);
                _navigationService.Navigate(birthdayUpdatePage);
            }
        }

        private void ButtonDeleteBirthday_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int birthdayId = (int)senderButton.Tag;
                BirthdayDto deleteBirthday = BirthdayService.Instance.GetById(birthdayId);

                DeleteDialog birthdayDeleteDialog = new DeleteDialog("Delete birthday?",
                    string.Format("Birthday: {0}\nAge: {1}\nDate: {2}", deleteBirthday.Name, deleteBirthday.Age, deleteBirthday.Birthday));
                birthdayDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                birthdayDeleteDialog.ShowDialog();

                var confirmDelete = birthdayDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    BirthdayService.Instance.DeleteBirthday(deleteBirthday);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new BirthdayAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            BirthdayService.Instance.LoadBirthdayList();
        }

        private void _birthdayListDownloadFinished(IList<BirthdayDto> birthdayList, bool success, string response)
        {
            BirthdayList = BirthdayService.Instance.BirthdayList;
        }
    }
}
