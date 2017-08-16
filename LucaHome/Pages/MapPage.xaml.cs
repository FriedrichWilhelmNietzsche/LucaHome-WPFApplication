using Common.Common;
using Common.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using OpenWeather.Service;
using Data.Services;
using Common.Dto;
using Common.UserControls;
using System.Collections.Generic;
using LucaHome.Builder;

namespace LucaHome.Pages
{
    public partial class MapPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MapPage";
        private readonly Logger _logger;

        private readonly MapContentService _mapContentService;
        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly TemperatureService _temperatureService;
        private readonly WirelessSocketService _wirelessSocketService;

        public MapPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _mapContentService = MapContentService.Instance;
            _navigationService = navigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _temperatureService = TemperatureService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} with arguments {1}", sender, routedEventArgs));

            _mapContentService.OnMapContentDownloadFinished += _onMapContentListDownloadFinished;

            if (_mapContentService.MapContentList == null)
            {
                _mapContentService.LoadMapContentList();
                return;
            }

            displayMapContent();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} with arguments {1}", sender, routedEventArgs));
            _mapContentService.OnMapContentDownloadFinished -= _onMapContentListDownloadFinished;
        }

        private void _onMapContentListDownloadFinished(IList<MapContentDto> mapContentList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMapContentListDownloadFinished with model {0} was successful {1}", mapContentList, success));
            displayMapContent();
        }

        private void displayMapContent()
        {
            _logger.Debug("displayMapContent");

            MapGrid.Children.Clear();

            foreach (MapContentDto entry in _mapContentService.MapContentList)
            {
                MapContentControl mapContentControl = MapEntryBuilder.BuildMapContentControl(entry, MapGrid);
                MapGrid.Children.Add(mapContentControl);
                MapGrid.UpdateLayout();
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _mapContentService.LoadMapContentList();
        }
    }
}
