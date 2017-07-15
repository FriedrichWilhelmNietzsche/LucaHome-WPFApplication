using Common.Dto;
using Common.Tools;

/* Reference Help
 * https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/using-application-settings-and-user-settings
 */

namespace Data.Controller
{
    public class SettingsController
    {
        private const string TAG = "SettingsController";
        private readonly Logger _logger;

        private readonly DownloadController _downloadController;

        private static SettingsController _instance = null;
        private static readonly object _padlock = new object();

        SettingsController()
        {
            _logger = new Logger(TAG);

            _downloadController = new DownloadController();
        }

        public static SettingsController Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new SettingsController();
                    }

                    return _instance;
                }
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
                return new UserDto("NA", "NA");
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
    }
}
