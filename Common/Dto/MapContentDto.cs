using System.Collections.Generic;
using System.Windows;

namespace Common.Dto
{
    public class MapContentDto
    {
        public enum DrawingType { Null, Socket, LAN, MediaServer, RaspberryPi, NAS, LightSwitch, Temperature, PuckJS, Menu, ShoppingList, Camera, Meter }

        private const string TAG = "MapContentDto";

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

        public override string ToString()
        {
            return string.Format("( {0}: (Id: {1} );(Type: {2} );(Position: {3} );(Name: {4} );(ShortName: {5} );(Area: {6} ))", TAG, _id, _drawingType, _position, _name, _shortName, _area);
        }
    }
}
