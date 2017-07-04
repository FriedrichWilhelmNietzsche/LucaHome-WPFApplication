using Common.Dto;
using Common.Tools;

/* Reference Help
 * https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/using-application-settings-and-user-settings
 */

namespace Data.Services
{
    public class AppSettingsService
    {
        private const string TAG = "AppSettingsService";
        private Logger _logger;

        public AppSettingsService()
        {
            _logger = new Logger(TAG);
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

                if (userName != null && passPhrase != null)
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
    }
}
