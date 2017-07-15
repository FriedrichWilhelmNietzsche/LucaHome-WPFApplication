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
        private readonly Logger _logger;

        private const int TIMEOUT = 5 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToTemperatureConverter _jsonDataToTemperatureConverter;
        private readonly OpenWeatherService _openWeatherService;

        private static TemperatureService _instance = null;
        private static readonly object _padlock = new object();

        private IList<TemperatureDto> _temperatureList = new List<TemperatureDto>();

        private Timer _downloadTimer;

        TemperatureService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToTemperatureConverter = new JsonDataToTemperatureConverter();
            _openWeatherService = OpenWeatherService.Instance;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event TemperatureDownloadEventHandler OnTemperatureDownloadFinished;

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
                return _openWeatherService?.CurrentWeather?.Condition?.Wallpaper;
            }
        }

        public Uri Icon
        {
            get
            {
                return _openWeatherService?.CurrentWeather?.Condition?.Icon;
            }
        }

        public string OpenWeatherCity
        {
            get
            {
                return _settingsController.OpenWeatherCity;
            }
            set
            {
                if (value == null)
                {
                    _logger.Error("Cannot add null value for OpenWeatherCity!");
                    return;
                }

                _settingsController.OpenWeatherCity = value;
            }
        }

        public void LoadTemperatureList()
        {
            _logger.Debug("LoadTemperatureList");
            loadTemperatureListAsync();
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            _openWeatherService.LoadCurrentWeather();
            _openWeatherService.LoadForecastModel();
            loadTemperatureListAsync();
        }

        private async Task loadTemperatureListAsync()
        {
            _logger.Debug("loadTemperatureListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnTemperatureDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_TEMPERATURES.Action;

            _downloadController.OnDownloadFinished += _temperatureDownloadFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.Temperature);
        }

        private void _temperatureDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_temperatureDownloadFinished");

            if (downloadType != DownloadType.Temperature)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _temperatureDownloadFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnTemperatureDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnTemperatureDownloadFinished(null, false, response);
                return;
            }

            IList<TemperatureDto> temperatureList = _jsonDataToTemperatureConverter.GetList(response);
            if (temperatureList == null)
            {
                _logger.Error("Converted temperatureList is null!");

                OnTemperatureDownloadFinished(null, false, response);
                return;
            }

            _temperatureList = temperatureList;

            WeatherModel currentWeather = _openWeatherService.CurrentWeather;
            if (currentWeather != null)
            {
                TemperatureDto currentWeatherTemperature = new TemperatureDto(currentWeather.Temperature, currentWeather.City, currentWeather.LastUpdate, string.Empty, TemperatureType.CITY, string.Empty);
                _temperatureList.Add(currentWeatherTemperature);
            }

            OnTemperatureDownloadFinished(_temperatureList, true, response);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _temperatureDownloadFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
