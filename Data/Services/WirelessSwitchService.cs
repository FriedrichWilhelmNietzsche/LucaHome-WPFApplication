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
    public delegate void WirelessSwitchDownloadEventHandler(IList<WirelessSwitchDto> wirelessSwitchList, bool success, string response);
    public delegate void WirelessSwitchToggleEventHandler(bool success, string response);
    public delegate void WirelessSwitchAddEventHandler(bool success, string response);
    public delegate void WirelessSwitchUpdateEventHandler(bool success, string response);
    public delegate void WirelessSwitchDeleteEventHandler(bool success, string response);

    public class WirelessSwitchService
    {
        private const string TAG = "WirelessSwitchService";
        private readonly Logger _logger;

        private const int TIMEOUT = 5 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly IJsonDataConverter<WirelessSwitchDto> _jsonDataToWirelessSwitchConverter;

        private static WirelessSwitchService _instance = null;
        private static readonly object _padlock = new object();

        private IList<WirelessSwitchDto> _wirelessSwitchList = new List<WirelessSwitchDto>();

        private Timer _downloadTimer;

        WirelessSwitchService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToWirelessSwitchConverter = new JsonDataToWirelessSwitchConverter();

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
            _logger.Debug("LoadWirelessSwitchList");
            loadWirelessSwitchListAsync();
        }

        public void ToggleWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            _logger.Debug(string.Format("Toggle switch {0}", wirelessSwitch));
            toggleWirelessSwitchAsync(wirelessSwitch.Name);
        }

        public void ToggleWirelessSwitch(string wirelessSwitchName)
        {
            _logger.Debug(string.Format("Toggle switch {0}", wirelessSwitchName));
            toggleWirelessSwitchAsync(wirelessSwitchName);
        }

        public void AddWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            _logger.Debug(string.Format("AddWirelessSwitch: add switch {0}", wirelessSwitch));
            addWirelessSwitchAsync(wirelessSwitch);
        }

        public void UpdateWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            _logger.Debug(string.Format("UpdateWirelessSwitch: update switch {0}", wirelessSwitch));
            updateWirelessSwitchAsync(wirelessSwitch);
        }

        public void DeleteWirelessSwitch(WirelessSwitchDto wirelessSwitch)
        {
            _logger.Debug(string.Format("DeleteWirelessSwitch: delete switch {0}", wirelessSwitch));
            deleteWirelessSwitchAsync(wirelessSwitch);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadWirelessSwitchListAsync();
        }

        private async Task loadWirelessSwitchListAsync()
        {
            _logger.Debug("loadWirelessSwitchListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnWirelessSwitchDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SWITCHES.Action;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitch);
        }

        private async Task toggleWirelessSwitchAsync(string switchName)
        {
            _logger.Debug(string.Format("toggleWirelessSwitchAsync: switchName {0}", switchName));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnWirelessSwitchToggleFinished(false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.TOGGLE_SWITCH.Action + switchName;

            _downloadController.OnDownloadFinished += _toggleWirelessSwitchFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchToggle);
        }

        private async Task addWirelessSwitchAsync(WirelessSwitchDto wirelessSwitch)
        {
            _logger.Debug(string.Format("addWirelessSwitchAsync: add new WirelessSwitchDto {0}", wirelessSwitch));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnWirelessSwitchAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                wirelessSwitch.CommandAdd);

            _downloadController.OnDownloadFinished += _addWirelessSwitchFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchAdd);
        }

        private async Task updateWirelessSwitchAsync(WirelessSwitchDto wirelessSwitch)
        {
            _logger.Debug(string.Format("updateWirelessSwitchAsync: updating WirelessSwitchDto {0}", wirelessSwitch));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnWirelessSwitchUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                wirelessSwitch.CommandUpdate);

            _downloadController.OnDownloadFinished += _updateWirelessSwitchFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchUpdate);
        }

        private async Task deleteWirelessSwitchAsync(WirelessSwitchDto wirelessSwitch)
        {
            _logger.Debug(string.Format("deleteWirelessSwitchAsync: delete WirelessSwitchDto {0}", wirelessSwitch));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnWirelessSwitchDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                wirelessSwitch.CommandDelete);

            _downloadController.OnDownloadFinished += _deleteWirelessSwitchFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.WirelessSwitchDelete);
        }

        private void _wirelessSwitchDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_wirelessSwitchDownloadFinished");

            if (downloadType != DownloadType.WirelessSwitch)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnWirelessSwitchDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnWirelessSwitchDownloadFinished(null, false, response);
                return;
            }

            IList<WirelessSwitchDto> wirelessSwitchList = _jsonDataToWirelessSwitchConverter.GetList(response);
            if (wirelessSwitchList == null)
            {
                _logger.Error("Converted wirelessSwitchList is null!");

                publishOnWirelessSwitchDownloadFinished(null, false, response);
                return;
            }

            _wirelessSwitchList = wirelessSwitchList;

            publishOnWirelessSwitchDownloadFinished(_wirelessSwitchList, true, response);
        }

        private void _toggleWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_toggleWirelessSwitchFinished");

            if (downloadType != DownloadType.WirelessSwitchToggle)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _toggleWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                publishOnWirelessSwitchToggleFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Setting was not successful!");

                publishOnWirelessSwitchToggleFinished(false, response);
                return;
            }

            publishOnWirelessSwitchToggleFinished(true, response);

            loadWirelessSwitchListAsync();
        }

        private void _addWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_addWirelessSwitchFinished");

            if (downloadType != DownloadType.WirelessSwitchAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _addWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                publishOnWirelessSwitchAddFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                publishOnWirelessSwitchAddFinished(false, response);
                return;
            }

            publishOnWirelessSwitchAddFinished(true, response);

            loadWirelessSwitchListAsync();
        }

        private void _updateWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_updateWirelessSwitchFinished");

            if (downloadType != DownloadType.WirelessSwitchUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _updateWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                publishOnWirelessSwitchUpdateFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Updating was not successful!");

                publishOnWirelessSwitchUpdateFinished(false, response);
                return;
            }

            publishOnWirelessSwitchUpdateFinished(true, response);

            loadWirelessSwitchListAsync();
        }

        private void _deleteWirelessSwitchFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_deleteWirelessSwitchFinished");

            if (downloadType != DownloadType.WirelessSwitchDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _deleteWirelessSwitchFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                publishOnWirelessSwitchDeleteFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Deleting was not successful!");

                publishOnWirelessSwitchDeleteFinished(false, response);
                return;
            }

            publishOnWirelessSwitchDeleteFinished(true, response);

            loadWirelessSwitchListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _wirelessSwitchDownloadFinished;
            _downloadController.OnDownloadFinished -= _toggleWirelessSwitchFinished;
            _downloadController.OnDownloadFinished -= _addWirelessSwitchFinished;
            _downloadController.OnDownloadFinished -= _updateWirelessSwitchFinished;
            _downloadController.OnDownloadFinished -= _deleteWirelessSwitchFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
