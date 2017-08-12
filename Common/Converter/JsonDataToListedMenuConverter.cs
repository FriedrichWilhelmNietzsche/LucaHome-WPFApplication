﻿using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToListedMenuConverter : IJsonDataConverter<ListedMenuDto>
    {
        private const string TAG = "JsonDataToListedMenuConverter";
        private static string _searchParameter = "{listedmenu:";

        private readonly Logger _logger;

        public JsonDataToListedMenuConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<ListedMenuDto> GetList(string[] stringArray)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0]);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(usedEntry);
            }
        }

        public IList<ListedMenuDto> GetList(string jsonString)
        {
            return parseStringToList(jsonString);
        }

        private IList<ListedMenuDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<ListedMenuDto> list = new List<ListedMenuDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 0; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            ListedMenuDto newValue = parseStringToValue(data);
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

            return new List<ListedMenuDto>();
        }

        private ListedMenuDto parseStringToValue(string[] data)
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
