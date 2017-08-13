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
using static Common.Dto.MapContentDto;
using Microsoft.Practices.Prism.Commands;
using LucaHome.Dialogs;
using static LucaHome.Dialogs.SetDialog;
using static Common.Dto.TemperatureDto;

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
                MapContentControl mapContentControl = new MapContentControl();

                mapContentControl.ButtonText = entry.ButtonText;
                mapContentControl.ButtonToolTip = entry.ButtonToolTip;
                mapContentControl.ButtonVisibility = entry.ButtonVisibility;

                if (entry.MapDrawingType == DrawingType.Socket)
                {
                    mapContentControl.ButtonCommand = new DelegateCommand(() =>
                    {
                        WirelessSocketDto socket = entry.Socket;
                        _logger.Debug(string.Format("MapSocketButton_Command with wirelessSocket {0}", socket));

                        SetDialog setDialog = new SetDialog(string.Format("Set socket {0}", socket.Name),
                            string.Format("Current State: {0}\nArea: {1}\nCode: {2}", socket.ActivationString, socket.Area, socket.Code));
                        setDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        setDialog.ShowDialog();

                        Action setAction = setDialog.SetAction;
                        if (setAction == Action.ACTIVATE)
                        {
                            _logger.Debug("Please enable the socket!");
                            _wirelessSocketService.SetWirelessSocket(socket, true);
                        }
                        else if (setAction == Action.DEACTIVATE)
                        {
                            _logger.Debug("Please disable the socket!");
                            _wirelessSocketService.SetWirelessSocket(socket, false);
                        }
                        else
                        {
                            _logger.Debug("Please do nothing!");
                        }
                    });
                }
                else if (entry.MapDrawingType == DrawingType.Temperature)
                {
                    mapContentControl.ButtonCommand = new DelegateCommand(() =>
                    {
                        _logger.Debug("MapTemperatureButton_Command");

                        TemperatureDto temperature = _temperatureService.GetByType(TemperatureType.RASPBERRY);
                        if (temperature != null)
                        {
                            ConfirmDialog temperatureDialog = new ConfirmDialog(
                                string.Format("Area: {0}", temperature.Area),
                                string.Format("{0}\nTime: {1}", temperature.TemperatureString, temperature.LastUpdate));
                            temperatureDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            temperatureDialog.ShowDialog();
                        }
                    });
                }
                else
                {
                    mapContentControl.ButtonCommand = entry.ButtonCommand;
                }
                mapContentControl.ButtonEnabled = entry.ButtonEnabled;

                double imageGridWidth = MapGrid.ActualWidth;
                double imageGridHeight = MapGrid.ActualHeight;

                double marginRight = imageGridWidth * entry.Position[1] / 100;
                double marginTop = imageGridHeight * entry.Position[0] / 100;

                double marginLeft = imageGridWidth - marginRight - 35;
                double marginBottom = imageGridHeight - marginTop - 35;

                mapContentControl.Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);

                MapGrid.Children.Add(mapContentControl);
                MapGrid.UpdateLayout();
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }
    }
}
