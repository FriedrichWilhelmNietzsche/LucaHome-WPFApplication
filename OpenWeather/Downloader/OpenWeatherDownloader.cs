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
        private Logger _logger;

        private String _city;

        public OpenWeatherDownloader(string city)
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);

            _city = city;
        }

        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot set null value to City!");
                    return;
                }

                _city = value;
            }
        }

        public string DownloadCurrentWeatherJson()
        {
            if (_city == null || _city.Length == 0)
            {
                _logger.Warning("You have to set the city berfore calling the weather!");
                return string.Empty;
            }

            string action = string.Format(OWAction.CALL_CURRENT_WEATHER, _city);
            string result = new WebClient().DownloadString(action);

            return result;
        }

        public string DownloadForecastWeatherJson()
        {
            if (_city == null || _city.Length == 0)
            {
                _logger.Warning("You have to set the city berfore calling the weather!");
                return string.Empty;
            }

            string action = string.Format(OWAction.CALL_FORECAST_WEATHER, _city);
            string result = new WebClient().DownloadString(action);

            return result;
        }
    }
}
