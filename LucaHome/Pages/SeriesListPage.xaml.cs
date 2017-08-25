using Common.Common;
using Common.Tools;
using Data.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Common.Dto;
using System.Windows.Media.Imaging;
using LucaHome.Dialogs;

namespace LucaHome.Pages
{
    public partial class SeriesListPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "SeriesListPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly SeriesService _seriesService;

        private SeriesDto _seriesDto;
        private IList<SeasonDto> _seasonList = new List<SeasonDto>();

        public SeriesListPage(NavigationService navigationService, SeriesDto seriesDto)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _seriesService = SeriesService.Instance;

            _seriesDto = seriesDto;
            _seasonList = _seriesDto.SeriesSeasons;

            InitializeComponent();
            DataContext = this;

            Wallpaper.ImageWallpaperSource = _seriesDto.Icon.UriSource;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SeriesName
        {
            get
            {
                return _seriesDto.SeriesName;
            }
        }

        public BitmapImage SeriesIcon
        {
            get
            {
                return _seriesDto.Icon;
            }
        }

        public IList<SeasonDto> SeasonList
        {
            get
            {
                return _seasonList;
            }
            set
            {
                _seasonList = value;
                OnPropertyChanged("SeasonDto");
            }
        }

        private void Episode_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                SeasonDto seasonDto = (SeasonDto)senderButton.Tag;

                SeasonDialog seasonDialog = new SeasonDialog(seasonDto);
                seasonDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                seasonDialog.ShowDialog();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string season = (string)senderButton.Tag;
                _seriesService.OpenExplorer(_seriesDto.SeriesName, season);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }
    }
}
