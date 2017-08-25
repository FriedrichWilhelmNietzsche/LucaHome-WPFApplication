using Common.Common;
using Common.Tools;
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
        private readonly Logger _logger;

        private readonly LibraryService _libraryService;
        private readonly NavigationService _navigationService;

        private string _magazinListSearchKey = string.Empty;
        private MagazinDirDto _magazinDirDto;
        private IList<string> _magazinList = new List<string>();

        public MagazinListPage(NavigationService navigationService, MagazinDirDto magazinDirDto)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _libraryService = LibraryService.Instance;
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
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string magazinTitle = (string)senderButton.Tag;
                _libraryService.StartReading(_magazinDirDto.DirName, magazinTitle);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }
    }
}
