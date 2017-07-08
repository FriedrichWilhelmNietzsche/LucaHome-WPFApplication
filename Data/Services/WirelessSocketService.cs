using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Services
{
    public delegate void WirelessSocketDownloadEventHandler(IList<WirelessSocketDto> wirelessSocketList, bool success);

    public class WirelessSocketService
    {
        private const string TAG = "WirelessSocketService";
        private readonly Logger _logger;

        private readonly AppSettingsService _appSettingsService;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToWirelessSocketConverter _jsonDataToWirelessSocketConverter;

        private static WirelessSocketService _instance = null;
        private static readonly object _padlock = new object();

        private IList<WirelessSocketDto> _wirelessSocketList = new List<WirelessSocketDto>();

        WirelessSocketService()
        {
            _logger = new Logger(TAG);

            _appSettingsService = AppSettingsService.Instance;
            _downloadController = new DownloadController();
            _jsonDataToWirelessSocketConverter = new JsonDataToWirelessSocketConverter();
        }

        public event WirelessSocketDownloadEventHandler OnWirelessSocketDownloadFinished;
        public event WirelessSocketDownloadEventHandler OnSetWirelessSocketFinished;

        public static WirelessSocketService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new WirelessSocketService();
                    }

                    return _instance;
                }
            }
        }

        public IList<WirelessSocketDto> WirelessSocketList
        {
            get
            {
                return _wirelessSocketList;
            }
        }

        public void LoadWirelessSocketList()
        {
            _logger.Debug("LoadWirelessSocketList");
            loadWirelessSocketListAsync();
        }

        public void SetWirelessSocket(WirelessSocketDto wirelessSocket, bool state)
        {
            _logger.Debug(string.Format("Set socket {0} to {1}", wirelessSocket, state));
            foreach (WirelessSocketDto socket in _wirelessSocketList)
            {
                if (socket.Name == wirelessSocket.Name)
                {
                    socket.IsActivated = state;
                    setWirelessSocketListAsync(socket.Name, socket.IsActivated);
                    break;
                }
            }
        }

        public void SetWirelessSocket(string wirelessSocketName, bool state)
        {
            _logger.Debug(string.Format("Set socket {0} to {1}", wirelessSocketName, state));
            foreach (WirelessSocketDto socket in _wirelessSocketList)
            {
                if (socket.Name == wirelessSocketName)
                {
                    socket.IsActivated = state;
                    setWirelessSocketListAsync(socket.Name, socket.IsActivated);
                    break;
                }
            }
        }

        public void ChangeWirelessSocketState(WirelessSocketDto wirelessSocket)
        {
            _logger.Debug(string.Format("Change state for socket {0}", wirelessSocket));
            foreach (WirelessSocketDto socket in _wirelessSocketList)
            {
                if (socket.Name == wirelessSocket.Name)
                {
                    socket.IsActivated = !socket.IsActivated;
                    setWirelessSocketListAsync(socket.Name, socket.IsActivated);
                    break;
                }
            }
        }

        public void ChangeWirelessSocketState(string wirelessSocketName)
        {
            _logger.Debug(string.Format("Change state for socket {0}", wirelessSocketName));
            foreach (WirelessSocketDto socket in _wirelessSocketList)
            {
                if (socket.Name == wirelessSocketName)
                {
                    socket.IsActivated = !socket.IsActivated;
                    setWirelessSocketListAsync(socket.Name, socket.IsActivated);
                    break;
                }
            }
        }

        private async Task loadWirelessSocketListAsync()
        {
            _logger.Debug("loadWirelessSocketList");

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnWirelessSocketDownloadFinished(null, false);
                return;
            }

            string requestUrl = "http://" + _appSettingsService.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SOCKETS.Action;

            _downloadController.OnDownloadFinished += _wirelessSocketDownloadFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.WirelessSocket);
        }

        private async Task setWirelessSocketListAsync(string socketName, bool state)
        {
            _logger.Debug(string.Format("setWirelessSocketListAsync: socketName {0} state {1}", socketName, state));

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnWirelessSocketDownloadFinished(null, false);
                return;
            }

            string requestUrl = "http://" + _appSettingsService.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.SET_SOCKET.Action + socketName + (state ? Constants.STATE_ON : Constants.STATE_OFF);

            _downloadController.OnDownloadFinished += _setWirelessSocketFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.WirelessSocket);
        }

        private void _wirelessSocketDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_wirelessSocketDownloadFinished");

            if (downloadType != DownloadType.WirelessSocket)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _wirelessSocketDownloadFinished;

            if (response.Contains("Error"))
            {
                _logger.Error(response);

                OnWirelessSocketDownloadFinished(null, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnWirelessSocketDownloadFinished(null, false);
                return;
            }

            IList<WirelessSocketDto> wirelessSocketList = _jsonDataToWirelessSocketConverter.GetList(response);
            if (wirelessSocketList == null)
            {
                _logger.Error("Converted wirelessSocketList is null!");

                OnWirelessSocketDownloadFinished(null, false);
                return;
            }

            _wirelessSocketList = wirelessSocketList;

            OnWirelessSocketDownloadFinished(_wirelessSocketList, true);
        }

        private void _setWirelessSocketFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_setWirelessSocketFinished");

            if (downloadType != DownloadType.WirelessSocket)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _setWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("0"))
            {
                _logger.Error(response);

                OnSetWirelessSocketFinished(null, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Setting was not successful!");

                OnSetWirelessSocketFinished(null, false);
                return;
            }

            OnSetWirelessSocketFinished(null, true);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _setWirelessSocketFinished;

            _downloadController.Dispose();
        }
    }
}
