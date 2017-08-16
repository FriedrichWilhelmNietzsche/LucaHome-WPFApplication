using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Common.Interfaces;
using static Common.Dto.SecurityDto;
using System.IO;
using System.Diagnostics;

namespace Data.Services
{
    public delegate void SecurityDownloadEventHandler(SecurityDto security, bool success, string response);

    public class SecurityService
    {
        private const string TAG = "SecurityService";
        private readonly Logger _logger;

        private const int TIMEOUT = 15 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly IJsonDataConverter<SecurityDto> _jsonDataToSecurityConverter;
        private readonly LocalDriveController _localDriveController;

        private static SecurityService _instance = null;
        private static readonly object _padlock = new object();

        private SecurityDto _security;
        private DriveInfo _raspberryDrive;

        private Timer _downloadTimer;

        SecurityService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToSecurityConverter = new JsonDataToSecurityConverter();
            _localDriveController = new LocalDriveController();

            _raspberryDrive = _localDriveController.GetRaspberryDrive();

            _downloadController.OnDownloadFinished += _securityDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event SecurityDownloadEventHandler OnSecurityDownloadFinished;
        private void publishOnSecurityDownloadFinished(SecurityDto security, bool success, string response)
        {
            OnSecurityDownloadFinished?.Invoke(security, success, response);
        }

        public static SecurityService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new SecurityService();
                    }

                    return _instance;
                }
            }
        }

        public SecurityDto Security
        {
            get
            {
                return _security;
            }
        }

        public IList<RegisteredEventDto> FoundRegisteredEvents(string searchKey)
        {
            List<RegisteredEventDto> foundRegisteredEvents = _security.RegisteredMotionEvents
                        .Where(entry =>
                        entry.Id.ToString().Contains(searchKey)
                        || entry.Name.Contains(searchKey))
                        .Select(entry => entry)
                        .ToList();

            return foundRegisteredEvents;
        }

        public void OpenFile(string registeredEventName)
        {
            _logger.Debug("OpenFile");
            if (_raspberryDrive == null)
            {
                _logger.Error("_raspberryDrive is null!");
                return;
            }

            string registeredEventPathString = string.Format("{0}{1}{2}", _raspberryDrive.Name, "Camera\\", registeredEventName);
            _logger.Debug(string.Format("Opening {0} with associated programm", registeredEventPathString));
            Process.Start(@registeredEventPathString);
        }

        public void LoadSecurity()
        {
            _logger.Debug("LoadSecurity");
            loadSecurityAsync();
        }

        public void ToggleCameraState()
        {
            _logger.Debug("SetCameraState");
            setCameraStateAsync(!_security.IsCameraActive);
        }

        public void ToggleCameraControlState()
        {
            _logger.Debug("SetCameraControlState");
            setCameraControlStateAsync(!_security.IsCameraControlActive);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadSecurityAsync();
        }

        private async Task loadSecurityAsync()
        {
            _logger.Debug("loadSecurityAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnSecurityDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_MOTION_DATA.Action;
            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            // Ugly hack to let the server change its' state
            await Task.Delay(1500);
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Security);
        }

        private async Task setCameraStateAsync(bool state)
        {
            _logger.Debug("setCameraStateAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnSecurityDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl;
            if (state)
            {
                requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.START_MOTION.Action;
            }
            else
            {
                requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.STOP_MOTION.Action;
            }

            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            _downloadController.OnDownloadFinished += _setCameraStateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.SecurityCamera);
        }

        private async Task setCameraControlStateAsync(bool state)
        {
            _logger.Debug("setCameraControlStateAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnSecurityDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.SET_MOTION_CONTROL_TASK.Action + (state ? "1" : "0");

            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            _downloadController.OnDownloadFinished += _setCameraControlStateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.SecurityCameraControl);
        }

        private void _securityDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_securityDownloadFinished");

            if (downloadType != DownloadType.Security)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            SecurityDto security = _jsonDataToSecurityConverter.GetList(response).First();
            if (security == null)
            {
                _logger.Error("Converted security is null!");

                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            _security = security;

            publishOnSecurityDownloadFinished(_security, true, response);
        }

        private void _setCameraStateFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_securityDownloadFinished");

            if (downloadType != DownloadType.SecurityCamera)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _setCameraStateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            loadSecurityAsync();
        }

        private void _setCameraControlStateFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_securityDownloadFinished");

            if (downloadType != DownloadType.SecurityCameraControl)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _setCameraControlStateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            loadSecurityAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _securityDownloadFinished;
            _downloadController.OnDownloadFinished -= _setCameraStateFinished;
            _downloadController.OnDownloadFinished -= _setCameraControlStateFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
