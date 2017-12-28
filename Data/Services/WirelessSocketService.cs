using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Data.Services
{
    public delegate void WirelessSocketDownloadEventHandler(IList<WirelessSocketDto> wirelessSocketList, bool success, string response);
    public delegate void WirelessSocketSetEventHandler(bool success, string response);
    public delegate void WirelessSocketAddEventHandler(bool success, string response);
    public delegate void WirelessSocketUpdateEventHandler(bool success, string response);
    public delegate void WirelessSocketDeleteEventHandler(bool success, string response);

    public class WirelessSocketService
    {
        private const string TAG = "WirelessSocketService";
        private const int TIMEOUT = 5 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static WirelessSocketService _instance = null;
        private static readonly object _padlock = new object();

        private IList<WirelessSocketDto> _wirelessSocketList = new List<WirelessSocketDto>();

        private Timer _downloadTimer;

        WirelessSocketService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _wirelessSocketDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event WirelessSocketDownloadEventHandler OnWirelessSocketDownloadFinished;
        private void publishOnWirelessSocketDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            OnWirelessSocketDownloadFinished?.Invoke(wirelessSocketList, success, response);
        }

        public event WirelessSocketSetEventHandler OnSetWirelessSocketFinished;
        private void publishOnSetWirelessSocketFinished(bool success, string response)
        {
            OnSetWirelessSocketFinished?.Invoke(success, response);
        }

        public event WirelessSocketAddEventHandler OnAddWirelessSocketFinished;
        private void publishOnAddWirelessSocketFinished(bool success, string response)
        {
            OnAddWirelessSocketFinished?.Invoke(success, response);
        }

        public event WirelessSocketUpdateEventHandler OnUpdateWirelessSocketFinished;
        private void publishOnUpdateWirelessSocketFinished(bool success, string response)
        {
            OnUpdateWirelessSocketFinished?.Invoke(success, response);
        }

        public event WirelessSocketDeleteEventHandler OnDeleteWirelessSocketFinished;
        private void publishOnDeleteWirelessSocketFinished(bool success, string response)
        {
            OnDeleteWirelessSocketFinished?.Invoke(success, response);
        }

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
                return _wirelessSocketList.OrderBy(wirelessSocket => wirelessSocket.TypeId).ToList();
            }
        }

        public WirelessSocketDto GetByTypeId(int typeId)
        {
            WirelessSocketDto foundWirelessSocket = _wirelessSocketList
                        .Where(wirelessSocket => wirelessSocket.TypeId == typeId)
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
            if (searchKey == string.Empty)
            {
                return _wirelessSocketList;
            }

            List<WirelessSocketDto> foundWirelessSocketList = _wirelessSocketList
                        .Where(wirelessSocket => wirelessSocket.ToString().Contains(searchKey))
                        .Select(wirelessSocket => wirelessSocket)
                        .OrderBy(wirelessSocket => wirelessSocket.Area)
                        .ToList();
            return foundWirelessSocketList;
        }

        public void LoadWirelessSocketList()
        {
            loadWirelessSocketListAsync();
        }

        public void SetWirelessSocket(WirelessSocketDto wirelessSocket, bool state)
        {
            Logger.Instance.Debug(TAG, string.Format("Set socket {0} to {1}", wirelessSocket, state));
            setWirelessSocketAsync(wirelessSocket.Name, state);
        }

        public void SetWirelessSocket(string wirelessSocketName, bool state)
        {
            Logger.Instance.Debug(TAG, string.Format("Set socket {0} to {1}", wirelessSocketName, state));
            setWirelessSocketAsync(wirelessSocketName, state);
        }

        public void ChangeWirelessSocketState(WirelessSocketDto wirelessSocket)
        {
            Logger.Instance.Debug(TAG, string.Format("Change state for socket {0}", wirelessSocket));
            setWirelessSocketAsync(wirelessSocket.Name, !wirelessSocket.IsActivated);
        }

        public void ChangeWirelessSocketState(string wirelessSocketName)
        {
            Logger.Instance.Debug(TAG, string.Format("Change state for socket {0}", wirelessSocketName));
            WirelessSocketDto wirelessSocket = GetByName(wirelessSocketName);
            setWirelessSocketAsync(wirelessSocket.Name, !wirelessSocket.IsActivated);
        }

        public void AddWirelessSocket(WirelessSocketDto newWirelessSocket)
        {
            Logger.Instance.Debug(TAG, string.Format("AddWirelessSocket: add socket {0}", newWirelessSocket));
            addWirelessSocketAsync(newWirelessSocket);
        }

        public void UpdateWirelessSocket(WirelessSocketDto updateWirelessSocket)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateWirelessSocket: updating socket {0}", updateWirelessSocket));
            updateWirelessSocketAsync(updateWirelessSocket);
        }

        public void DeleteWirelessSocket(WirelessSocketDto deleteWirelessSocket)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteWirelessSocket: deleting socket {0}", deleteWirelessSocket));
            deleteWirelessSocketAsync(deleteWirelessSocket);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadWirelessSocketListAsync();
        }

        private async Task loadWirelessSocketListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnWirelessSocketDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_SOCKETS.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocket);
        }

        private async Task setWirelessSocketAsync(string socketName, bool state)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnSetWirelessSocketFinished(false, "No user");
                return;
            }

            string action = LucaServerAction.SET_SOCKET.Action + socketName + (state ? Constants.STATE_ON : Constants.STATE_OFF);
            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                action);

            _downloadController.OnDownloadFinished += _setWirelessSocketFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketSet);
        }

        private async Task addWirelessSocketAsync(WirelessSocketDto newWirelessSocket)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnAddWirelessSocketFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newWirelessSocket.CommandAdd);

            _downloadController.OnDownloadFinished += _addWirelessSocketFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketAdd);
        }

        private async Task updateWirelessSocketAsync(WirelessSocketDto updateWirelessSocket)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnUpdateWirelessSocketFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateWirelessSocket.CommandUpdate);

            _downloadController.OnDownloadFinished += _updateWirelessSocketFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketUpdate);
        }

        private async Task deleteWirelessSocketAsync(WirelessSocketDto deleteWirelessSocket)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnDeleteWirelessSocketFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteWirelessSocket.CommandDelete);

            _downloadController.OnDownloadFinished += _deleteWirelessSocketFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSocketDelete);
        }

        private void _wirelessSocketDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSocket)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnWirelessSocketDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnWirelessSocketDownloadFinished(null, false, response);
                return;
            }

            IList<WirelessSocketDto> wirelessSocketList = JsonDataToWirelessSocketConverter.Instance.GetList(response);
            if (wirelessSocketList == null)
            {
                Logger.Instance.Error(TAG, "Converted wirelessSocketList is null!");
                publishOnWirelessSocketDownloadFinished(null, false, response);
                return;
            }

            _wirelessSocketList = wirelessSocketList.OrderBy(wirelessSocket => wirelessSocket.TypeId).ToList();
            publishOnWirelessSocketDownloadFinished(_wirelessSocketList, true, response);
        }

        private void _setWirelessSocketFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSocketSet)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _setWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnSetWirelessSocketFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Setting was not successful!");
                publishOnSetWirelessSocketFinished(false, response);
                return;
            }

            publishOnSetWirelessSocketFinished(true, response);
            loadWirelessSocketListAsync();
        }

        private void _addWirelessSocketFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSocketAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _addWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnAddWirelessSocketFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnAddWirelessSocketFinished(false, response);
                return;
            }

            publishOnAddWirelessSocketFinished(true, response);
            loadWirelessSocketListAsync();
        }

        private void _updateWirelessSocketFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSocketUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _updateWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnUpdateWirelessSocketFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Updating was not successful!");
                publishOnUpdateWirelessSocketFinished(false, response);
                return;
            }

            publishOnUpdateWirelessSocketFinished(true, response);
            loadWirelessSocketListAsync();
        }

        private void _deleteWirelessSocketFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSocketDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _deleteWirelessSocketFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnDeleteWirelessSocketFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Deleting was not successful!");
                publishOnDeleteWirelessSocketFinished(false, response);
                return;
            }

            publishOnDeleteWirelessSocketFinished(true, response);
            loadWirelessSocketListAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _wirelessSocketDownloadFinished;
            _downloadController.OnDownloadFinished -= _setWirelessSocketFinished;
            _downloadController.OnDownloadFinished -= _addWirelessSocketFinished;
            _downloadController.OnDownloadFinished -= _updateWirelessSocketFinished;
            _downloadController.OnDownloadFinished -= _deleteWirelessSocketFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
