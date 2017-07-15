using Common.Dto;
using Common.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using static Common.Dto.MapContentDto;

namespace Common.Converter
{
    public class JsonDataToMapContentConverter
    {
        private const string TAG = "JsonDataToMapContentConverter";
        private static string _searchParameter = "{mapcontent:";

        private readonly Logger _logger;

        public JsonDataToMapContentConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<MapContentDto> GetList(string[] stringArray, IList<WirelessSocketDto> socketList, IList<ScheduleDto> scheduleList)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return ParseStringToList(stringArray[0], socketList, scheduleList);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return ParseStringToList(usedEntry, socketList, scheduleList);
            }
        }

        public IList<MapContentDto> GetList(string jsonString, IList<WirelessSocketDto> socketList, IList<ScheduleDto> scheduleList)
        {
            return ParseStringToList(jsonString, socketList, scheduleList);
        }

        private IList<MapContentDto> ParseStringToList(string value, IList<WirelessSocketDto> socketList, IList<ScheduleDto> scheduleList)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 0)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<MapContentDto> list = new List<MapContentDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 0; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            MapContentDto newValue = ParseStringToValue(data, socketList, scheduleList);
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

            return new List<MapContentDto>();
        }

        private MapContentDto ParseStringToValue(string[] data, IList<WirelessSocketDto> socketList, IList<ScheduleDto> scheduleList)
        {
            if (data.Length == 8)
            {
                _logger.Warning("Length is 8! trying to fix!");
                if (data[0].Length == 0)
                {
                    _logger.Warning("Value at index 0 is null! Trying to fix...");
                    string[] fixedData = new string[7];
                    for (int index = 1; index < data.Length; index++)
                    {
                        fixedData[index - 1] = data[index];
                    }
                    data = fixedData;
                }
            }

            if (data.Length == 7)
            {
                if (data[0].Contains("{id:")
                    && data[1].Contains("{position:")
                    && data[2].Contains("{type:")
                    && data[3].Contains("{schedules:")
                    && data[4].Contains("{sockets:")
                    && data[5].Contains("{temperatureArea:")
                    && data[6].Contains("{visibility:"))
                {
                    string idString = data[0].Replace("{id:", "").Replace("};", "");
                    int id = -1;
                    bool parseSuccessId = int.TryParse(idString, out id);
                    if (!parseSuccessId)
                    {
                        _logger.Error("Failed to parse id from data!");
                        return null;
                    }

                    string positionString = data[1].Replace("{position:", "").Replace("};", "");
                    string[] coordinates = Regex.Split(positionString, "\\|");
                    int x = -1;
                    int y = -1;
                    bool parseSuccessX = int.TryParse(coordinates[0], out x);
                    bool parseSuccessY = int.TryParse(coordinates[1], out y);
                    if (!parseSuccessX || !parseSuccessY)
                    {
                        _logger.Error("Failed to parse position from data!");
                        return null;
                    }
                    int[] position = { x, y };

                    string typeString = data[2].Replace("{type:", "").Replace("};", "");
                    int typeInteger = 0;
                    bool parseSuccessType = int.TryParse(typeString, out typeInteger);
                    if (!parseSuccessType)
                    {
                        _logger.Error("Failed to parse type from data!");
                        return null;
                    }
                    DrawingType type = (DrawingType)typeInteger;

                    IList<ScheduleDto> mapContentScheduleList = new List<ScheduleDto>();
                    string scheduleListString = data[3].Replace("{schedules:", "").Replace("};", "");
                    string[] scheduleStringList = Regex.Split(scheduleListString, "\\|");
                    foreach (string entry in scheduleStringList)
                    {
                        foreach (ScheduleDto schedule in scheduleList)
                        {
                            if (entry.Contains(schedule.Name))
                            {
                                mapContentScheduleList.Add(schedule);
                                break;
                            }
                        }
                    }

                    WirelessSocketDto mapContentSocket = null;
                    string socketListString = data[4].Replace("{sockets:", "").Replace("};", "");
                    string[] socketStringList = Regex.Split(socketListString, "\\|");
                    bool foundSocket = false;
                    foreach (string entry in socketStringList)
                    {
                        foreach (WirelessSocketDto socket in socketList)
                        {
                            if (entry.Contains(socket.Name))
                            {
                                mapContentSocket = socket;
                                foundSocket = true;
                                break;
                            }
                        }

                        if (foundSocket)
                        {
                            break;
                        }
                    }

                    string temperatureArea = data[5].Replace("{temperatureArea:", "").Replace("};", "");

                    string visibilityString = data[6].Replace("{visibility:", "").Replace("};", "");
                    Visibility visibility = visibilityString.Contains("1") ? Visibility.Visible : Visibility.Collapsed;

                    return new MapContentDto(id, position, type, temperatureArea, mapContentSocket, mapContentScheduleList, visibility);
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
