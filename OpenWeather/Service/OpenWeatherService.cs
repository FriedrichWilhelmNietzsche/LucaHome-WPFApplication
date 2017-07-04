using Common.Tools;
using OpenWeather.Common;
using OpenWeather.Converter;
using OpenWeather.Downloader;
using OpenWeather.Models;
using System;
using System.Threading.Tasks;

namespace OpenWeather.Service
{
    public delegate string CurrentWeatherDownloadEventHandler(WeatherModel currentWeather, bool success);
    public delegate string ForecastWeatherDownloadEventHandler(ForecastModel forecastWeather, bool success);

    public class OpenWeatherService
    {
        private const String TAG = "OpenWeatherService";
        private Logger _logger;

        private static OpenWeatherService _instance = null;
        private static readonly object _padlock = new object();

        private string _city;
        private bool _initialized;

        private OpenWeatherDownloader _openWeatherDownloader;
        private JsonToWeatherConverter _jsonToWeatherConverter;

        private WeatherModel _currentWeather = null;
        private ForecastModel _forecastWeather = null;

        OpenWeatherService()
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);
            _jsonToWeatherConverter = new JsonToWeatherConverter();
        }

        public event CurrentWeatherDownloadEventHandler CurrentWeatherDownloadFinished;
        public event ForecastWeatherDownloadEventHandler ForecastWeatherDownloadFinished;

        public static OpenWeatherService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new OpenWeatherService();
                    }

                    return _instance;
                }
            }
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
                    _logger.Error("Cannot add null value for City!");
                    return;
                }

                _city = value;

                _openWeatherDownloader = new OpenWeatherDownloader(_city);

                _initialized = true;
            }
        }

        public WeatherModel CurrentWeather
        {
            get
            {
                return _currentWeather;
            }
        }

        public ForecastModel ForecastWeather
        {
            get
            {
                return _forecastWeather;
            }
        }

        public void LoadCurrentWeather()
        {
            _logger.Debug("LoadCurrentWeather");

            if (!_initialized)
            {
                _logger.Error("Not initialized!");
                return;
            }

            Task task = new Task(() => loadCurrentWeather());
            task.Start();
        }

        public void LoadForecastModel()
        {
            _logger.Debug("LoadForecastModel");

            if (!_initialized)
            {
                _logger.Error("Not initialized!");
                return;
            }

            Task task = new Task(() => loadForecastModel());
            task.Start();
        }

        private void loadCurrentWeather()
        {
            _logger.Debug("loadCurrentWeather");

            string jsonString = _openWeatherDownloader.DownloadCurrentWeatherJson();
            _logger.Debug(string.Format("Downloaded jsonString is {0}", jsonString));

            WeatherModel newWeatherModel = _jsonToWeatherConverter.ConvertFromJsonToCurrentWeather(jsonString);
            _logger.Debug(string.Format("New WeatherModel is {0}", newWeatherModel));

            if (newWeatherModel != null)
            {
                _currentWeather = newWeatherModel;
            }

            CurrentWeatherDownloadFinished(_currentWeather, (newWeatherModel != null));
        }

        private void loadForecastModel()
        {
            _logger.Debug("loadForecastModel");

            string jsonString = _openWeatherDownloader.DownloadForecastWeatherJson();
            _logger.Debug(string.Format("Downloaded jsonString is {0}", jsonString));

            ForecastModel newForecastModel = _jsonToWeatherConverter.ConvertFromJsonToForecastWeather(jsonString);
            _logger.Debug(string.Format("New ForecastModel is {0}", newForecastModel));

            if(newForecastModel != null)
            {
                _forecastWeather = newForecastModel;
            }

            ForecastWeatherDownloadFinished(_forecastWeather, (newForecastModel != null));
        }
    }
}
