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

        private readonly Logger _logger;

        public JsonDataToScheduleConverter()
        {
            _logger = new Logger(TAG);
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

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                int id = 0;
                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken scheduleJsonData = child["Schedule"];

                    bool isTimer = scheduleJsonData["IsTimer"].ToString() == "1";
                    if (isTimer)
                    {
                        continue;
                    }

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

                    WirelessAction wirelessAction = scheduleJsonData["OnOff"].ToString() == "1" ? WirelessAction.Activate : WirelessAction.Deactivate;
                    bool isActive = scheduleJsonData["State"].ToString() == "1";

                    ScheduleDto newSchedule = new ScheduleDto(id, name, wirelessSocket, wirelessSwitch, time, wirelessAction, isActive);
                    scheduleList.Add(newSchedule);

                    id++;
                }

                return scheduleList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<ScheduleDto>();
        }
    }
}
