using Common.Enums;
using System;

namespace OpenWeather.Models
{
    public class WeatherModel
    {
        private const string TAG = "WeatherModel";

        private string _city;
        private string _country;

        private string _description;

        private double _temperature;
        private double _humidity;
        private double _pressure;

        private DateTime _sunrise;
        private DateTime _sunset;

        private DateTime _lastUpdate;

        private WeatherCondition _condition;

        public WeatherModel(
            string city,
            string country,
            string description,
            double temperature,
            double humidity,
            double pressure,
            DateTime sunrise,
            DateTime sunset,
            DateTime lastUpdate,
            WeatherCondition condition)
        {
            _city = city;
            _country = country;
            _description = description;

            _temperature = temperature;
            _humidity = humidity;
            _pressure = pressure;

            _sunrise = sunrise;
            _sunset = sunset;

            _lastUpdate = lastUpdate;

            _condition = condition;
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

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public double Temperature
        {
            get
            {
                return _temperature;
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

        public DateTime Sunrise
        {
            get
            {
                return _sunrise;
            }
        }

        public DateTime Sunset
        {
            get
            {
                return _sunset;
            }
        }

        public DateTime LastUpdate
        {
            get
            {
                return _lastUpdate;
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
