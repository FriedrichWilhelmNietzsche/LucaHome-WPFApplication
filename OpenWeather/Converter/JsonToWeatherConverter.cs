using Common.Converter;
using Common.Enums;
using Common.Tools;
using Newtonsoft.Json.Linq;
using OpenWeather.Common;
using OpenWeather.Models;
using System;
using System.Collections.Generic;

namespace OpenWeather.Converter
{
    public class JsonToWeatherConverter
    {
        private const string TAG = "JsonToWeatherConverter";
        private Logger _logger;

        private UnixToDateTimeConverter _unixToDateTimeConverter;

        public JsonToWeatherConverter()
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);
            _unixToDateTimeConverter = new UnixToDateTimeConverter();
        }

        public WeatherModel ConvertFromJsonToCurrentWeather(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            
            string city = jsonObject["name"].ToString();
            JToken locationJObject = jsonObject.GetValue("sys");
            string country = locationJObject["country"].ToString();

            JToken descriptionDataJObject = jsonObject.GetValue("weather")[0];
            string description = descriptionDataJObject["description"].ToString();
            WeatherCondition weatherCondition = WeatherCondition.GetByDescription(description);

            JToken weatherDataDataJObject = jsonObject.GetValue("main");
            string temperatureString = weatherDataDataJObject["temp"].ToString();
            string pressureString = weatherDataDataJObject["pressure"].ToString();
            string humidityString = weatherDataDataJObject["humidity"].ToString();

            double temperature = -273.15;
            double pressure = -1;
            double humiditiy = -1;
            try
            {
                temperature = Convert.ToDouble(temperatureString);
                pressure = Convert.ToDouble(pressureString);
                humiditiy = Convert.ToDouble(humidityString);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
            }

            string sunriseString = locationJObject["sunrise"].ToString();
            string sunsetString = locationJObject["sunset"].ToString();

            DateTime sunrise = _unixToDateTimeConverter.UnixTimeStampToDateTime(sunriseString);
            DateTime sunset = _unixToDateTimeConverter.UnixTimeStampToDateTime(sunsetString);

            string temperatureMinString = weatherDataDataJObject["temp_min"].ToString();
            string temperatureMaxString = weatherDataDataJObject["temp_max"].ToString();
            
            WeatherModel newWeatherModel = new WeatherModel(
                city,
                country,
                description,
                temperature,
                humiditiy,
                pressure,
                sunrise,
                sunset,
                DateTime.Now,
                weatherCondition);

            _logger.Debug(newWeatherModel.ToString());

            return newWeatherModel;
        }

        public ForecastModel ConvertFromJsonToForecastWeather(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);

            JToken locationJObject = jsonObject.Last.First;
            string city = locationJObject["name"].ToString();
            string country = locationJObject["country"].ToString();

            IList<ForecastPartModel> forecastList = new List<ForecastPartModel>();
            JToken dataJObject = jsonObject.GetValue("list");
            foreach(JToken dataEntry in dataJObject)
            {
                JToken mainDataJObject = dataEntry["main"];
                string temperatureMinString = mainDataJObject["temp_min"].ToString();
                string temperatureMaxString = mainDataJObject["temp_max"].ToString();
                string pressureString = mainDataJObject["pressure"].ToString();
                string humidityString = mainDataJObject["humidity"].ToString();

                double temperatureMin = -273.15;
                double temperatureMax = -273.15;
                double pressure = -1;
                double humiditiy = -1;
                try
                {
                    temperatureMin = Convert.ToDouble(temperatureMinString);
                    temperatureMax = Convert.ToDouble(temperatureMaxString);
                    pressure = Convert.ToDouble(pressureString);
                    humiditiy = Convert.ToDouble(humidityString);
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.ToString());
                }

                JToken weatherDataJObject = dataEntry["weather"][0];
                string description = weatherDataJObject["description"].ToString();
                WeatherCondition weatherCondition = WeatherCondition.GetByDescription(description);

                string dateTimeString = dataEntry["dt_txt"].ToString();
                DateTime dateTime = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                ForecastPartModel newForecastPartModel = new ForecastPartModel(
                    description,
                    temperatureMin,
                    temperatureMax,
                    pressure,
                    humiditiy,
                    dateTime,
                    weatherCondition);
                forecastList.Add(newForecastPartModel);

                _logger.Debug(newForecastPartModel.ToString());
            }
            
            ForecastModel newForecastModel = new ForecastModel(
                city,
                country,
                forecastList);

            _logger.Debug(newForecastModel.ToString());

            return newForecastModel;
        }
    }
}
