using Common.Common;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Threading.Tasks;

namespace Data.Services
{
    public delegate void UserCheckedEventHandler(string response, bool success);

    public class UserService
    {
        private const string TAG = "UserService";
        private readonly Logger _logger;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;

        private static UserService _instance = null;
        private static readonly object _padlock = new object();

        private UserDto _tempUser = null;

        UserService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
        }

        public event UserCheckedEventHandler OnUserCheckedFinished;

        public static UserService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserService();
                    }

                    return _instance;
                }
            }
        }

        public UserDto User
        {
            get
            {
                return _settingsController.User;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for User!");
                    return;
                }

                _settingsController.User = value;
            }
        }

        public bool UserSaved()
        {
            UserDto user = _settingsController.User;

            if (!user.Name.Equals("NA") && !user.Passphrase.Equals("NA"))
            {
                return true;
            }

            return false;
        }

        public void ValidateUser()
        {
            _logger.Debug(string.Format("ValidateLoggedInUser: User {0}", User));
            validateUserAsync(User);
        }

        public void ValidateUser(UserDto user)
        {
            _logger.Debug(string.Format("ValidateUser: User {0}", user));
            validateUserAsync(user);
        }

        private async Task validateUserAsync(UserDto user)
        {
            _logger.Debug(string.Format("validateUserAsync: User {0}", user));

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.VALIDATE_USER.Action;

            _downloadController.OnDownloadFinished += _validateUserFinished;

            _tempUser = user;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.User);
        }

        private void _validateUserFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_validateUserFinished");

            if (downloadType != DownloadType.User)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _validateUserFinished;

            if (response.Contains("Error"))
            {
                _logger.Error(response);

                OnUserCheckedFinished(response, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Validation was not successful!");

                OnUserCheckedFinished(response, false);
                return;
            }

            _settingsController.User = _tempUser;
            _tempUser = null;

            OnUserCheckedFinished(response, true);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _validateUserFinished;

            _downloadController.Dispose();
        }
    }
}
