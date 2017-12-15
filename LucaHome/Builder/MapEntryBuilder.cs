using Common.Dto;
using Common.UserControls;
using Data.Services;
using LucaHome.Dialogs;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using static Common.Dto.MapContentDto;
using static Common.Dto.TemperatureDto;
using static LucaHome.Dialogs.SetDialog;

namespace LucaHome.Builder
{
    public static class MapEntryBuilder
    {
        private const string TAG = "MapEntryBuilder";

        public static MapContentControl BuildMapContentControl(MapContentDto entry, Grid MapGrid)
        {
            MapContentControl mapContentControl = new MapContentControl();

            mapContentControl.ButtonText = entry.ShortName;
            mapContentControl.ButtonToolTip = entry.ButtonToolTip;
            mapContentControl.ButtonVisibility = entry.ButtonVisibility;

            if (entry.MapDrawingType == DrawingType.Socket)
            {
                mapContentControl.ButtonCommand = new DelegateCommand(() =>
                {
                    WirelessSocketDto socket = entry.WirelessSocket;

                    SetDialog setDialog = new SetDialog(string.Format("Set socket {0}", socket.Name),
                        string.Format("Current State: {0}\nArea: {1}\nCode: {2}", socket.ActivationString, socket.Area, socket.Code));
                    setDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    setDialog.ShowDialog();

                    Action setAction = setDialog.SetAction;
                    if (setAction == Action.ACTIVATE)
                    {
                        WirelessSocketService.Instance.SetWirelessSocket(socket, true);
                    }
                    else if (setAction == Action.DEACTIVATE)
                    {
                        WirelessSocketService.Instance.SetWirelessSocket(socket, false);
                    }
                });
            }
            else if (entry.MapDrawingType == DrawingType.LightSwitch)
            {
                mapContentControl.ButtonCommand = new DelegateCommand(() =>
                {
                    WirelessSwitchDto wirelessSwitch = entry.WirelessSwitch;

                    ToggleDialog toggleDialog = new ToggleDialog(
                        string.Format("Toggle switch {0}", wirelessSwitch.Name),
                        string.Format("Current Action: {0}\nArea: {1}\nKeyCode: {2}", wirelessSwitch.Action.ToString(), wirelessSwitch.Area, wirelessSwitch.KeyCode));
                    toggleDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    toggleDialog.ShowDialog();

                    WirelessSwitchService.Instance.ToggleWirelessSwitch(wirelessSwitch);
                });
            }
            else if (entry.MapDrawingType == DrawingType.Temperature)
            {
                mapContentControl.ButtonCommand = new DelegateCommand(() =>
                {
                    TemperatureDto temperature = TemperatureService.Instance.GetByType(TemperatureType.RASPBERRY);
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

            return mapContentControl;
        }
    }
}
