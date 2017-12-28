using Common.Dto;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static Common.Dto.ScheduleDto;

namespace Common.Converter
{
    public class JsonDataToScheduleConverter
    {
        private const string TAG = "JsonDataToScheduleConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToScheduleConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToScheduleConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToScheduleConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToScheduleConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<ScheduleDto> GetList(string[] stringArray, IList<WirelessSocketDto> socketList, IList<WirelessSwitchDto> switchList)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0], socketList, switchList);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(usedEntry, socketList, switchList);
            }
        }

        public IList<ScheduleDto> GetList(string jsonString, IList<WirelessSocketDto> socketList, IList<WirelessSwitchDto> switchList)
        {
            return parseStringToList(jsonString, socketList, switchList);
        }

        private IList<ScheduleDto> parseStringToList(string value, IList<WirelessSocketDto> socketList, IList<WirelessSwitchDto> switchList)
        {
            if (!value.Contains("Error"))
            {
                IList<ScheduleDto> scheduleList = new List<ScheduleDto>();

                try
                {
                    JObject jsonObject = JObject.Parse(value);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken scheduleJsonData = child["Schedule"];

                        bool isTimer = scheduleJsonData["IsTimer"].ToString() == "1";
                        if (isTimer)
                        {
                            continue;
                        }

                        int id = int.Parse(scheduleJsonData["Id"].ToString());

                        string name = scheduleJsonData["Name"].ToString();

                        string socketName = scheduleJsonData["Socket"].ToString();
                        WirelessSocketDto wirelessSocket = null;
                        for (int index = 0; index < socketList.Count; index++)
                        {
                            if (socketList[index].Name.Contains(socketName))
                            {
                                wirelessSocket = socketList[index];
                                break;
                            }
                        }

                        string gpioName = scheduleJsonData["Gpio"].ToString();
                        // TODO Gpios currently not supported in LucaHome WPF

                        string switchName = scheduleJsonData["Switch"].ToString();
                        WirelessSwitchDto wirelessSwitch = null;
                        for (int index = 0; index < switchList.Count; index++)
                        {
                            if (switchList[index].Name.Contains(switchName))
                            {
                                wirelessSwitch = switchList[index];
                                break;
                            }
                        }

                        int weekday = int.Parse(scheduleJsonData["Weekday"].ToString());

                        int hour = int.Parse(scheduleJsonData["Hour"].ToString());
                        int minute = int.Parse(scheduleJsonData["Minute"].ToString());

                        DateTime time = new DateTime(1970, 1, 1, hour, minute, 0);

                        int scheduleDayOfWeekInteger = (int)time.DayOfWeek;
                        int difference = scheduleDayOfWeekInteger - weekday;
                        time = time.AddDays(difference);

                        WirelessAction wirelessAction = scheduleJsonData["Action"].ToString() == "1" ? WirelessAction.Activate : WirelessAction.Deactivate;
                        bool isActive = scheduleJsonData["IsActive"].ToString() == "1";

                        ScheduleDto newSchedule = new ScheduleDto(id, name, wirelessSocket, wirelessSwitch, time, wirelessAction, isActive);
                        scheduleList.Add(newSchedule);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return scheduleList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<ScheduleDto>();
        }
    }
}
