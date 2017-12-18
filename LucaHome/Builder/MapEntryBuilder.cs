using Common.Dto;
using Common.UserControls;
using Data.Services;
using LucaHome.Dialogs;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using static Common.Dto.MapContentDto;
using static Common.Dto.TemperatureDto;

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

                    SetDialog setDialog = new SetDialog(
                        socket.Name,
                        string.Format("Current State: {0}\nArea: {1}\nCode: {2}", socket.ActivationString, socket.Area, socket.Code),
                        "Activate",
                        "Deactivate");
                    setDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    setDialog.ShowDialog();

                    Action setAction = setDialog.SetAction;
                    if (setAction == Action.CONFIRM)
                    {
                        WirelessSocketService.Instance.SetWirelessSocket(socket, true);
                    }
                    else if (setAction == Action.CANCEL)
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

                    SetDialog setDialog = new SetDialog(
                        string.Format("{0}", wirelessSwitch.Name),
                        string.Format("Current Action: {0}\nArea: {1}\nKeyCode: {2}", wirelessSwitch.Action.ToString(), wirelessSwitch.Area, wirelessSwitch.KeyCode),
                        "Toggle",
                        "Cancel");
                    setDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    setDialog.ShowDialog();

                    Action setAction = setDialog.SetAction;
                    if (setAction == Action.CONFIRM)
                    {
                        WirelessSwitchService.Instance.ToggleWirelessSwitch(wirelessSwitch);
                    }
                    else if (setAction == Action.CANCEL)
                    {
                        // Do nothing
                    }
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
            double imageGridHeight = MapGrid.ActualHeight - 100;

            double marginLeft = imageGridWidth * entry.Position[0] / 100;
            double marginBottom = imageGridHeight * entry.Position[1] / 100;

            double marginRight = imageGridWidth - marginLeft - 35;
            double marginTop = imageGridHeight - marginBottom - 35;

            mapContentControl.Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);

            return mapContentControl;
        }
    }
}
