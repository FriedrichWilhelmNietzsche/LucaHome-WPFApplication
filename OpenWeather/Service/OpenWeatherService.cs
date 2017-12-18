using Common.Enums;
using Common.Tools;
using OpenWeather.Controller;
using OpenWeather.Converter;
using OpenWeather.Downloader;
using OpenWeather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace OpenWeather.Service
{
    public delegate void CurrentWeatherDownloadEventHandler(WeatherModel currentWeather, bool success);
    public delegate void ForecastWeatherDownloadEventHandler(ForecastModel forecastWeather, bool success);

    public class OpenWeatherService
    {
        private const String TAG = "OpenWeatherService";

        private const int TIMEOUT = 5 * 60 * 1000;

        private static OpenWeatherService _instance = null;
        private static readonly object _padlock = new object();

        private readonly OpenWeatherDownloader _openWeatherDownloader;
        private readonly JsonToWeatherConverter _jsonToWeatherConverter;

        private bool _setWallpaperActive = false;

        private WeatherModel _currentWeather = null;
        private ForecastModel _forecastWeather = null;

        private Timer _downloadTimer;

        OpenWeatherService()
        {
            Logger.Instance.Information(TAG, "Created new instance");

            _jsonToWeatherConverter = new JsonToWeatherConverter();
            _openWeatherDownloader = new OpenWeatherDownloader();

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
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

        public ForecastModel FoundForecastEntries(string searchKey)
        {
            List<ForecastPartModel> foundForecastEntries;

            DateTime today = DateTime.Now;
            DateTime tomorrow = new DateTime(today.Year, today.Month, today.Day + 1);

            switch (searchKey)
            {
                case "Today":
                case "Heute":
                    foundForecastEntries = _forecastWeather.List
                        .Where(forecastEntry => forecastEntry.Datetime.Date == today.Date)
                        .Select(forecastEntry => forecastEntry)
                        .ToList();
                    break;

                case "Tomorrow":
                case "Morgen":
                    foundForecastEntries = _forecastWeather.List
                        .Where(forecastEntry => forecastEntry.Datetime.Date == tomorrow.Date)
                        .Select(forecastEntry => forecastEntry)
                        .ToList();
                    break;

                default:
                    foundForecastEntries = _forecastWeather.List
                        .Where(forecastEntry =>
                            forecastEntry.Condition.ToString().Contains(searchKey)
                            || forecastEntry.DateTimeString.Contains(searchKey)
                            || forecastEntry.Description.Contains(searchKey)
                            || forecastEntry.HumidityString.Contains(searchKey)
                            || forecastEntry.PressureString.Contains(searchKey)
                            || forecastEntry.TemperatureString.Contains(searchKey))
                        .Select(forecastEntry => forecastEntry)
                        .ToList();
                    break;
            }

            ForecastModel foundForecastModel = new ForecastModel(_forecastWeather.City, _forecastWeather.Country, foundForecastEntries);

            return foundForecastModel;
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
                    Logger.Instance.Error(TAG, "Cannot set null value for City!");
                    return;
                }

                _openWeatherDownloader.City = value;

                LoadCurrentWeather();
                LoadForecastModel();
            }
        }

        public bool SetWallpaperActive
        {
            get
            {
                return _setWallpaperActive;
            }
            set
            {
                _setWallpaperActive = value;
                setWallpaper();
            }
        }

        public WeatherModel CurrentWeather
        {
            get
            {
                if (_currentWeather == null)
                {
                    return new WeatherModel("Null", "Null", "Null", -273.15, -1, -1, DateTime.Now, DateTime.Now, DateTime.Now, WeatherCondition.NULL);
                }

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
            if (!_openWeatherDownloader.Initialized)
            {
                Logger.Instance.Error(TAG, "OpenWeatherDownloader not initialized!");
                return;
            }

            Task task = new Task(() => loadCurrentWeather());
            task.Start();
        }

        public void LoadForecastModel()
        {
            if (!_openWeatherDownloader.Initialized)
            {
                Logger.Instance.Error(TAG, "OpenWeatherDownloader not initialized!");
                return;
            }

            Task task = new Task(() => loadForecastModel());
            task.Start();
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadCurrentWeather();
            loadForecastModel();
        }

        private void loadCurrentWeather()
        {
            string jsonString = _openWeatherDownloader.DownloadCurrentWeatherJson();

            WeatherModel newWeatherModel = _jsonToWeatherConverter.ConvertFromJsonToCurrentWeather(jsonString);

            if (newWeatherModel != null)
            {
                _currentWeather = newWeatherModel;
            }
            else
            {
                Logger.Instance.Error(TAG, "newWeatherModel is null!");
            }

            OnCurrentWeatherDownloadFinishedHandler(_currentWeather, (newWeatherModel != null));
            setWallpaper();
        }

        private void loadForecastModel()
        {
            string jsonString = _openWeatherDownloader.DownloadForecastWeatherJson();

            ForecastModel newForecastModel = _jsonToWeatherConverter.ConvertFromJsonToForecastWeather(jsonString);

            if (newForecastModel != null)
            {
                _forecastWeather = newForecastModel;
            }
            else
            {
                Logger.Instance.Error(TAG, "newForecastModel is null!");
            }

            OnForecastWeatherDownloadFinishedHandler(_forecastWeather, (newForecastModel != null));
        }

        private void setWallpaper()
        {
            if (!_setWallpaperActive)
            {
                return;
            }

            if (_currentWeather == null)
            {
                Logger.Instance.Error(TAG, "CurrentWeather is null!");
                return;
            }

            WallpaperController.SetDesktopWallpaperFromBitmap(_currentWeather.Condition.Wallpaper, Style.Fill);
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
