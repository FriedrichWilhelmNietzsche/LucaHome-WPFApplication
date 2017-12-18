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
    public delegate void WirelessSwitchDownloadEventHandler(IList<WirelessSwitchDto> wirelessSwitchList, bool success, string response);
    public delegate void WirelessSwitchToggleEventHandler(bool success, string response);
    public delegate void WirelessSwitchAddEventHandler(bool success, string response);
    public delegate void WirelessSwitchUpdateEventHandler(bool success, string response);
    public delegate void WirelessSwitchDeleteEventHandler(bool success, string response);

    public class WirelessSwitchService
    {
        private const string TAG = "WirelessSwitchService";
        private const int TIMEOUT = 5 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static WirelessSwitchService _instance = null;
        private static readonly object _padlock = new object();

        private IList<WirelessSwitchDto> _wirelessSwitchList = new List<WirelessSwitchDto>();

        private Timer _downloadTimer;

        WirelessSwitchService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _wirelessSwitchDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event WirelessSwitchDownloadEventHandler OnWirelessSwitchDownloadFinished;
        private void publishOnWirelessSwitchDownloadFinished(IList<WirelessSwitchDto> wirelessSwitchList, bool success, string response)
        {
            OnWirelessSwitchDownloadFinished?.Invoke(wirelessSwitchList, success, response);
        }

        public event WirelessSwitchToggleEventHandler OnWirelessSwitchToggleFinished;
        private void publishOnWirelessSwitchToggleFinished(bool success, string response)
        {
            OnWirelessSwitchToggleFinished?.Invoke(success, response);
        }

        public event WirelessSwitchAddEventHandler OnWirelessSwitchAddFinished;
        private void publishOnWirelessSwitchAddFinished(bool success, string response)
        {
            OnWirelessSwitchAddFinished?.Invoke(success, response);
        }

        public event WirelessSwitchUpdateEventHandler OnWirelessSwitchUpdateFinished;
        private void publishOnWirelessSwitchUpdateFinished(bool success, string response)
        {
            OnWirelessSwitchUpdateFinished?.Invoke(success, response);
        }

        public event WirelessSwitchDeleteEventHandler OnWirelessSwitchDeleteFinished;
        private void publishOnWirelessSwitchDeleteFinished(bool success, string response)
        {
            OnWirelessSwitchDeleteFinished?.Invoke(success, response);
        }

        public static WirelessSwitchService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new WirelessSwitchService();
                    }

                    return _instance;
                }
            }
        }

        public IList<WirelessSwitchDto> WirelessSwitchList
        {
            get
            {
                return _wirelessSwitchList;
            }
        }

        public WirelessSwitchDto GetById(int id)
        {
            WirelessSwitchDto foundWirelessSwitch = _wirelessSwitchList
                        .Where(wirelessSwitch => wirelessSwitch.Id == id)
                        .Select(wirelessSwitch => wirelessSwitch)
                        .FirstOrDefault();

            return foundWirelessSwitch;
        }

        public WirelessSwitchDto GetByName(string name)
        {
            WirelessSwitchDto foundWirelessSwitch = _wirelessSwitchList
                        .Where(wirelessSwitch => wirelessSwitch.Name.Equals(name))
                        .Select(wirelessSwitch => wirelessSwitch)
                        .FirstOrDefault();

            return foundWirelessSwitch;
        }

        public IList<WirelessSwitchDto> FoundWirelessSwitches(string searchKey)
        {
            if (searchKey == string.Empty)
            {
                return _wirelessSwitchList;
            }

            List<WirelessSwitchDto> foundWirelessSwitches = _wirelessSwitchList
                        .Where(wirelessSwitch =>
                            wirelessSwitch.Name.Contains(searchKey)
                            || wirelessSwitch.Area.Contains(searchKey)
                            || wirelessSwitch.Code.Contains(searchKey)
                            || wirelessSwitch.ShortName.Contains(searchKey)
                            || wirelessSwitch.IsActivated.ToString().Contains(searchKey)
                            || wirelessSwitch.ActivationString.Contains(searchKey)
                            || wirelessSwitch.RemoteId.ToString().Contains(searchKey)
                            || wirelessSwitch.KeyCode.ToString().Contains(searchKey)
                            || wirelessSwitch.Action.ToString().Contains(searchKey))
                        .Select(wirelessSwitch => wirelessSwitch)
                        .ToList();

            return foundWirelessSwitches;
        }

        public void LoadWirelessSwitchList()
        {
            loadWirelessSwitchListAsync();
        }

        public void ToggleWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            Logger.Instance.Debug(TAG, string.Format("Toggle switch {0}", wirelessSwitch));
            toggleWirelessSwitchAsync(wirelessSwitch.Name);
        }

        public void ToggleWirelessSwitch(string wirelessSwitchName)
        {
            Logger.Instance.Debug(TAG, string.Format("Toggle switch {0}", wirelessSwitchName));
            toggleWirelessSwitchAsync(wirelessSwitchName);
        }

        public void AddWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            Logger.Instance.Debug(TAG, string.Format("AddWirelessSwitch: add switch {0}", wirelessSwitch));
            addWirelessSwitchAsync(wirelessSwitch);
        }

        public void UpdateWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateWirelessSwitch: update switch {0}", wirelessSwitch));
            updateWirelessSwitchAsync(wirelessSwitch);
        }

        public void DeleteWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteWirelessSwitch: delete switch {0}", wirelessSwitch));
            deleteWirelessSwitchAsync(wirelessSwitch);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadWirelessSwitchListAsync();
        }

        private async Task loadWirelessSwitchListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnWirelessSwitchDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_SWITCHES.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitch);
        }

        private async Task toggleWirelessSwitchAsync(string switchName)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnWirelessSwitchToggleFinished(false, "No user");
                return;
            }

            string action = LucaServerAction.TOGGLE_SWITCH.Action + switchName;
            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                action);

            _downloadController.OnDownloadFinished += _toggleWirelessSwitchFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchToggle);
        }

        private async Task addWirelessSwitchAsync(WirelessSwitchDto wirelessSwitch)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnWirelessSwitchAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                wirelessSwitch.CommandAdd);

            _downloadController.OnDownloadFinished += _addWirelessSwitchFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchAdd);
        }

        private async Task updateWirelessSwitchAsync(WirelessSwitchDto wirelessSwitch)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnWirelessSwitchUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                wirelessSwitch.CommandUpdate);

            _downloadController.OnDownloadFinished += _updateWirelessSwitchFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchUpdate);
        }

        private async Task deleteWirelessSwitchAsync(WirelessSwitchDto wirelessSwitch)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnWirelessSwitchDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                wirelessSwitch.CommandDelete);

            _downloadController.OnDownloadFinished += _deleteWirelessSwitchFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchDelete);
        }

        private void _wirelessSwitchDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSwitch)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnWirelessSwitchDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnWirelessSwitchDownloadFinished(null, false, response);
                return;
            }

            IList<WirelessSwitchDto> wirelessSwitchList = JsonDataToWirelessSwitchConverter.Instance.GetList(response);
            if (wirelessSwitchList == null)
            {
                Logger.Instance.Error(TAG, "Converted wirelessSwitchList is null!");
                publishOnWirelessSwitchDownloadFinished(null, false, response);
                return;
            }

            _wirelessSwitchList = wirelessSwitchList;
            publishOnWirelessSwitchDownloadFinished(_wirelessSwitchList, true, response);
        }

        private void _toggleWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSwitchToggle)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _toggleWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnWirelessSwitchToggleFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Setting was not successful!");
                publishOnWirelessSwitchToggleFinished(false, response);
                return;
            }

            publishOnWirelessSwitchToggleFinished(true, response);
            loadWirelessSwitchListAsync();
        }

        private void _addWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSwitchAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _addWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnWirelessSwitchAddFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnWirelessSwitchAddFinished(false, response);
                return;
            }

            publishOnWirelessSwitchAddFinished(true, response);
            loadWirelessSwitchListAsync();
        }

        private void _updateWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSwitchUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _updateWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnWirelessSwitchUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Updating was not successful!");
                publishOnWirelessSwitchUpdateFinished(false, response);
                return;
            }

            publishOnWirelessSwitchUpdateFinished(true, response);
            loadWirelessSwitchListAsync();
        }

        private void _deleteWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.WirelessSwitchDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _deleteWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnWirelessSwitchDeleteFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Deleting was not successful!");
                publishOnWirelessSwitchDeleteFinished(false, response);
                return;
            }

            publishOnWirelessSwitchDeleteFinished(true, response);
            loadWirelessSwitchListAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _wirelessSwitchDownloadFinished;
            _downloadController.OnDownloadFinished -= _toggleWirelessSwitchFinished;
            _downloadController.OnDownloadFinished -= _addWirelessSwitchFinished;
            _downloadController.OnDownloadFinished -= _updateWirelessSwitchFinished;
            _downloadController.OnDownloadFinished -= _deleteWirelessSwitchFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
