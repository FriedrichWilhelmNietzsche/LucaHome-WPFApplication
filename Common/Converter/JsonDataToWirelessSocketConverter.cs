using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToWirelessSocketConverter : IJsonDataConverter<WirelessSocketDto>
    {
        private const string TAG = "JsonDataToWirelessSocketConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToWirelessSocketConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToWirelessSocketConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToWirelessSocketConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToWirelessSocketConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<WirelessSocketDto> GetList(string[] jsonStringArray)
        {
            if (StringHelper.StringsAreEqual(jsonStringArray))
            {
                return parseStringToList(jsonStringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(jsonStringArray, _searchParameter);
                return parseStringToList(usedEntry);
            }
        }

        public IList<WirelessSocketDto> GetList(string jsonString)
        {
            return parseStringToList(jsonString);
        }

        private IList<WirelessSocketDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<WirelessSocketDto> wirelessSocketList = new List<WirelessSocketDto>();

                try
                {
                    JObject jsonObject = JObject.Parse(value);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken wirelessSocketJsonData = child["WirelessSocket"];

                        int typeId = int.Parse(wirelessSocketJsonData["TypeId"].ToString());

                        string name = wirelessSocketJsonData["Name"].ToString();
                        string area = wirelessSocketJsonData["Area"].ToString();
                        string code = wirelessSocketJsonData["Code"].ToString();

                        bool isActivated = wirelessSocketJsonData["State"].ToString() == "1";

                        JToken lastTriggerJsonData = wirelessSocketJsonData["LastTrigger"];

                        int year = int.Parse(lastTriggerJsonData["Year"].ToString());
                        int month = int.Parse(lastTriggerJsonData["Month"].ToString());
                        int day = int.Parse(lastTriggerJsonData["Day"].ToString());
                        int hour = int.Parse(lastTriggerJsonData["Hour"].ToString());
                        int minute = int.Parse(lastTriggerJsonData["Minute"].ToString());

                        if (year == -1 || month == -1 || day == -1 || hour == -1 || minute == -1)
                        {
                            year = 1970;
                            month = 1;
                            day = 1;
                            hour = 0;
                            minute = 0;
                        }

                        DateTime lastTriggerDate = new DateTime(year, month, day, hour, minute, 0);

                        string lastTriggerUser = lastTriggerJsonData["UserName"].ToString();

                        WirelessSocketDto newMenu = new WirelessSocketDto(typeId, name, area, code, isActivated, lastTriggerDate, lastTriggerUser);
                        wirelessSocketList.Add(newMenu);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return wirelessSocketList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<WirelessSocketDto>();
        }
    }
}
