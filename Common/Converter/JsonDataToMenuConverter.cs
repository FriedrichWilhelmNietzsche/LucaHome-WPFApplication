using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToMenuConverter
    {
        private const string TAG = "JsonDataToMenuConverter";
        private static string _searchParameter = "{menu:";

        private static Logger _logger;

        public JsonDataToMenuConverter()
        {
            _logger = new Logger(TAG);
        }

        public static IList<MenuDto> GetList(string[] stringArray)
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

        private static IList<MenuDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<MenuDto> list = new List<MenuDto>();

                        string[] entries = value.Split(new string[] { "\\" + _searchParameter }, StringSplitOptions.RemoveEmptyEntries);
                        for (int index = 0; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = replacedEntry.Split(new string[] { "\\};" }, StringSplitOptions.RemoveEmptyEntries);
                            MenuDto newValue = ParseStringToValue(index, data);
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

            return null;
        }

        private static MenuDto ParseStringToValue(int id, string[] data)
        {
            if (data.Length == 6)
            {
                if (data[0].Contains("{weekday:")
                    && data[1].Contains("{day:")
                    && data[2].Contains("{month:")
                    && data[3].Contains("{year:")
                    && data[4].Contains("{title:")
                    && data[5].Contains("{description:"))
                {

                    string weekday = data[0].Replace("{weekday:", "").Replace("};", "");

                    string dayString = data[1].Replace("{day:", "").Replace("};", "");
                    int day = -1;
                    bool parseSuccessDay = int.TryParse(dayString, out day);
                    if (!parseSuccessDay)
                    {
                        _logger.Error("Failed to parse day from data!");
                        return null;
                    }

                    string monthString = data[2].Replace("{month:", "").Replace("};", "");
                    int month = -1;
                    bool parseSuccessMonth = int.TryParse(dayString, out month);
                    if (!parseSuccessMonth)
                    {
                        _logger.Error("Failed to parse month from data!");
                        return null;
                    }

                    string yearString = data[3].Replace("{year:", "").Replace("};", "");
                    int year = -1;
                    bool parseSuccessYear = int.TryParse(dayString, out year);
                    if (!parseSuccessYear)
                    {
                        _logger.Error("Failed to parse year from data!");
                        return null;
                    }

                    DateTime date = new DateTime(year, month, day);

                    string title = data[4].Replace("{title:", "").Replace("};", "");

                    string description = data[5].Replace("{description:", "").Replace("};", "");
                    if (description.Length == 0)
                    {
                        description = " ";
                    }

                    return new MenuDto(id, title, description, date);
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
