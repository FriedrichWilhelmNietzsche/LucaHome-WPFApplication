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
        private readonly Logger _logger;

        private const int TIMEOUT = 60 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToMapContentConverter _jsonDataToMapContentConverter;
        private readonly ScheduleService _scheduleService;
        private readonly TemperatureService _temperatureService;
        private readonly WirelessSocketService _wirelessSocketService;

        private static MapContentService _instance = null;
        private static readonly object _padlock = new object();

        private IList<MapContentDto> _mapContentList = new List<MapContentDto>();

        private Timer _downloadTimer;

        MapContentService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToMapContentConverter = new JsonDataToMapContentConverter();
            _scheduleService = ScheduleService.Instance;
            _temperatureService = TemperatureService.Instance;
            _wirelessSocketService = WirelessSocketService.Instance;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event MapContentDownloadEventHandler OnMapContentDownloadFinished;

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
                            || mapContent.Position.ToString().Contains(searchKey)
                            || mapContent.MapDrawingType.ToString().Contains(searchKey)
                            || mapContent.Area.Contains(searchKey)
                            || mapContent.Temperature.ToString().Contains(searchKey)
                            || mapContent.Socket.ToString().Contains(searchKey)
                            || mapContent.ScheduleList.ToString().Contains(searchKey)
                            || mapContent.ButtonVisibility.ToString().Contains(searchKey))
                        .Select(mapContent => mapContent)
                        .ToList();

            return foundMapContents;
        }

        public void LoadMapContentList()
        {
            _logger.Debug("LoadMapContentList");
            loadMapContentListAsync();
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadMapContentListAsync();
        }

        private async Task loadMapContentListAsync()
        {
            _logger.Debug("loadMapContentListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnMapContentDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_MAP_CONTENTS.Action;

            _downloadController.OnDownloadFinished += _mapContentDownloadFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MapContent);
        }

        private void _mapContentDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_mapContentDownloadFinished");

            if (downloadType != DownloadType.MapContent)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _mapContentDownloadFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnMapContentDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnMapContentDownloadFinished(null, false, response);
                return;
            }

            IList<MapContentDto> mapContentList = _jsonDataToMapContentConverter.GetList(response, _temperatureService.TemperatureList, _wirelessSocketService.WirelessSocketList, _scheduleService.ScheduleList);
            if (mapContentList == null)
            {
                _logger.Error("Converted mapContentList is null!");

                OnMapContentDownloadFinished(null, false, response);
                return;
            }

            _mapContentList = mapContentList;

            OnMapContentDownloadFinished(_mapContentList, true, response);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _mapContentDownloadFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
