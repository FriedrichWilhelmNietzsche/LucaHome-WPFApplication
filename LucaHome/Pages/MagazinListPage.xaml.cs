using Data.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Common.Dto;
using System.Windows.Media.Imaging;

namespace LucaHome.Pages
{
    public partial class MagazinListPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MagazinListPage";

        private readonly NavigationService _navigationService;

        private string _magazinListSearchKey = string.Empty;
        private MagazinDirDto _magazinDirDto;
        private IList<string> _magazinList = new List<string>();

        public MagazinListPage(NavigationService navigationService, MagazinDirDto magazinDirDto)
        {
            _navigationService = navigationService;

            _magazinDirDto = magazinDirDto;
            _magazinList = _magazinDirDto.DirContent;

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string MagazinListSearchKey
        {
            get
            {
                return _magazinListSearchKey;
            }
            set
            {
                _magazinListSearchKey = value;
                OnPropertyChanged("MagazinListSearchKey");

                if (_magazinListSearchKey != string.Empty)
                {
                    MagazinList = _magazinDirDto.DirContent
                        .Where(entry => entry.Contains(_magazinListSearchKey))
                        .Select(entry => entry).ToList();
                }
                else
                {
                    MagazinList = _magazinDirDto.DirContent;
                }
            }
        }

        public string DirName
        {
            get
            {
                return _magazinDirDto.DirName;
            }
        }

        public BitmapImage MagazinIcon
        {
            get
            {
                return _magazinDirDto.Icon;
            }
        }

        public IList<string> MagazinList
        {
            get
            {
                return _magazinList;
            }
            set
            {
                _magazinList = value;
                OnPropertyChanged("MagazinList");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string magazinTitle = (string)senderButton.Tag;
                LibraryService.Instance.StartReading(_magazinDirDto.DirName, magazinTitle);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
