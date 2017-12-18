using Common.Tools;
using OpenWeather.Common;
using System;
using System.Net;

/* Reference Help
 * https://stackoverflow.com/questions/11891082/how-can-i-download-a-website-content-to-a-string
 * https://stackoverflow.com/questions/5566942/how-to-get-a-json-string-from-url
 */

namespace OpenWeather.Downloader
{
    public class OpenWeatherDownloader
    {
        private const String TAG = "OpenWeatherDownloader";

        private String _city;
        private bool _initialized;

        public OpenWeatherDownloader()
        {
            // Nothing to do here
        }

        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                if (value == null || value == string.Empty)
                {
                    Logger.Instance.Error(TAG, "Cannot set null value to City!");
                    return;
                }

                _city = value;
                _initialized = true;
            }
        }

        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        public string DownloadCurrentWeatherJson()
        {
            if (!_initialized || _city == null || _city.Length == 0)
            {
                Logger.Instance.Warning(TAG, "You have to set the city before calling the weather!");
                return string.Empty;
            }

            string action = string.Format(OWAction.CALL_CURRENT_WEATHER, _city);
            string result = new WebClient().DownloadString(action);

            return result;
        }

        public string DownloadForecastWeatherJson()
        {
            if (!_initialized || _city == null || _city.Length == 0)
            {
                Logger.Instance.Warning(TAG, "You have to set the city before calling the weather!");
                return string.Empty;
            }

            string action = string.Format(OWAction.CALL_FORECAST_WEATHER, _city);
            string result = new WebClient().DownloadString(action);

            return result;
        }
    }
}
