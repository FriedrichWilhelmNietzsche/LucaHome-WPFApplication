using Common.Tools;
using Newtonsoft.Json.Linq;
using OpenWeather.Common;
using OpenWeather.Models;

namespace OpenWeather.Converter
{
    public class JsonToWeatherConverter
    {
        private const string TAG = "JsonToWeatherConverter";
        private Logger _logger;

        public JsonToWeatherConverter()
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);
        }

        public WeatherModel ConvertFromJsonToCurrentWeather(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            
            string city = jsonObject["name"].ToString();
            JToken locationJObject = jsonObject.GetValue("sys");
            string country = locationJObject["country"].ToString();

            string sunriseString = locationJObject["sunrise"].ToString();
            string sunsetString = locationJObject["sunset"].ToString();

            JToken descriptionDataJObject = jsonObject.GetValue("weather")[0];
            string description = descriptionDataJObject["description"].ToString();

            JToken weatherDataDataJObject = jsonObject.GetValue("main");
            string temperatureString = weatherDataDataJObject["temp"].ToString();
            string pressureString = weatherDataDataJObject["pressure"].ToString();
            string humidityString = weatherDataDataJObject["humidity"].ToString();

            string temperatureMinString = weatherDataDataJObject["temp_min"].ToString();
            string temperatureMaxString = weatherDataDataJObject["temp_max"].ToString();

            // TODO convert to WeatherModel

            return null;
        }

        public ForecastModel ConvertFromJsonToForecastWeather(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);

            JToken locationJObject = jsonObject.Last.First;
            string city = locationJObject["name"].ToString();
            string country = locationJObject["country"].ToString();

            JToken dataJObject = jsonObject.GetValue("list");
            string description = jsonObject["description"].ToString();

            // TODO get rest of data and convert to ForecastModel

            return null;
        }
    }
}
