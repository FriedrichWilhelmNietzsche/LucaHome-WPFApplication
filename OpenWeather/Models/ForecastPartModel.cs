using Common.Enums;
using Common.Tools;
using OpenWeather.Common;
using System;

namespace OpenWeather.Models
{
    public class ForecastPartModel
    {
        private const string TAG = "ForecastPartModel";
        private Logger _logger;

        private string _description;

        private double _tempMin;
        private double _tempMax;
        private double _pressure;
        private double _humidity;

        private DateTime _dateTime;

        private WeatherCondition _condition;

        public ForecastPartModel(string description, double tempMin, double tempMax, double pressure, double humidity, DateTime dateTime, WeatherCondition condition)
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);
            
            _description = description;

            _tempMin = tempMin;
            _tempMax = tempMax;
            _humidity = humidity;
            _pressure = pressure;

            _dateTime = dateTime;

            _condition = condition;
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public double TemperatureMin
        {
            get
            {
                return _tempMin;
            }
        }

        public double TemperatureMax
        {
            get
            {
                return _tempMax;
            }
        }

        public double Humidity
        {
            get
            {
                return _humidity;
            }
        }

        public double Pressure
        {
            get
            {
                return _pressure;
            }
        }

        public DateTime Datetime
        {
            get
            {
                return _dateTime;
            }
        }

        public WeatherCondition Condition
        {
            get
            {
                return _condition;
            }
        }
    }
}
