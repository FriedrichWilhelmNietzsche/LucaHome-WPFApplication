using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Common.Dto
{
    public class MapContentDto
    {
        public enum DrawingType { Null, Socket, LAN, MediaServer, RaspberryPi, NAS, LightSwitch, Temperature, PuckJS, Menu, ShoppingList, Camera }

        private const string TAG = "MapContentDto";
        private readonly Logger _logger;

        private int _id;
        private DrawingType _drawingType;
        private int _drawingTypeId;
        private int[] _position;
        private string _name;
        private string _shortName;
        private string _area;
        private Visibility _visibility;

        private IList<ListedMenuDto> _listedMenuList;
        private IList<MenuDto> _menuList;
        private IList<ShoppingEntryDto> _shoppingList;

        private MediaServerDto _mediaServer;
        private SecurityDto _security;
        private TemperatureDto _temperature;
        private WirelessSocketDto _wirelessSocket;
        private WirelessSwitchDto _wirelessSwitch;

        public MapContentDto(
            int id,
            DrawingType drawingType,
            int drawingTypeId,
            int[] position,
            string name,
            string shortName,
            string area,
            Visibility visibility,

            IList<ListedMenuDto> listedMenuList,
            IList<MenuDto> menuList,
            IList<ShoppingEntryDto> shoppingList,

            MediaServerDto mediaServer,
            SecurityDto security,
            TemperatureDto temperature,
            WirelessSocketDto wirelessSocket,
            WirelessSwitchDto wirelessSwitch)
        {
            _logger = new Logger(TAG);

            _id = id;
            _drawingType = drawingType;
            _drawingTypeId = drawingTypeId;

            _position = position;

            _name = name;
            _shortName = shortName;
            _area = area;

            _visibility = visibility;

            _listedMenuList = listedMenuList;
            _menuList = menuList;
            _shoppingList = shoppingList;

            _mediaServer = mediaServer;
            _security = security;
            _temperature = temperature;
            _wirelessSocket = wirelessSocket;
            _wirelessSwitch = wirelessSwitch;
        }

        public int Id
        {
            get
            {
                return _id;
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

        public int DrawingTypeId
        {
            get
            {
                return _drawingTypeId;
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

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string ShortName
        {
            get
            {
                return _shortName;
            }
            set
            {
                _shortName = value;
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

        public IList<ListedMenuDto> ListedMenuList
        {
            get
            {
                return _listedMenuList;
            }
        }

        public IList<MenuDto> MenuList
        {
            get
            {
                return _menuList;
            }
        }

        public IList<ShoppingEntryDto> ShoppingList
        {
            get
            {
                return _shoppingList;
            }
        }

        public MediaServerDto MediaServer
        {
            get
            {
                return _mediaServer;
            }
        }

        public SecurityDto Security
        {
            get
            {
                return _security;
            }
        }

        public TemperatureDto Temperature
        {
            get
            {
                return _temperature;
            }
        }

        public WirelessSocketDto WirelessSocket
        {
            get
            {
                return _wirelessSocket;
            }
        }

        public WirelessSwitchDto WirelessSwitch
        {
            get
            {
                return _wirelessSwitch;
            }
        }
        
        public string ButtonToolTip
        {
            get
            {
                string drawingType = "";

                switch (_drawingType)
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
                    case DrawingType.Null:
                    default:
                        drawingType = "Null";
                        break;
                }

                return string.Format("Here is the {0} {1}", drawingType, _name);
            }
        }

        public ICommand ButtonCommand
        {
            get
            {
                switch (_drawingType)
                {
                    case DrawingType.Socket:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a socket"); });
                    case DrawingType.LAN:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a lan"); });
                    case DrawingType.MediaServer:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a mediaserver"); });
                    case DrawingType.RaspberryPi:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a raspberry pi"); });
                    case DrawingType.NAS:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a nas"); });
                    case DrawingType.LightSwitch:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a lightswitch"); });
                    case DrawingType.Temperature:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a temperature"); });
                    case DrawingType.PuckJS:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for a puck js"); });
                    case DrawingType.Menu:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for the menu"); });
                    case DrawingType.ShoppingList:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for the shopping list"); });
                    case DrawingType.Camera:
                        return new DelegateCommand(() => { _logger.Debug("There is currently no command for the camera"); });
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
                        return _wirelessSocket != null;
                    case DrawingType.LAN:
                        return false;
                    case DrawingType.MediaServer:
                        return _mediaServer != null;
                    case DrawingType.RaspberryPi:
                        return false;
                    case DrawingType.NAS:
                        return false;
                    case DrawingType.LightSwitch:
                        return _wirelessSwitch != null;
                    case DrawingType.Temperature:
                        return _temperature != null;
                    case DrawingType.PuckJS:
                        return false;
                    case DrawingType.Menu:
                        return _listedMenuList != null || _menuList != null;
                    case DrawingType.ShoppingList:
                        return _shoppingList != null;
                    case DrawingType.Camera:
                        return _security != null;
                    case DrawingType.Null:
                    default:
                        return false;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Type: {2} );(Position: {3} );(Name: {4} );(ShortName: {5} );(Area: {6} ))", TAG, _id, _drawingType, _position, _name, _shortName, _area);
        }
    }
}
