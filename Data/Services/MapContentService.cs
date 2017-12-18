using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Timers;

namespace Data.Services
{
    public delegate void MapContentDownloadEventHandler(IList<MapContentDto> mapContentList, bool success, string response);

    public class MapContentService
    {
        private const string TAG = "MapContentService";
        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static MapContentService _instance = null;
        private static readonly object _padlock = new object();

        private IList<MapContentDto> _mapContentList = new List<MapContentDto>();

        private Timer _downloadTimer;

        MapContentService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _mapContentDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event MapContentDownloadEventHandler OnMapContentDownloadFinished;
        private void publishOnMapContentDownloadFinished(IList<MapContentDto> mapContentList, bool success, string response)
        {
            OnMapContentDownloadFinished?.Invoke(mapContentList, success, response);
        }

        public static MapContentService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MapContentService();
                    }

                    return _instance;
                }
            }
        }

        public IList<MapContentDto> MapContentList
        {
            get
            {
                return _mapContentList;
            }
        }

        public MapContentDto GetById(int id)
        {
            MapContentDto foundMapContent = _mapContentList
                        .Where(mapContent => mapContent.Id == id)
                        .Select(mapContent => mapContent)
                        .FirstOrDefault();

            return foundMapContent;
        }

        public IList<MapContentDto> FoundapContents(string searchKey)
        {
            List<MapContentDto> foundMapContents = _mapContentList
                        .Where(mapContent =>
                            mapContent.Id.ToString().Contains(searchKey)
                            || mapContent.MapDrawingType.ToString().Contains(searchKey)
                            || mapContent.DrawingTypeId.ToString().Contains(searchKey)
                            || mapContent.Position.ToString().Contains(searchKey)
                            || mapContent.Name.Contains(searchKey)
                            || mapContent.ShortName.Contains(searchKey)
                            || mapContent.Area.Contains(searchKey)
                            || mapContent.ButtonVisibility.ToString().Contains(searchKey)
                            || mapContent.ListedMenuList.ToString().Contains(searchKey)
                            || mapContent.MenuList.ToString().Contains(searchKey)
                            || mapContent.ShoppingList.ToString().Contains(searchKey)
                            || mapContent.MediaServer.ToString().Contains(searchKey)
                            || mapContent.Security.ToString().Contains(searchKey)
                            || mapContent.Temperature.ToString().Contains(searchKey)
                            || mapContent.WirelessSocket.ToString().Contains(searchKey)
                            || mapContent.WirelessSwitch.ToString().Contains(searchKey)
                            || mapContent.ButtonToolTip.Contains(searchKey))
                        .Select(mapContent => mapContent)
                        .ToList();

            return foundMapContents;
        }

        public void LoadMapContentList()
        {
            Logger.Instance.Debug(TAG, "LoadMapContentList");
            loadMapContentListAsync();
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Logger.Instance.Debug(TAG, string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadMapContentListAsync();
        }

        private async Task loadMapContentListAsync()
        {
            Logger.Instance.Debug(TAG, "loadMapContentListAsync");

            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMapContentDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_MAP_CONTENTS.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MapContent);
        }

        private void _mapContentDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            Logger.Instance.Debug(TAG, "_mapContentDownloadFinished");

            if (downloadType != DownloadType.MapContent)
            {
                Logger.Instance.Debug(TAG, string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);

                publishOnMapContentDownloadFinished(null, false, response);
                return;
            }

            Logger.Instance.Debug(TAG, string.Format("response: {0}", response));

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");

                publishOnMapContentDownloadFinished(null, false, response);
                return;
            }

            IList<MapContentDto> mapContentList = JsonDataToMapContentConverter.Instance.GetList(
                response,
                MenuService.Instance.ListedMenuList,
                MenuService.Instance.MenuList,
                ShoppingListService.Instance.ShoppingList,
                /* TODO add MediaServerData */
                new List<MediaServerDto>(),
                SecurityService.Instance.Security,
                TemperatureService.Instance.TemperatureList,
                WirelessSocketService.Instance.WirelessSocketList,
                WirelessSwitchService.Instance.WirelessSwitchList);

            if (mapContentList == null)
            {
                Logger.Instance.Error(TAG, "Converted mapContentList is null!");

                publishOnMapContentDownloadFinished(null, false, response);
                return;
            }

            _mapContentList = mapContentList;

            publishOnMapContentDownloadFinished(_mapContentList, true, response);
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _mapContentDownloadFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
