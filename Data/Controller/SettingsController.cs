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

        private static SettingsController _instance = null;
        private static readonly object _padlock = new object();

        SettingsController()
        {
            // Empty constructor, nothing needed here
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
                    Logger.Instance.Debug(TAG, string.Format("Returning user {0} from localSettings!", user));
                    return user;
                }

                Logger.Instance.Warning(TAG, "No user found!");
                return new UserDto("NA", "NA");
            }
            set
            {
                if (value == null)
                {
                    Logger.Instance.Error(TAG, "Cannot add null value for user!");
                    return;
                }

                UserDto newUser = value;
                Logger.Instance.Debug(TAG, string.Format("Received new user {0} to save to settings!", newUser));

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
                    Logger.Instance.Error(TAG, "Cannot add null value for HomeSSID!");
                    return;
                }

                Properties.Settings.Default.HomeSSID = value;
                Logger.Instance.Debug(TAG, string.Format("Received new HomeSSID {0} to save to settings!", value));

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
                    Logger.Instance.Error(TAG, "Cannot add null value for ServerIpAddress!");
                    return;
                }

                Properties.Settings.Default.ServerIpAddress = value;
                Logger.Instance.Debug(TAG, string.Format("Received new ServerIpAddress {0} to save to settings!", value));

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
                    Logger.Instance.Error(TAG, "Cannot add null value for OpenWeatherCity!");
                    return;
                }

                Properties.Settings.Default.OpenWeatherCity = value;
                Logger.Instance.Debug(TAG, string.Format("Received new OpenWeatherCity {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        public bool SetWallpaperActive
        {
            get
            {
                return Properties.Settings.Default.SetWallpaperActive;
            }
            set
            {
                Properties.Settings.Default.SetWallpaperActive = value;
                Logger.Instance.Debug(TAG, string.Format("Received new SetWallpaperActive {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        public int CoinHourTrend
        {
            get
            {
                return Properties.Settings.Default.CoinHourTrend;
            }
            set
            {
                Properties.Settings.Default.CoinHourTrend = value;
                Logger.Instance.Debug(TAG, string.Format("Received new CoinHourTrend {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        public int MediaServerPort
        {
            get
            {
                return Properties.Settings.Default.MediaServerPort;
            }
            set
            {
                Properties.Settings.Default.MediaServerPort = value;
                Logger.Instance.Debug(TAG, string.Format("Received new MediaServerPort {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }

        public int YoutubeMaxCount
        {
            get
            {
                return Properties.Settings.Default.YoutubeMaxCount;
            }
            set
            {
                Properties.Settings.Default.YoutubeMaxCount = value;
                Logger.Instance.Debug(TAG, string.Format("Received new YoutubeMaxCount {0} to save to settings!", value));

                Properties.Settings.Default.Save();
            }
        }
    }
}
