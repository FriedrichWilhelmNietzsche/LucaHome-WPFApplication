using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using static Common.Dto.SecurityDto;

namespace Common.Converter
{
    public class JsonDataToSecurityConverter : IJsonDataConverter<SecurityDto>
    {
        private const string TAG = "JsonDataToSecurityConverter";
        private static string _searchParameter = "{\"MotionData\":";

        private readonly Logger _logger;

        public JsonDataToSecurityConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<SecurityDto> GetList(string[] stringArray)
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

        public IList<SecurityDto> GetList(string responseString)
        {
            return parseStringToList(responseString);
        }

        private IList<SecurityDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<SecurityDto> securityList = new List<SecurityDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("MotionData");

                bool state = jsonObjectData["State"].ToString() == "ON";
                bool control = jsonObjectData["Control"].ToString() == "ON";

                string url = jsonObjectData["URL"].ToString();

                JToken eventsJsonData = jsonObjectData["Events"];
                IList<RegisteredEventDto> registeredEventList = new List<RegisteredEventDto>();
                int id = 0;

                foreach (JToken child in eventsJsonData.Children())
                {
                    JToken changeJsonData = child["Change"];

                    string name = child["FileName"].ToString();

                    RegisteredEventDto registeredEvent = new RegisteredEventDto(id, name);
                    registeredEventList.Add(registeredEvent);

                    id++;
                }

                SecurityDto security = new SecurityDto(state, control, url, registeredEventList);
                securityList.Add(security);

                return securityList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<SecurityDto>();
        }
    }
}
