using Common.Dto;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows;

namespace Common.Converter
{
    public class JsonDataToMapContentConverter
    {
        private const string TAG = "JsonDataToMapContentConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToMapContentConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToMapContentConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToMapContentConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToMapContentConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<MapContentDto> GetList(
            string[] stringArray,
            IList<ListedMenuDto> listedMenuList, IList<MenuDto> menuList, IList<ShoppingEntryDto> shoppingList,
            IList<MediaServerDto> mediaServerList, SecurityDto security, IList<TemperatureDto> temperatureList,
            IList<WirelessSocketDto> wirelessSocketList, IList<WirelessSwitchDto> wirelessSwitchList)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(
                    stringArray[0],
                    listedMenuList, menuList, shoppingList,
                    mediaServerList, security, temperatureList,
                    wirelessSocketList, wirelessSwitchList);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(
                    usedEntry,
                    listedMenuList, menuList, shoppingList,
                    mediaServerList, security, temperatureList,
                    wirelessSocketList, wirelessSwitchList);
            }
        }

        public IList<MapContentDto> GetList(
            string jsonString,
            IList<ListedMenuDto> listedMenuList, IList<MenuDto> menuList, IList<ShoppingEntryDto> shoppingList,
            IList<MediaServerDto> mediaServerList, SecurityDto security, IList<TemperatureDto> temperatureList,
            IList<WirelessSocketDto> wirelessSocketList, IList<WirelessSwitchDto> wirelessSwitchList)
        {
            return parseStringToList(
                jsonString,
                listedMenuList, menuList, shoppingList,
                mediaServerList, security, temperatureList,
                wirelessSocketList, wirelessSwitchList);
        }

        private IList<MapContentDto> parseStringToList(
            string value,
            IList<ListedMenuDto> listedMenuList, IList<MenuDto> menuList, IList<ShoppingEntryDto> shoppingList,
            IList<MediaServerDto> mediaServerList, SecurityDto security, IList<TemperatureDto> temperatureList,
            IList<WirelessSocketDto> wirelessSocketList, IList<WirelessSwitchDto> wirelessSwitchList)
        {
            if (!value.Contains("Error"))
            {
                IList<MapContentDto> mapContentList = new List<MapContentDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken mapContentJsonData = child["MapContent"];

                    int id = int.Parse(mapContentJsonData["ID"].ToString());

                    string type = mapContentJsonData["Type"].ToString();
                    MapContentDto.DrawingType drawingType;
                    switch (type)
                    {
                        case "WirelessSocket":
                            drawingType = MapContentDto.DrawingType.Socket;
                            break;
                        case "LAN":
                            drawingType = MapContentDto.DrawingType.LAN;
                            break;
                        case "MediaServer":
                            drawingType = MapContentDto.DrawingType.MediaServer;
                            break;
                        case "RaspberryPi":
                            drawingType = MapContentDto.DrawingType.RaspberryPi;
                            break;
                        case "NAS":
                            drawingType = MapContentDto.DrawingType.NAS;
                            break;
                        case "LightSwitch":
                            drawingType = MapContentDto.DrawingType.LightSwitch;
                            break;
                        case "Temperature":
                            drawingType = MapContentDto.DrawingType.Temperature;
                            break;
                        case "PuckJS":
                            drawingType = MapContentDto.DrawingType.PuckJS;
                            break;
                        case "Menu":
                            drawingType = MapContentDto.DrawingType.Menu;
                            break;
                        case "ShoppingList":
                            drawingType = MapContentDto.DrawingType.ShoppingList;
                            break;
                        case "Camera":
                            drawingType = MapContentDto.DrawingType.Camera;
                            break;
                        default:
                            drawingType = MapContentDto.DrawingType.Null;
                            break;
                    }
                    int typeId = int.Parse(mapContentJsonData["TypeId"].ToString());

                    string name = mapContentJsonData["Name"].ToString();
                    string shortName = mapContentJsonData["ShortName"].ToString();
                    string area = mapContentJsonData["Area"].ToString();

                    Visibility visibility = mapContentJsonData["Visibility"].ToString() == "1" ? Visibility.Visible : Visibility.Collapsed;

                    JToken positionJsonData = mapContentJsonData["Position"];
                    JToken pointJsonData = positionJsonData["Point"];

                    int positionX = int.Parse(pointJsonData["X"].ToString());
                    int positionY = int.Parse(pointJsonData["Y"].ToString());

                    int[] position = new int[] { positionX, positionY };

                    IList<ListedMenuDto> _listedMenuList = ((name == "ListedMenu" && drawingType == MapContentDto.DrawingType.Menu) ? listedMenuList : null);
                    IList<MenuDto> _menuList = ((name == "Menu" && drawingType == MapContentDto.DrawingType.Menu) ? menuList : null);
                    IList<ShoppingEntryDto> _shoppingList = ((name == "ShoppingList" && drawingType == MapContentDto.DrawingType.ShoppingList) ? shoppingList : null);

                    MediaServerDto _mediaServer = drawingType == MapContentDto.DrawingType.MediaServer ? (mediaServerList.Count > 0 ? mediaServerList[typeId - 1] : null) : null;
                    SecurityDto _security = drawingType == MapContentDto.DrawingType.Camera ? security : null;

                    // TODO Fix temperature selection
                    // TemperatureDto temperature = drawingType == MapContentDto.DrawingType.Temperature ? (temperatureList.Count > 0 ? temperatureList[typeId - 1] : null) : null;
                    TemperatureDto _temperature = new TemperatureDto(-273.15, "SPACE", new System.DateTime(), "", TemperatureDto.TemperatureType.DUMMY, "");
                    WirelessSocketDto _wirelessSocket = drawingType == MapContentDto.DrawingType.Socket ? (wirelessSocketList.Count > 0 ? wirelessSocketList[typeId - 1] : null) : null;
                    WirelessSwitchDto _wirelessSwitch = drawingType == MapContentDto.DrawingType.LightSwitch ? (wirelessSwitchList.Count > 0 ? wirelessSwitchList[typeId - 1] : null) : null;

                    MapContentDto newMapContent = new MapContentDto(id, drawingType, typeId, position, name, shortName, area, visibility,
                        _listedMenuList, _menuList, _shoppingList, _mediaServer, _security, _temperature, _wirelessSocket, _wirelessSwitch);

                    mapContentList.Add(newMapContent);
                }

                return mapContentList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<MapContentDto>();
        }
    }
}
