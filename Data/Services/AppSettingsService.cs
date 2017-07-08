using Common.Common;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Threading.Tasks;

/* Reference Help
 * https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/using-application-settings-and-user-settings
 */

namespace Data.Services
{
    public delegate void UserCheckedEventHandler(string response, bool success);

    public class AppSettingsService
    {
        private const string TAG = "AppSettingsService";
        private readonly Logger _logger;

        private readonly DownloadController _downloadController;

        private static AppSettingsService _instance = null;
        private static readonly object _padlock = new object();

        AppSettingsService()
        {
            _logger = new Logger(TAG);

            _downloadController = new DownloadController();
        }

        public event UserCheckedEventHandler OnUserCheckedFinished;

        public static AppSettingsService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppSettingsService();
                    }

                    return _instance;
                }
            }
        }

        public bool EnteredUserData
        {
            get
            {
                string userName = Properties.Settings.Default.UserName;
                string passPhrase = Properties.Settings.Default.PassPhrase;

                if (userName.Equals("NA") || passPhrase.Equals("NA"))
                {
                    return false;
                }

                return true;
            }
        }

        public UserDto User
        {
            get
            {
                string userName = Properties.Settings.Default.UserName;
                string passPhrase = Properties.Settings.Default.PassPhrase;

                if (userName != null && passPhrase != null 
                    && userName != "NA" && passPhrase != "NA")
                {
                    UserDto user = new UserDto(userName, passPhrase);
                    _logger.Debug(string.Format("Returning user {0} from localSettings!", user));
                    return user;
                }

                _logger.Warning("No user found!");
                return null;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for user!");
                    return;
                }

                UserDto newUser = value;
                _logger.Debug(string.Format("Received new user {0} to save to settings!", newUser));

                Properties.Settings.Default.UserName = newUser.Name;
                Properties.Settings.Default.PassPhrase = newUser.Passphrase;

                Properties.Settings.Default.Save();

                if(newUser.Name != "NA" && newUser.Passphrase != "NA")
                {
                    validateUserAsync(newUser);
                }
            }
        }

        public string HomeSSID
        {
            get
            {
                return Properties.Settings.Default.HomeSSID;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for HomeSSID!");
                    return;
                }

                Properties.Settings.Default.HomeSSID = value;
                _logger.Debug(string.Format("Received new HomeSSID {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        public string OpenWeatherCity
        {
            get
            {
                return Properties.Settings.Default.OpenWeatherCity;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for OpenWeatherCity!");
                    return;
                }

                Properties.Settings.Default.OpenWeatherCity = value;
                _logger.Debug(string.Format("Received new OpenWeatherCity {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        public string ServerIpAddress
        {
            get
            {
                return Properties.Settings.Default.ServerIpAddress;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for ServerIpAddress!");
                    return;
                }

                Properties.Settings.Default.ServerIpAddress = value;
                _logger.Debug(string.Format("Received new ServerIpAddress {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        public int ServerPort
        {
            get
            {
                return Properties.Settings.Default.ServerPort;
            }
            set
            {
                Properties.Settings.Default.ServerPort = value;
                _logger.Debug(string.Format("Received new ServerPort {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        private async Task validateUserAsync(UserDto user)
        {
            _logger.Debug("validateUserAsync");

            string requestUrl = "http://" + ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.VALIDATE_USER.Action;
            
            _downloadController.OnDownloadFinished += _validateUserFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.User);
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

                User = new UserDto("NA", "NA");

                OnUserCheckedFinished(response, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Validation was not successful!");

                User = new UserDto("NA", "NA");

                OnUserCheckedFinished(response, false);
                return;
            }

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
