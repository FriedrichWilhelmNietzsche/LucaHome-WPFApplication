using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using OpenWeather.Models;
using OpenWeather.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static Common.Dto.TemperatureDto;

namespace Data.Services
{
    public delegate void TemperatureDownloadEventHandler(IList<TemperatureDto> temperatureList, bool success, string response);

    public class TemperatureService
    {
        private const string TAG = "TemperatureService";
        private const int TIMEOUT = 5 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static TemperatureService _instance = null;
        private static readonly object _padlock = new object();

        private IList<TemperatureDto> _temperatureList = new List<TemperatureDto>();

        private Timer _downloadTimer;

        TemperatureService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _temperatureDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event TemperatureDownloadEventHandler OnTemperatureDownloadFinished;
        private void publishOnTemperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success, string response)
        {
            OnTemperatureDownloadFinished?.Invoke(temperatureList, success, response);
        }

        public static TemperatureService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new TemperatureService();
                    }

                    return _instance;
                }
            }
        }

        public IList<TemperatureDto> TemperatureList
        {
            get
            {
                return _temperatureList;
            }
        }

        public TemperatureDto GetByType(TemperatureType temperatureType)
        {
            TemperatureDto foundTemperature = _temperatureList
                        .Where(temperature => temperature.GetTemperatureType == temperatureType)
                        .Select(temperature => temperature)
                        .FirstOrDefault();
            return foundTemperature;
        }

        public Uri Wallpaper
        {
            get
            {
                return OpenWeatherService.Instance.CurrentWeather?.Condition?.WallpaperUri;
            }
        }

        public Uri Icon
        {
            get
            {
                return OpenWeatherService.Instance.CurrentWeather?.Condition?.Icon;
            }
        }

        public string OpenWeatherCity
        {
            get
            {
                return SettingsController.Instance.OpenWeatherCity;
            }
            set
            {
                if (value == null)
                {
                    Logger.Instance.Error(TAG, "Cannot add null value for OpenWeatherCity!");
                    return;
                }

                SettingsController.Instance.OpenWeatherCity = value;
            }
        }

        public bool SetWallpaperActive
        {
            get
            {
                return SettingsController.Instance.SetWallpaperActive;
            }
            set
            {
                SettingsController.Instance.SetWallpaperActive = value;
            }
        }

        public void LoadTemperatureList()
        {
            loadTemperatureListAsync();
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            OpenWeatherService.Instance.LoadCurrentWeather();
            OpenWeatherService.Instance.LoadForecastModel();
            loadTemperatureListAsync();
        }

        private async Task loadTemperatureListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnTemperatureDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_TEMPERATURES.Action);
            
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Temperature);
        }

        private void _temperatureDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.Temperature)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnTemperatureDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnTemperatureDownloadFinished(null, false, response);
                return;
            }

            IList<TemperatureDto> temperatureList = JsonDataToTemperatureConverter.Instance.GetList(response);
            if (temperatureList == null)
            {
                Logger.Instance.Error(TAG, "Converted temperatureList is null!");
                publishOnTemperatureDownloadFinished(null, false, response);
                return;
            }

            _temperatureList = temperatureList;

            WeatherModel currentWeather = OpenWeatherService.Instance.CurrentWeather;
            if (currentWeather != null)
            {
                TemperatureDto currentWeatherTemperature = new TemperatureDto(currentWeather.Temperature, "Outdoor", currentWeather.LastUpdate, string.Empty, TemperatureType.CITY, string.Empty);
                _temperatureList.Add(currentWeatherTemperature);
            }

            publishOnTemperatureDownloadFinished(_temperatureList, true, response);
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _temperatureDownloadFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
