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
using static Common.Dto.SecurityDto;
using System.IO;
using System.Diagnostics;

namespace Data.Services
{
    public delegate void SecurityDownloadEventHandler(SecurityDto security, bool success, string response);

    public class SecurityService
    {
        private const string TAG = "SecurityService";
        private const int TIMEOUT = 15 * 60 * 1000;

        private readonly DownloadController _downloadController;
        private readonly LocalDriveController _localDriveController;

        private static SecurityService _instance = null;
        private static readonly object _padlock = new object();

        private SecurityDto _security;
        private DriveInfo _raspberryDrive;

        private Timer _downloadTimer;

        SecurityService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _securityDownloadFinished;

            _localDriveController = new LocalDriveController();
            _raspberryDrive = _localDriveController.GetRaspberryDrive();
            if (_raspberryDrive == null)
            {
                Logger.Instance.Error(TAG, "Found no raspberry pi drive!");
            }

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
            if (searchKey == string.Empty)
            {
                return _security.RegisteredMotionEvents;
            }

            List<RegisteredEventDto> foundRegisteredEventList = _security.RegisteredMotionEvents
                        .Where(entry => entry.ToString().Contains(searchKey))
                        .Select(entry => entry)
                        .OrderBy(entry => entry.Id)
                        .ToList();
            return foundRegisteredEventList;
        }

        public void OpenFile(string registeredEventName)
        {
            if (_raspberryDrive == null)
            {
                Logger.Instance.Error(TAG, "_raspberryDrive is null!");
                return;
            }

            string registeredEventPathString = string.Format("{0}{1}{2}", _raspberryDrive.Name, "Camera\\", registeredEventName);
            Process.Start(@registeredEventPathString);
        }

        public void LoadSecurity()
        {
            loadSecurityAsync();
        }

        public void ToggleCameraState()
        {
            setCameraStateAsync(!_security.IsCameraActive);
        }

        public void ToggleCameraControlState()
        {
            setCameraControlStateAsync(!_security.IsCameraControlActive);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadSecurityAsync();
        }

        private async Task loadSecurityAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnSecurityDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_MOTION_DATA.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Security);
        }

        private async Task setCameraStateAsync(bool state)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnSecurityDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl;
            if (state)
            {
                requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                    SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                    user.Name, user.Passphrase,
                    LucaServerAction.START_MOTION.Action);
            }
            else
            {
                requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                    SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                    user.Name, user.Passphrase,
                    LucaServerAction.STOP_MOTION.Action);
            }

            _downloadController.OnDownloadFinished += _setCameraStateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.SecurityCamera);
        }

        private async Task setCameraControlStateAsync(bool state)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnSecurityDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}{5}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.SET_MOTION_CONTROL_TASK.Action,
                (state ? "1" : "0"));

            _downloadController.OnDownloadFinished += _setCameraControlStateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.SecurityCameraControl);
        }

        private void _securityDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.Security)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            SecurityDto security = JsonDataToSecurityConverter.Instance.GetList(response).First();
            if (security == null)
            {
                Logger.Instance.Error(TAG, "Converted security is null!");
                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            _security = security;
            publishOnSecurityDownloadFinished(_security, true, response);
        }

        private void _setCameraStateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.SecurityCamera)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _setCameraStateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            // Ugly hack to let the server change its' state
            Task.Delay(1500);

            loadSecurityAsync();
        }

        private void _setCameraControlStateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.SecurityCameraControl)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _setCameraControlStateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnSecurityDownloadFinished(null, false, response);
                return;
            }

            // Ugly hack to let the server change its' state
            Task.Delay(1500);

            loadSecurityAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _securityDownloadFinished;
            _downloadController.OnDownloadFinished -= _setCameraStateFinished;
            _downloadController.OnDownloadFinished -= _setCameraControlStateFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
