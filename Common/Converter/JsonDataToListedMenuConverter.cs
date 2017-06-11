using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToListedMenuConverter
    {
        private const string TAG = "JsonDataToListedMenuConverter";
        private static string _searchParameter = "{listedmenu:";

        private static Logger _logger;

        public JsonDataToListedMenuConverter()
        {
            _logger = new Logger(TAG);
        }

        public static IList<ListedMenuDto> GetList(string[] stringArray)
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

        private static IList<ListedMenuDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<ListedMenuDto> list = new List<ListedMenuDto>();

                        string[] entries = value.Split(new string[] { "\\" + _searchParameter }, StringSplitOptions.RemoveEmptyEntries);
                        for (int index = 0; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = replacedEntry.Split(new string[] { "\\};" }, StringSplitOptions.RemoveEmptyEntries);
                            ListedMenuDto newValue = ParseStringToValue(data);
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

        private static ListedMenuDto ParseStringToValue(string[] data)
        {
            if (data.Length == 4)
            {
                if (data[0].Contains("{id:")
                    && data[1].Contains("{description:")
                    && data[2].Contains("{rating:")
                    && data[3].Contains("{lastSuggestion:"))
                {
                    string idString = data[0].Replace("{id:", "").Replace("};", "");
                    int id = -1;
                    bool parseSuccessId = int.TryParse(idString, out id);
                    if (!parseSuccessId)
                    {
                        _logger.Error("Failed to parse id from data!");
                        return null;
                    }

                    string description = data[1].Replace("{description:", "").Replace("};", "");
                    if (description.Length == 0)
                    {
                        description = " ";
                    }

                    string ratingString = data[2].Replace("{rating:", "").Replace("};", "");
                    int rating = -1;
                    bool parseSuccessRating = int.TryParse(ratingString, out rating);
                    if (!parseSuccessRating)
                    {
                        _logger.Error("Failed to parse rating from data!");
                        return null;
                    }

                    string lastSuggestionString = data[3].Replace("{lastSuggestion:", "").Replace("};", "");
                    bool lastSuggestion = lastSuggestionString.Contains("1");

                    return new ListedMenuDto(id, description, rating, lastSuggestion);
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
