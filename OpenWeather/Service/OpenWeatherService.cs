using Common.Tools;
using OpenWeather.Common;
using OpenWeather.Converter;
using OpenWeather.Downloader;
using OpenWeather.Models;
using System;
using System.Threading.Tasks;

namespace OpenWeather.Service
{
    public delegate void CurrentWeatherDownloadEventHandler(WeatherModel currentWeather, bool success);
    public delegate void ForecastWeatherDownloadEventHandler(ForecastModel forecastWeather, bool success);

    public class OpenWeatherService
    {
        private const String TAG = "OpenWeatherService";
        private readonly Logger _logger;

        private static OpenWeatherService _instance = null;
        private static readonly object _padlock = new object();

        private readonly OpenWeatherDownloader _openWeatherDownloader;
        private readonly JsonToWeatherConverter _jsonToWeatherConverter;

        private WeatherModel _currentWeather = null;
        private ForecastModel _forecastWeather = null;

        OpenWeatherService()
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);
            _logger.Information("Created new instance");

            _jsonToWeatherConverter = new JsonToWeatherConverter();
            _openWeatherDownloader = new OpenWeatherDownloader();
        }

        public event CurrentWeatherDownloadEventHandler OnCurrentWeatherDownloadFinished;
        private void OnCurrentWeatherDownloadFinishedHandler(WeatherModel currentWeather, bool success)
        {
            OnCurrentWeatherDownloadFinished?.Invoke(currentWeather, success);
        }

        public event ForecastWeatherDownloadEventHandler OnForecastWeatherDownloadFinished;
        private void OnForecastWeatherDownloadFinishedHandler(ForecastModel forecastWeather, bool success)
        {
            OnForecastWeatherDownloadFinished?.Invoke(forecastWeather, success);
        }

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
                return _openWeatherDownloader.City;
            }
            set
            {
                if (value == null || value == string.Empty)
                {
                    _logger.Error("Cannot set null value for City!");
                    return;
                }

                _openWeatherDownloader.City = value;
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

            if (!_openWeatherDownloader.Initialized)
            {
                _logger.Error("OpenWeatherDownloader not initialized!");
                return;
            }

            Task task = new Task(() => loadCurrentWeather());
            task.Start();
        }

        public void LoadForecastModel()
        {
            _logger.Debug("LoadForecastModel");

            if (!_openWeatherDownloader.Initialized)
            {
                _logger.Error("OpenWeatherDownloader not initialized!");
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

            OnCurrentWeatherDownloadFinishedHandler(_currentWeather, (newWeatherModel != null));
        }

        private void loadForecastModel()
        {
            _logger.Debug("loadForecastModel");

            string jsonString = _openWeatherDownloader.DownloadForecastWeatherJson();
            _logger.Debug(string.Format("Downloaded jsonString is {0}", jsonString));

            ForecastModel newForecastModel = _jsonToWeatherConverter.ConvertFromJsonToForecastWeather(jsonString);
            _logger.Debug(string.Format("New ForecastModel is {0}", newForecastModel));

            if (newForecastModel != null)
            {
                _forecastWeather = newForecastModel;
            }

            OnForecastWeatherDownloadFinishedHandler(_forecastWeather, (newForecastModel != null));
        }
    }
}
