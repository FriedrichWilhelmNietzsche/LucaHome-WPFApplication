using Common.Tools;
using OpenWeather.Common;
using OpenWeather.Converter;
using OpenWeather.Downloader;
using OpenWeather.Models;
using System;

namespace OpenWeather.Service
{
    public class OpenWeatherService
    {
        private const String TAG = "OpenWeatherService";
        private Logger _logger;

        private string _city;

        private OpenWeatherDownloader _openWeatherDownloader;
        private JsonToWeatherConverter _jsonToWeatherConverter;

        public OpenWeatherService(string city)
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);

            _city = city;

            _openWeatherDownloader = new OpenWeatherDownloader(_city);
            _jsonToWeatherConverter = new JsonToWeatherConverter();
        }

        public WeatherModel GetCurrentWeather()
        {
            string jsonString = _openWeatherDownloader.DownloadCurrentWeatherJson();
            _logger.Debug(string.Format("Downloaded jsonString is {0}", jsonString));

            WeatherModel newWeatherModel = _jsonToWeatherConverter.ConvertFromJsonToCurrentWeather(jsonString);
            _logger.Debug(string.Format("New WeatherModel is {0}", newWeatherModel));

            return newWeatherModel;
        }

        public ForecastModel GetForecastModel()
        {
            string jsonString = _openWeatherDownloader.DownloadForecastWeatherJson();
            _logger.Debug(string.Format("Downloaded jsonString is {0}", jsonString));

            ForecastModel newForecastModel = _jsonToWeatherConverter.ConvertFromJsonToForecastWeather(jsonString);
            _logger.Debug(string.Format("New ForecastModel is {0}", newForecastModel));

            return newForecastModel;
        }
    }
}
