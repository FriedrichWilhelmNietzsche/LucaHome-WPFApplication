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

        private readonly Logger _logger;

        public JsonDataToWirelessSwitchConverter()
        {
            _logger = new Logger(TAG);
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
            _logger.Debug(string.Format("GetList with jsonString {0}", jsonString));

            return parseStringToList(jsonString);
        }

        private IList<WirelessSwitchDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<WirelessSwitchDto> wirelessSwitchList = new List<WirelessSwitchDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                int id = 0;
                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken wirelessSwitchJsonData = child["WirelessSwitch"];

                    string name = wirelessSwitchJsonData["Name"].ToString();
                    string area = wirelessSwitchJsonData["Area"].ToString();

                    int remoteId = int.Parse(wirelessSwitchJsonData["RemoteId"].ToString());
                    char keyCode = char.Parse(wirelessSwitchJsonData["KeyCode"].ToString());

                    bool action = wirelessSwitchJsonData["Action"].ToString() == "1";

                    JToken lastTriggerJsonData = wirelessSwitchJsonData["LastTrigger"];

                    int year = int.Parse(lastTriggerJsonData["Year"].ToString());
                    int month = int.Parse(lastTriggerJsonData["Month"].ToString());
                    int day = int.Parse(lastTriggerJsonData["Day"].ToString());
                    int hour = int.Parse(lastTriggerJsonData["Minute"].ToString());
                    int minute = int.Parse(lastTriggerJsonData["Hour"].ToString());

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

                    WirelessSwitchDto newMenu = new WirelessSwitchDto(id, name, area, remoteId, keyCode, false, action, lastTriggerDate, lastTriggerUser);
                    wirelessSwitchList.Add(newMenu);

                    id++;
                }

                return wirelessSwitchList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<WirelessSwitchDto>();
        }
    }
}
