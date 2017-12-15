using Common.Tools;
using Data.Controller;
using System.Net.NetworkInformation;

namespace Data.Services
{
    public class NetworkService
    {
        private const string TAG = "NetworkService";
        private readonly Logger _logger;

        private readonly SettingsController _settingsController;

        private static NetworkService _instance = null;
        private static readonly object _padlock = new object();

        NetworkService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
        }

        public static NetworkService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new NetworkService();
                    }

                    return _instance;
                }
            }
        }

        public string HomeSSID
        {
            get
            {
                return _settingsController.HomeSSID;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for HomeSSID!");
                    return;
                }

                _settingsController.HomeSSID = value;
            }
        }

        public string ServerIpAddress
        {
            get
            {
                return _settingsController.ServerIpAddress;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for ServerIpAddress!");
                    return;
                }

                _settingsController.ServerIpAddress = value;
            }
        }

        public bool IsNetworkAvailable
        {
            get
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
        }
    }
}
