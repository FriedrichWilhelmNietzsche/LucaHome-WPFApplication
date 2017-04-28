using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToBirthdayConverter
    {
        private const string TAG = "JsonDataToBirthdayConverter";
        private static string _searchParameter = "{birthday:";

        private static Logger _logger;
        
        public JsonDataToBirthdayConverter()
        {
            _logger = new Logger(TAG);
        }

        public static IList<BirthdayDto> GetList(string[] stringArray)
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

        private static IList<BirthdayDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<BirthdayDto> list = new List<BirthdayDto>();
                        
                        string[] entries = value.Split(new string[] { "\\" + _searchParameter }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string entry in entries)
                        {
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = replacedEntry.Split(new string[] { "\\};" }, StringSplitOptions.RemoveEmptyEntries);
                            BirthdayDto newValue = ParseStringToValue(data);
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

        private static BirthdayDto ParseStringToValue(string[] data)
        {
            if (data.Length == 5)
            {
                if (data[0].Contains("{id:") && data[1].Contains("{name:") && data[2].Contains("{day:")
                        && data[3].Contains("{month:") && data[4].Contains("{year:"))
                {

                    string idString = data[0].Replace("{id:", "").Replace("};", "");
                    int id = -1;
                    bool parseSuccessId = int.TryParse(idString, out id);
                    if (!parseSuccessId)
                    {
                        _logger.Warning("Failed to parse id from data!");
                    }

                    string name = data[1].Replace("{name:", "").Replace("};", "");

                    string dayString = data[2].Replace("{day:", "").Replace("};", "");
                    int day = -1;
                    bool parseSuccessDay = int.TryParse(dayString, out day);
                    if (!parseSuccessDay)
                    {
                        _logger.Error("Failed to parse day from data!");
                        return null;
                    }

                    string monthString = data[3].Replace("{month:", "").Replace("};", "");
                    int month = -1;
                    bool parseSuccessMonth = int.TryParse(dayString, out month);
                    if (!parseSuccessMonth)
                    {
                        _logger.Error("Failed to parse month from data!");
                        return null;
                    }

                    string yearString = data[4].Replace("{year:", "").Replace("};", "");
                    int year = -1;
                    bool parseSuccessYear = int.TryParse(dayString, out year);
                    if (!parseSuccessYear)
                    {
                        _logger.Error("Failed to parse year from data!");
                        return null;
                    }

                    DateTime birthday = new DateTime(year, month, day);
                    
                    return new BirthdayDto(name, birthday);
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
