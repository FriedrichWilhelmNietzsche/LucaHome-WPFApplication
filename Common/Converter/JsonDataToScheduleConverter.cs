using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Common.Dto.ScheduleDto;

namespace Common.Converter
{
    public class JsonDataToScheduleConverter
    {
        private const string TAG = "JsonDataToScheduleConverter";
        private static string _searchParameter = "{schedule:";

        private readonly Logger _logger;

        public JsonDataToScheduleConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<ScheduleDto> GetList(string[] stringArray, IList<WirelessSocketDto> socketList)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0], socketList);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(usedEntry, socketList);
            }
        }

        public IList<ScheduleDto> GetList(string jsonString, IList<WirelessSocketDto> socketList)
        {
            return parseStringToList(jsonString, socketList);
        }

        private IList<ScheduleDto> parseStringToList(string value, IList<WirelessSocketDto> socketList)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<ScheduleDto> list = new List<ScheduleDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 0; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            ScheduleDto newValue = parseStringToValue(index, data, socketList);
                            if (newValue != null)
                            {
                                list.Add(newValue);
                            }
                        }

                        return list;
                    }
                }
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<ScheduleDto>();
        }

        private ScheduleDto parseStringToValue(int id, string[] data, IList<WirelessSocketDto> socketList)
        {
            if (data.Length == 11)
            {
                if (data[0].Contains("{Name:")
                    && data[1].Contains("{Socket:")
                    && data[2].Contains("{Gpio:")
                    && data[3].Contains("{Weekday:")
                    && data[4].Contains("{Hour:")
                    && data[5].Contains("{Minute:")
                    && data[6].Contains("{OnOff:")
                    && data[7].Contains("{IsTimer:")
                    && data[8].Contains("{PlaySound:")
                    && data[9].Contains("{Raspberry:")
                    && data[10].Contains("{State:"))
                {
                    string name = data[0].Replace("{Name:", "").Replace("};", "");

                    string socketName = data[1].Replace("{Socket:", "").Replace("};", "");
                    WirelessSocketDto socket = null;
                    for (int index = 0; index < socketList.Count; index++)
                    {
                        if (socketList[index].Name.Contains(socketName))
                        {
                            socket = socketList[index];
                            break;
                        }
                    }

                    //string gpioString = data[2].Replace("{Gpio:", "").Replace("};", "");
                    string weekdayString = data[3].Replace("{Weekday:", "").Replace("};", "");
                    string hourString = data[4].Replace("{Hour:", "").Replace("};", "");
                    string minuteString = data[5].Replace("{Minute:", "").Replace("};", "");
                    string actionString = data[6].Replace("{OnOff:", "").Replace("};", "");
                    string isTimerString = data[7].Replace("{IsTimer:", "").Replace("};", "");
                    //string playSoundString = data[8].Replace("{PlaySound:", "").Replace("};", "");
                    //string playRaspberryString = data[9].Replace("{Raspberry:", "").Replace("};", "");
                    string isActiveString = data[10].Replace("{State:", "").Replace("};", "");


                    int weekdayInteger = -1;
                    bool parseSuccessWeekday = int.TryParse(weekdayString, out weekdayInteger);
                    if (!parseSuccessWeekday)
                    {
                        _logger.Warning("Failed to parse weekdayInteger from data!");
                    }
                    Weekday weekday = (Weekday)weekdayInteger;

                    int hour = -1;
                    bool parseSuccessHour = int.TryParse(hourString, out hour);
                    if (!parseSuccessHour)
                    {
                        _logger.Warning("Failed to parse hour from data!");
                    }

                    int minute = -1;
                    bool parseSuccessMinute = int.TryParse(minuteString, out minute);
                    if (!parseSuccessMinute)
                    {
                        _logger.Warning("Failed to parse minute from data!");
                    }

                    DateTime time = new DateTime(0, 0, 0, hour, minute, 0);

                    bool action = actionString.Contains("1");
                    bool isTimer = isTimerString.Contains("1");
                    bool isActive = isActiveString.Contains("1");

                    if (!isTimer)
                    {
                        return new ScheduleDto(id, name, "", socket, weekday, time, (action ? SocketAction.Activate : SocketAction.Deactivate), isActive);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    _logger.Error("data contains invalid entries!");
                }
            }
            else
            {
                _logger.Error(string.Format("Data has invalid length {0}", data.Length));
            }

            _logger.Error(string.Format("{0} has an error!", data));

            return null;
        }
    }
}
