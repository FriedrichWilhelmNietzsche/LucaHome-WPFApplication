using Common.Dto;
using Common.Tools;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToMovieConverter
    {
        private const string TAG = "JsonDataToMovieConverter";
        private static string _searchParameter = "{movie:";

        private static Logger _logger;
        
        public JsonDataToMovieConverter()
        {
            _logger = new Logger(TAG);
        }

        public static IList<MovieDto> GetList(string[] stringArray)
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

        private static IList<MovieDto> ParseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<MovieDto> list = new List<MovieDto>();
                        
                        string[] entries = value.Split(new string[] { "\\" + _searchParameter }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string entry in entries)
                        {
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = replacedEntry.Split(new string[] { "\\};" }, StringSplitOptions.RemoveEmptyEntries);
                            MovieDto newValue = ParseStringToValue(data);
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

        private static MovieDto ParseStringToValue(string[] data)
        {
            if (data.Length == 6)
            {
                if (data[0].Contains("{Title:") && data[1].Contains("{Genre:") && data[2].Contains("{Description:")
                        && data[3].Contains("{Rating:") && data[4].Contains("{Watched:") && data[5].Contains("{Sockets:"))
                {

                    string title = data[0].Replace("{Title:", "").Replace("};", "");
                    string genre = data[1].Replace("{Genre:", "").Replace("};", "");
                    string description = data[2].Replace("{Description:", "").Replace("};", "");

                    string ratingString = data[3].Replace("{Rating:", "").Replace("};", "");
                    int rating = -1;
                    bool parseSuccessRating = int.TryParse(ratingString, out rating);
                    if (!parseSuccessRating)
                    {
                        _logger.Warning("Failed to parse rating from data!");
                    }

                    string watchedString = data[4].Replace("{Watched:", "").Replace("};", "");
                    int watched = -1;
                    bool parseSuccessWatched = int.TryParse(watchedString, out watched);
                    if (!parseSuccessWatched)
                    {
                        _logger.Warning("Failed to parse watched from data!");
                    }

                    MovieDto newValue = new MovieDto(title, genre, description, rating, watched);
                    return newValue;
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
