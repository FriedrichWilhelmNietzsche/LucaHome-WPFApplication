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

        private static JsonDataToTemperatureConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToTemperatureConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToTemperatureConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToTemperatureConverter();
                    }

                    return _instance;
                }
            }
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

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<TemperatureDto>();
        }
    }
}
