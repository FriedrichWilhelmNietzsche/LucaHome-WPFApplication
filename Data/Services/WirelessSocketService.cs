using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Interfaces;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Data.Services
{
    public delegate void WirelessSocketDownloadEventHandler(IList<WirelessSocketDto> wirelessSocketList, bool success, string response);
    public delegate void WirelessSocketAddEventHandler(bool success, string response);
    public delegate void WirelessSocketUpdateEventHandler(bool success, string response);
    public delegate void WirelessSocketDeleteEventHandler(bool success, string response);

    public class WirelessSocketService
    {
        private const string TAG = "WirelessSocketService";
        private readonly Logger _logger;

        private const int TIMEOUT = 5 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly IJsonDataConverter<WirelessSocketDto> _jsonDataToWirelessSocketConverter;

        private static WirelessSocketService _instance = null;
        private static readonly object _padlock = new object();

        private IList<WirelessSocketDto> _wirelessSocketList = new List<WirelessSocketDto>();

        private Timer _downloadTimer;

        WirelessSocketService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToWirelessSocketConverter = new JsonDataToWirelessSocketConverter();

            _downloadController.OnDownloadFinished += _wirelessSocketDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event WirelessSocketDownloadEventHandler OnWirelessSocketDownloadFinished;
        public event WirelessSocketDownloadEventHandler OnSetWirelessSocketFinished;
        public event WirelessSocketAddEventHandler OnAddWirelessSocketFinished;
        public event WirelessSocketUpdateEventHandler OnUpdateWirelessSocketFinished;
        public event WirelessSocketDeleteEventHandler OnDeleteWirelessSocketFinished;

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

        public WirelessSocketDto GetById(int id)
        {
            WirelessSocketDto foundWirelessSocket = _wirelessSocketList
                        .Where(wirelessSocket => wirelessSocket.Id == id)
                        .Select(wirelessSocket => wirelessSocket)
                        .FirstOrDefault();

            return foundWirelessSocket;
        }

        public WirelessSocketDto GetByName(string name)
        {
            WirelessSocketDto foundWirelessSocket = _wirelessSocketList
                        .Where(wirelessSocket => wirelessSocket.Name.Equals(name))
                        .Select(wirelessSocket => wirelessSocket)
                        .FirstOrDefault();

            return foundWirelessSocket;
        }

        public IList<WirelessSocketDto> FoundWirelessSockets(string searchKey)
        {
            List<WirelessSocketDto> foundWirelessSockets = _wirelessSocketList
                        .Where(wirelessSocket =>
                            wirelessSocket.Name.Contains(searchKey)
                            || wirelessSocket.Area.Contains(searchKey)
                            || wirelessSocket.Code.Contains(searchKey)
                            || wirelessSocket.ShortName.Contains(searchKey)
                            || wirelessSocket.IsActivated.ToString().Contains(searchKey)
                            || wirelessSocket.ActivationString.Contains(searchKey))
                        .Select(wirelessSocket => wirelessSocket)
                        .ToList();

            return foundWirelessSockets;
        }

        public void LoadWirelessSocketList()
        {
            _logger.Debug("LoadWirelessSocketList");
            loadWirelessSocketListAsync();
        }

        public void SetWirelessSocket(WirelessSocketDto wirelessSocket, bool state)
        {
            _logger.Debug(string.Format("Set socket {0} to {1}", wirelessSocket, state));
            setWirelessSocketAsync(wirelessSocket.Name, state);
        }

        public void SetWirelessSocket(string wirelessSocketName, bool state)
        {
            _logger.Debug(string.Format("Set socket {0} to {1}", wirelessSocketName, state));
            WirelessSocketDto wirelessSocket = GetByName(wirelessSocketName);
            setWirelessSocketAsync(wirelessSocket.Name, state);
        }

        public void ChangeWirelessSocketState(WirelessSocketDto wirelessSocket)
        {
            _logger.Debug(string.Format("Change state for socket {0}", wirelessSocket));
            setWirelessSocketAsync(wirelessSocket.Name, !wirelessSocket.IsActivated);
        }

        public void ChangeWirelessSocketState(string wirelessSocketName)
        {
            _logger.Debug(string.Format("Change state for socket {0}", wirelessSocketName));
            WirelessSocketDto wirelessSocket = GetByName(wirelessSocketName);
            setWirelessSocketAsync(wirelessSocket.Name, !wirelessSocket.IsActivated);
        }

        public void AddWirelessSocket(WirelessSocketDto newWirelessSocket)
        {
            _logger.Debug(string.Format("AddWirelessSocket: add socket {0}", newWirelessSocket));
            addWirelessSocketAsync(newWirelessSocket);
        }

        public void UpdateWirelessSocket(WirelessSocketDto updateWirelessSocket)
        {
            _logger.Debug(string.Format("UpdateWirelessSocket: updating socket {0}", updateWirelessSocket));
            updateWirelessSocketAsync(updateWirelessSocket);
        }

        public void DeleteWirelessSocket(WirelessSocketDto deleteWirelessSocket)
        {
            _logger.Debug(string.Format("DeleteWirelessSocket: deleting socket {0}", deleteWirelessSocket));
            deleteWirelessSocketAsync(deleteWirelessSocket);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadWirelessSocketListAsync();
        }

        private async Task loadWirelessSocketListAsync()
        {
            _logger.Debug("loadWirelessSocketList");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnWirelessSocketDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SOCKETS.Action;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocket);
        }

        private async Task setWirelessSocketAsync(string socketName, bool state)
        {
            _logger.Debug(string.Format("setWirelessSocketListAsync: socketName {0} state {1}", socketName, state));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnWirelessSocketDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.SET_SOCKET.Action + socketName + (state ? Constants.STATE_ON : Constants.STATE_OFF);

            _downloadController.OnDownloadFinished += _setWirelessSocketFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketSet);
        }

        private async Task addWirelessSocketAsync(WirelessSocketDto newWirelessSocket)
        {
            _logger.Debug(string.Format("addWirelessSocketAsync: add new wirelessSocket {0}", newWirelessSocket));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnAddWirelessSocketFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newWirelessSocket.CommandAdd);

            _downloadController.OnDownloadFinished += _addWirelessSocketFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketAdd);
        }

        private async Task updateWirelessSocketAsync(WirelessSocketDto updateWirelessSocket)
        {
            _logger.Debug(string.Format("updateWirelessSocketAsync: updating wirelessSocket {0}", updateWirelessSocket));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnUpdateWirelessSocketFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateWirelessSocket.CommandUpdate);

            _downloadController.OnDownloadFinished += _updateWirelessSocketFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketUpdate);
        }

        private async Task deleteWirelessSocketAsync(WirelessSocketDto deleteWirelessSocket)
        {
            _logger.Debug(string.Format("deleteWirelessSocketAsync: delete wirelessSocket {0}", deleteWirelessSocket));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnDeleteWirelessSocketFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteWirelessSocket.CommandDelete);

            _downloadController.OnDownloadFinished += _deleteWirelessSocketFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketDelete);
        }

        private void _wirelessSocketDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_wirelessSocketDownloadFinished");

            if (downloadType != DownloadType.WirelessSocket)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnWirelessSocketDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnWirelessSocketDownloadFinished(null, false, response);
                return;
            }

            IList<WirelessSocketDto> wirelessSocketList = _jsonDataToWirelessSocketConverter.GetList(response);
            if (wirelessSocketList == null)
            {
                _logger.Error("Converted wirelessSocketList is null!");

                OnWirelessSocketDownloadFinished(null, false, response);
                return;
            }

            _wirelessSocketList = wirelessSocketList;

            OnWirelessSocketDownloadFinished(_wirelessSocketList, true, response);
        }

        private void _setWirelessSocketFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_setWirelessSocketFinished");

            if (downloadType != DownloadType.WirelessSocketSet)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _setWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnSetWirelessSocketFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Setting was not successful!");

                OnSetWirelessSocketFinished(null, false, response);
                return;
            }

            OnSetWirelessSocketFinished(null, true, response);

            loadWirelessSocketListAsync();
        }

        private void _addWirelessSocketFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_addWirelessSocketFinished");

            if (downloadType != DownloadType.WirelessSocketAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _addWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnAddWirelessSocketFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                OnAddWirelessSocketFinished(false, response);
                return;
            }

            OnAddWirelessSocketFinished(true, response);

            loadWirelessSocketListAsync();
        }

        private void _updateWirelessSocketFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_updateWirelessSocketFinished");

            if (downloadType != DownloadType.WirelessSocketUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _updateWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnUpdateWirelessSocketFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Updating was not successful!");

                OnUpdateWirelessSocketFinished(false, response);
                return;
            }

            OnUpdateWirelessSocketFinished(true, response);

            loadWirelessSocketListAsync();
        }

        private void _deleteWirelessSocketFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_deleteWirelessSocketFinished");

            if (downloadType != DownloadType.WirelessSocketDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _deleteWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnDeleteWirelessSocketFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Deleting was not successful!");

                OnDeleteWirelessSocketFinished(false, response);
                return;
            }

            OnDeleteWirelessSocketFinished(true, response);

            loadWirelessSocketListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _wirelessSocketDownloadFinished;
            _downloadController.OnDownloadFinished -= _setWirelessSocketFinished;
            _downloadController.OnDownloadFinished -= _addWirelessSocketFinished;
            _downloadController.OnDownloadFinished -= _updateWirelessSocketFinished;
            _downloadController.OnDownloadFinished -= _deleteWirelessSocketFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
