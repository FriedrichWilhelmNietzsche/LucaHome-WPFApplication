using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToWirelessSwitchConverter : IJsonDataConverter<WirelessSwitchDto>
    {
        private const string TAG = "JsonDataToWirelessSwitchConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToWirelessSwitchConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToWirelessSwitchConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToWirelessSwitchConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToWirelessSwitchConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<WirelessSwitchDto> GetList(string[] jsonStringArray)
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

        public IList<WirelessSwitchDto> GetList(string jsonString)
        {
            return parseStringToList(jsonString);
        }

        private IList<WirelessSwitchDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<WirelessSwitchDto> wirelessSwitchList = new List<WirelessSwitchDto>();

                try
                {
                    JObject jsonObject = JObject.Parse(value);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken wirelessSwitchJsonData = child["WirelessSwitch"];

                        int typeId = int.Parse(wirelessSwitchJsonData["TypeId"].ToString());

                        string name = wirelessSwitchJsonData["Name"].ToString();
                        string area = wirelessSwitchJsonData["Area"].ToString();

                        int remoteId = int.Parse(wirelessSwitchJsonData["RemoteId"].ToString());
                        char keyCode = char.Parse(wirelessSwitchJsonData["KeyCode"].ToString());

                        bool action = wirelessSwitchJsonData["Action"].ToString() == "1";

                        JToken lastTriggerJsonData = wirelessSwitchJsonData["LastTrigger"];

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

                        WirelessSwitchDto newMenu = new WirelessSwitchDto(typeId, name, area, remoteId, keyCode, false, action, lastTriggerDate, lastTriggerUser);
                        wirelessSwitchList.Add(newMenu);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return wirelessSwitchList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<WirelessSwitchDto>();
        }
    }
}
