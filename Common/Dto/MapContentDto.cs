using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Common.Dto
{
    public class MapContentDto
    {
        public enum DrawingType { Null, Raspberry, Arduino, Socket, Temperature, MediaServer, ShoppingList, Menu, Camera, PuckJS }

        private const string TAG = "MapContentDto";
        private readonly Logger _logger;

        private int _id;
        private int[] _position;
        private DrawingType _drawingType;
        private string _area;
        private TemperatureDto _temperature;
        private WirelessSocketDto _socket;
        private IList<ScheduleDto> _scheduleList;
        private Visibility _visibility;

        public MapContentDto(
            int id,
            int[] position,
            DrawingType drawingType,
            string area,
            TemperatureDto temperature,
            WirelessSocketDto socket,
            IList<ScheduleDto> scheduleList,
            Visibility visibility)
        {
            _logger = new Logger(TAG);

            _id = id;
            _position = position;
            _drawingType = drawingType;
            _area = area;
            _temperature = temperature;
            _socket = socket;
            _scheduleList = scheduleList;
            _visibility = visibility;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public int[] Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public DrawingType MapDrawingType
        {
            get
            {
                return _drawingType;
            }
            set
            {
                _drawingType = value;
            }
        }

        public string Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
            }
        }

        public TemperatureDto Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
            }
        }

        public WirelessSocketDto Socket
        {
            get
            {
                return _socket;
            }
            set
            {
                _socket = value;
            }
        }

        public IList<ScheduleDto> ScheduleList
        {
            get
            {
                return _scheduleList;
            }
            set
            {
                _scheduleList = value;
            }
        }

        public string ButtonText
        {
            get
            {
                switch (_drawingType)
                {
                    case DrawingType.Arduino:
                        return "";
                    case DrawingType.Camera:
                        return "Cam";
                    case DrawingType.MediaServer:
                        return "";
                    case DrawingType.Menu:
                        return "Menu";
                    case DrawingType.PuckJS:
                        return "PuckJS";
                    case DrawingType.Raspberry:
                        return "";
                    case DrawingType.ShoppingList:
                        return "S";
                    case DrawingType.Socket:
                        return _socket?.ShortName;
                    case DrawingType.Temperature:
                        return _temperature?.TemperatureString;
                    case DrawingType.Null:
                    default:
                        return "";
                }
            }
        }

        public string ButtonToolTip
        {
            get
            {
                switch (_drawingType)
                {
                    case DrawingType.Arduino:
                        return "Here is an arduino!";
                    case DrawingType.Camera:
                        return "Here is the camera!";
                    case DrawingType.MediaServer:
                        return "Here is a mediaServer!";
                    case DrawingType.Menu:
                        return "Here is the menu!";
                    case DrawingType.PuckJS:
                        return "Here is a PuckJS!";
                    case DrawingType.Raspberry:
                        return "Here is a raspberry!";
                    case DrawingType.ShoppingList:
                        return "Here is the shopping list!";
                    case DrawingType.Socket:
                        return string.Format("Here is the socket {0}", _socket?.Name);
                    case DrawingType.Temperature:
                        return string.Format("Here is the temperature at {0}", _area);
                    case DrawingType.Null:
                    default:
                        return "";
                }
            }
        }

        public Visibility ButtonVisibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
            }
        }

        public ICommand ButtonCommand
        {
            get
            {
                switch (_drawingType)
                {
                    case DrawingType.Arduino:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for an arduino"); });
                    case DrawingType.Camera:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for the camera"); });
                    case DrawingType.MediaServer:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a mediaserver"); });
                    case DrawingType.Menu:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for the menu"); });
                    case DrawingType.PuckJS:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a puck js"); });
                    case DrawingType.Raspberry:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a raspberry"); });
                    case DrawingType.ShoppingList:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for the shopping list"); });
                    case DrawingType.Socket:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a socket"); });
                    case DrawingType.Temperature:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a temperature"); });
                    case DrawingType.Null:
                    default:
                        return new DelegateCommand(() => { _logger.Debug(string.Format("No valid command found for type {0}", _drawingType)); });
                }
            }
        }

        public bool ButtonEnabled
        {
            get
            {
                switch (_drawingType)
                {
                    case DrawingType.Socket:
                        return _socket != null ? true : false;
                    case DrawingType.Temperature:
                        return true;
                    case DrawingType.Arduino:
                    case DrawingType.Camera:
                    case DrawingType.MediaServer:
                    case DrawingType.Menu:
                    case DrawingType.PuckJS:
                    case DrawingType.Raspberry:
                    case DrawingType.ShoppingList:
                    case DrawingType.Null:
                    default:
                        return false;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Position: {2} );(Type: {3} );(Area: {4} );(Temperature: {5} );(Socket: {6} );(ScheduleList: {7} );(ButtonVisibility: {8} ))", TAG, _id, _position, _drawingType, _area, _temperature, _socket, _scheduleList, _visibility);
        }
    }
}
