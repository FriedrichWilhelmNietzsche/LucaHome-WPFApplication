using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToChangeConverter
    {
        private const string TAG = "JsonDataToChangeConverter";
        private static string _searchParameter = "{change:";

        private static Logger _logger;

        public JsonDataToChangeConverter()
        {
            _logger = new Logger(TAG);
        }

        public static IList<ChangeDto> GetList(string[] stringArray)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return ParseStringToList(stringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return ParseStringToList(usedEntry);
            }
        }

        private static IList<ChangeDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<ChangeDto> list = new List<ChangeDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 0; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            ChangeDto newValue = ParseStringToValue(index, data);
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

            return new List<ChangeDto>();
        }

        private static ChangeDto ParseStringToValue(int id, string[] data)
        {
            if (data.Length == 7)
            {
                if (data[0].Contains("{Type:")
                        && data[1].Contains("{Hour:")
                        && data[2].Contains("{Minute:")
                        && data[3].Contains("{Day:")
                        && data[4].Contains("{Month:")
                        && data[5].Contains("{Year:")
                        && data[6].Contains("{User:"))
                {

                    string type = data[0].Replace("{Type:", "").Replace("};", "");

                    string dayString = data[3].Replace("{Day:", "").Replace("};", "");
                    int day = -1;
                    bool parseSuccessDay = int.TryParse(dayString, out day);
                    if (!parseSuccessDay)
                    {
                        _logger.Error("Failed to parse day from data!");
                        return null;
                    }

                    string monthString = data[4].Replace("{Month:", "").Replace("};", "");
                    int month = -1;
                    bool parseSuccessMonth = int.TryParse(dayString, out month);
                    if (!parseSuccessMonth)
                    {
                        _logger.Error("Failed to parse month from data!");
                        return null;
                    }

                    string yearString = data[5].Replace("{Year:", "").Replace("};", "");
                    int year = -1;
                    bool parseSuccessYear = int.TryParse(dayString, out year);
                    if (!parseSuccessYear)
                    {
                        _logger.Error("Failed to parse year from data!");
                        return null;
                    }

                    string hourString = data[1].Replace("{Hour:", "").Replace("};", "");
                    int hour = -1;
                    bool parseSuccessHour = int.TryParse(hourString, out hour);
                    if (!parseSuccessHour)
                    {
                        _logger.Error("Failed to parse hour from data!");
                        return null;
                    }

                    string minuteString = data[2].Replace("{Minute:", "").Replace("};", "");
                    int minute = -1;
                    bool parseSuccessMinute = int.TryParse(minuteString, out minute);
                    if (!parseSuccessMinute)
                    {
                        _logger.Error("Failed to parse minute from data!");
                        return null;
                    }

                    DateTime dateTime = new DateTime(year, month, day, hour, minute, 0);

                    string user = data[6].Replace("{User:", "").Replace("};", "");

                    return new ChangeDto(id, type, dateTime, user);
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
