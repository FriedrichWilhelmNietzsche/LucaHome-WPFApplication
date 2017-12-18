using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToMovieConverter : IJsonDataConverter<MovieDto>
    {
        private const string TAG = "JsonDataToMovieConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToMovieConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToMovieConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToMovieConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToMovieConverter();
                    }

                    return _instance;
                }
            }
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
                IList<MovieDto> movieList = new List<MovieDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                int id = 0;
                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken movieJsonData = child["Movie"];

                    string title = movieJsonData["Title"].ToString();
                    string genre = movieJsonData["Genre"].ToString();
                    string description = movieJsonData["Description"].ToString();

                    int rating = int.Parse(movieJsonData["Rating"].ToString());
                    int watched = int.Parse(movieJsonData["Watched"].ToString());

                    MovieDto newMovie = new MovieDto(id, title, genre, description, rating, watched);
                    movieList.Add(newMovie);

                    id++;
                }

                return movieList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<MovieDto>();
        }
    }
}
