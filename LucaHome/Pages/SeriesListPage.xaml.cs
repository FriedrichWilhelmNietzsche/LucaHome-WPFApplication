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

        private readonly NavigationService _navigationService;

        private SeriesDto _seriesDto;
        private IList<SeasonDto> _seasonList = new List<SeasonDto>();

        public SeriesListPage(NavigationService navigationService, SeriesDto seriesDto)
        {
            _navigationService = navigationService;

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
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                SeasonDto seasonDto = (SeasonDto)senderButton.Tag;

                SeasonDialog seasonDialog = new SeasonDialog(seasonDto);
                seasonDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                seasonDialog.ShowDialog();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string season = (string)senderButton.Tag;
                SeriesService.Instance.OpenExplorer(_seriesDto.SeriesName, season);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
