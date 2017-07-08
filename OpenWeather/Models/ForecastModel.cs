using Common.Enums;
using Common.Tools;
using OpenWeather.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenWeather.Models
{
    public class ForecastModel
    {
        private const string TAG = "ForecastModel";
        private Logger _logger;

        private string _city;
        private string _country;
        private IList<ForecastPartModel> _list = new List<ForecastPartModel>();

        public ForecastModel(
            string city,
            string country,
            IList<ForecastPartModel> list)
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);

            _city = city;
            _country = country;
            _list = list;
        }

        public string City
        {
            get
            {
                return _city;
            }
        }

        public string Country
        {
            get
            {
                return _country;
            }
        }

        public IList<ForecastPartModel> List
        {
            get
            {
                return _list;
            }
        }

        public Uri Icon
        {
            get
            {
                WeatherCondition mostWeatherConditions = _list
                    .GroupBy(v => v)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key.Condition)
                    .FirstOrDefault();

                return mostWeatherConditions?.Icon;
            }
        }

        public Uri Wallpaper
        {
            get
            {
                WeatherCondition mostWeatherConditions = _list
                    .GroupBy(v => v)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key.Condition)
                    .FirstOrDefault();

                return mostWeatherConditions?.Wallpaper;
            }
        }
    }
}
