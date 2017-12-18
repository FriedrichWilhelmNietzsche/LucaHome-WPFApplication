using Common.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
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

        private readonly NavigationService _navigationService;

        public MapPage(NavigationService navigationService)
        {
            _navigationService = navigationService;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MapContentService.Instance.OnMapContentDownloadFinished += _onMapContentListDownloadFinished;

            if (MapContentService.Instance.MapContentList == null)
            {
                MapContentService.Instance.LoadMapContentList();
                return;
            }

            displayMapContent();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MapContentService.Instance.OnMapContentDownloadFinished -= _onMapContentListDownloadFinished;
        }

        private void _onMapContentListDownloadFinished(IList<MapContentDto> mapContentList, bool success, string response)
        {
            displayMapContent();
        }

        private void displayMapContent()
        {
            MapGrid.Children.Clear();

            foreach (MapContentDto entry in MapContentService.Instance.MapContentList)
            {
                MapContentControl mapContentControl = MapEntryBuilder.BuildMapContentControl(entry, MapGrid);
                MapGrid.Children.Add(mapContentControl);
                MapGrid.UpdateLayout();
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            MapContentService.Instance.LoadMapContentList();
        }
    }
}
