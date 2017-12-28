using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static Common.Dto.SecurityDto;

namespace Common.Converter
{
    public class JsonDataToSecurityConverter : IJsonDataConverter<SecurityDto>
    {
        private const string TAG = "JsonDataToSecurityConverter";
        private static string _searchParameter = "{\"MotionData\":";

        private static JsonDataToSecurityConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToSecurityConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToSecurityConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToSecurityConverter();
                    }

                    return _instance;
                }
            }
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

                try
                {
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
                        string name = child["FileName"].ToString();

                        RegisteredEventDto registeredEvent = new RegisteredEventDto(id, name);
                        registeredEventList.Add(registeredEvent);

                        id++;
                    }

                    SecurityDto security = new SecurityDto(state, control, url, registeredEventList);
                    securityList.Add(security);
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return securityList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<SecurityDto>();
        }
    }
}
