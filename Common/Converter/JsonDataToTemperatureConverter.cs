using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToTemperatureConverter : IJsonDataConverter<TemperatureDto>
    {
        private const string TAG = "JsonDataToTemperatureConverter";
        private static string _searchParameter = "{\"Temperature\":";

        private readonly Logger _logger;

        public JsonDataToTemperatureConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<TemperatureDto> GetList(string[] stringArray)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(usedEntry);
            }
        }

        public IList<TemperatureDto> GetList(string jsonString)
        {
            return parseStringToList(jsonString);
        }

        private IList<TemperatureDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<TemperatureDto> temperatureList = new List<TemperatureDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Temperature");

                double temperatureValue = double.Parse(jsonObjectData["Value"].ToString());

                string area = jsonObjectData["Area"].ToString();

                string sensorPath = jsonObjectData["SensorPath"].ToString();
                string graphPath = jsonObjectData["GraphPath"].ToString();

                TemperatureDto security = new TemperatureDto(temperatureValue, area, DateTime.Now, sensorPath, TemperatureDto.TemperatureType.RASPBERRY, graphPath);
                temperatureList.Add(security);

                return temperatureList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<TemperatureDto>();
        }
    }
}
