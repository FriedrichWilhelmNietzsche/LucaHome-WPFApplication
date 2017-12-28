using Common.Dto;
using Common.Tools;
using Common.UserControls;
using Data.Services;
using LucaHome.Dialogs;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            mapContentControl.ButtonVisibility = entry.ButtonVisibility;

            mapContentControl.ButtonCommand = getButtonCommand(entry);
            mapContentControl.ButtonToolTip = getButtonToolTip(entry);
            mapContentControl.ButtonEnabled = getButtonEnabled(entry);

            mapContentControl.Margin = getButtonMargin(entry, MapGrid);
            mapContentControl.ButtonForeground = getButtonForeground(entry.MapDrawingType);
            mapContentControl.ButtonBackground = getButtonBackground(entry.MapDrawingType);

            return mapContentControl;
        }

        private static ICommand getButtonCommand(MapContentDto entry)
        {
            switch (entry.MapDrawingType)
            {
                case DrawingType.Socket:
                    return new DelegateCommand(() =>
                    {
                        WirelessSocketDto socket = entry.WirelessSocket;

                        SetDialog setDialog = new SetDialog(
                            socket.Name,
                            string.Format("Current State: {0}\nArea: {1}\nCode: {2}", socket.ActivationString, socket.Area, socket.Code),
                            "Activate",
                            "Deactivate");
                        setDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        setDialog.ShowDialog();

                        DialogAction setDialogAction = setDialog.SetDialogAction;
                        if (setDialogAction == DialogAction.CONFIRM)
                        {
                            WirelessSocketService.Instance.SetWirelessSocket(socket, true);
                        }
                        else if (setDialogAction == DialogAction.CANCEL)
                        {
                            WirelessSocketService.Instance.SetWirelessSocket(socket, false);
                        }
                    });

                case DrawingType.LAN:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for a lan"); });
                case DrawingType.MediaServer:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for a mediaserver"); });
                case DrawingType.RaspberryPi:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for a raspberry pi"); });
                case DrawingType.NAS:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for a nas"); });

                case DrawingType.LightSwitch:
                    return new DelegateCommand(() =>
                    {
                        WirelessSwitchDto wirelessSwitch = entry.WirelessSwitch;

                        SetDialog setDialog = new SetDialog(
                            string.Format("{0}", wirelessSwitch.Name),
                            string.Format("Current Action: {0}\nArea: {1}\nKeyCode: {2}", wirelessSwitch.Action.ToString(), wirelessSwitch.Area, wirelessSwitch.KeyCode),
                            "Toggle",
                            "Cancel");
                        setDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        setDialog.ShowDialog();

                        DialogAction setDialogAction = setDialog.SetDialogAction;
                        if (setDialogAction == DialogAction.CONFIRM)
                        {
                            WirelessSwitchService.Instance.ToggleWirelessSwitch(wirelessSwitch);
                        }
                        else if (setDialogAction == DialogAction.CANCEL)
                        {
                            // Do nothing
                        }
                    });

                case DrawingType.Temperature:
                    return new DelegateCommand(() =>
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

                case DrawingType.PuckJS:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for a puck js"); });
                case DrawingType.Menu:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for the menu"); });
                case DrawingType.ShoppingList:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for the shopping list"); });
                case DrawingType.Camera:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for the camera"); });
                case DrawingType.Meter:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, "There is currently no command for the meter"); });
                case DrawingType.Null:
                default:
                    return new DelegateCommand(() => { Logger.Instance.Warning(TAG, string.Format("No valid command found for type {0}", entry.MapDrawingType)); });
            }
        }

        private static string getButtonToolTip(MapContentDto entry)
        {
            string drawingType = "";

            switch (entry.MapDrawingType)
            {
                case DrawingType.Socket:
                    drawingType = "Socket";
                    break;
                case DrawingType.LAN:
                    drawingType = "LAN";
                    break;
                case DrawingType.MediaServer:
                    drawingType = "MediaServer";
                    break;
                case DrawingType.RaspberryPi:
                    drawingType = "RaspberryPi";
                    break;
                case DrawingType.NAS:
                    drawingType = "NAS";
                    break;
                case DrawingType.LightSwitch:
                    drawingType = "LightSwitch";
                    break;
                case DrawingType.Temperature:
                    drawingType = "Temperature";
                    break;
                case DrawingType.PuckJS:
                    drawingType = "PuckJS";
                    break;
                case DrawingType.Menu:
                    drawingType = "Menu";
                    break;
                case DrawingType.ShoppingList:
                    drawingType = "ShoppingList";
                    break;
                case DrawingType.Camera:
                    drawingType = "Camera";
                    break;
                case DrawingType.Meter:
                    drawingType = "Meter";
                    break;
                case DrawingType.Null:
                default:
                    drawingType = "Null";
                    break;
            }

            return string.Format("Here is the {0} {1}", drawingType, entry.Name);
        }

        private static Thickness getButtonMargin(MapContentDto entry, Grid MapGrid)
        {
            double imageGridWidth = MapGrid.ActualWidth;
            double imageGridHeight = MapGrid.ActualHeight - 100;

            double marginLeft = imageGridWidth * entry.Position[0] / 100;
            double marginBottom = imageGridHeight * entry.Position[1] / 100;

            double marginRight = imageGridWidth - marginLeft - 35;
            double marginTop = imageGridHeight - marginBottom - 35;

            return new Thickness(marginLeft, marginTop, marginRight, marginBottom);
        }

        private static string getButtonForeground(DrawingType mapDrawingType)
        {
            switch (mapDrawingType)
            {
                case DrawingType.Socket:
                    return "white";
                case DrawingType.LAN:
                    return "black";
                case DrawingType.MediaServer:
                    return "black";
                case DrawingType.RaspberryPi:
                    return "black";
                case DrawingType.NAS:
                    return "white";
                case DrawingType.LightSwitch:
                    return "black";
                case DrawingType.Temperature:
                    return "black";
                case DrawingType.PuckJS:
                    return "white";
                case DrawingType.Menu:
                    return "black";
                case DrawingType.ShoppingList:
                    return "white";
                case DrawingType.Camera:
                    return "white";
                case DrawingType.Meter:
                    return "white";
                case DrawingType.Null:
                default:
                    return "black";
            }
        }

        private static string getButtonBackground(DrawingType mapDrawingType)
        {
            switch (mapDrawingType)
            {
                case DrawingType.Socket:
                    return "red";
                case DrawingType.LAN:
                    return "orange";
                case DrawingType.MediaServer:
                    return "darkslateblue";
                case DrawingType.RaspberryPi:
                    return "darkyellow";
                case DrawingType.NAS:
                    return "grey";
                case DrawingType.LightSwitch:
                    return "lightblue";
                case DrawingType.Temperature:
                    return "lightgreen";
                case DrawingType.PuckJS:
                    return "darkgreen";
                case DrawingType.Menu:
                    return "yellow";
                case DrawingType.ShoppingList:
                    return "purple";
                case DrawingType.Camera:
                    return "black";
                case DrawingType.Meter:
                    return "darkblue";
                case DrawingType.Null:
                default:
                    return "lightseagreen";
            }
        }

        private static bool getButtonEnabled(MapContentDto entry)
        {
            switch (entry.MapDrawingType)
            {
                case DrawingType.Socket:
                    return entry.WirelessSocket != null;
                case DrawingType.LAN:
                    return false;
                case DrawingType.MediaServer:
                    return entry.MediaServer != null;
                case DrawingType.RaspberryPi:
                    return false;
                case DrawingType.NAS:
                    return false;
                case DrawingType.LightSwitch:
                    return entry.WirelessSwitch != null;
                case DrawingType.Temperature:
                    return entry.Temperature != null;
                case DrawingType.PuckJS:
                    return false;
                case DrawingType.Menu:
                    return entry.ListedMenuList != null || entry.MenuList != null;
                case DrawingType.ShoppingList:
                    return entry.ShoppingList != null;
                case DrawingType.Camera:
                    return entry.Security != null;
                case DrawingType.Meter:
                    return false;
                case DrawingType.Null:
                default:
                    return false;
            }
        }
    }
}
