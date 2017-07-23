using Common.Dto;
using Common.Tools;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Converter
{
    public class JsonDataToMovieConverter
    {
        private const string TAG = "JsonDataToMovieConverter";
        private static string _searchParameter = "{movie:";

        private readonly Logger _logger;

        public JsonDataToMovieConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<MovieDto> GetList(string[] stringArray)
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

        public IList<MovieDto> GetList(string jsonString)
        {
            return parseStringToList(jsonString);
        }

        private IList<MovieDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                if (StringHelper.GetStringCount(value, _searchParameter) > 1)
                {
                    if (value.Contains(_searchParameter))
                    {
                        IList<MovieDto> list = new List<MovieDto>();

                        string[] entries = Regex.Split(value, "\\" + _searchParameter);
                        for (int index = 0; index < entries.Length; index++)
                        {
                            string entry = entries[index];
                            string replacedEntry = entry.Replace(_searchParameter, "").Replace("};};", "");

                            string[] data = Regex.Split(replacedEntry, "\\};");
                            MovieDto newValue = parseStringToValue(index, data);
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

            return new List<MovieDto>();
        }

        private MovieDto parseStringToValue(int id, string[] data)
        {
            if (data.Length == 5)
            {
                if (data[0].Contains("{Title:")
                    && data[1].Contains("{Genre:")
                    && data[2].Contains("{Description:")
                    && data[3].Contains("{Rating:")
                    && data[4].Contains("{Watched:"))
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

                    MovieDto newValue = new MovieDto(id, title, genre, description, rating, watched);
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
