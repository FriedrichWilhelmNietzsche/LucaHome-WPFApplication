using Common.Tools;
using Data.Controller;
using System.Net.NetworkInformation;

namespace Data.Services
{
    public class NetworkService
    {
        private const string TAG = "NetworkService";

        private static NetworkService _instance = null;
        private static readonly object _padlock = new object();

        NetworkService()
        {
            // Empty constructor, nothing needed here
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
                return SettingsController.Instance.HomeSSID;
            }
            set
            {
                if (value == null)
                {
                    Logger.Instance.Error(TAG, "Cannot add null value for HomeSSID!");
                    return;
                }

                SettingsController.Instance.HomeSSID = value;
            }
        }

        public string ServerIpAddress
        {
            get
            {
                return SettingsController.Instance.ServerIpAddress;
            }
            set
            {
                if (value == null)
                {
                    Logger.Instance.Error(TAG, "Cannot add null value for ServerIpAddress!");
                    return;
                }

                SettingsController.Instance.ServerIpAddress = value;
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
