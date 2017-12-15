using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToListedMenuConverter : IJsonDataConverter<ListedMenuDto>
    {
        private const string TAG = "JsonDataToListedMenuConverter";
        private static string _searchParameter = "{\"Data\":";

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
                IList<ListedMenuDto> listedMenuList = new List<ListedMenuDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken listedMenuJsonData = child["ListedMenu"];

                    int id = int.Parse(listedMenuJsonData["ID"].ToString());

                    string title = listedMenuJsonData["Title"].ToString();
                    string description = listedMenuJsonData["Description"].ToString();

                    int rating = int.Parse(listedMenuJsonData["Rating"].ToString());
                    int useCounter = int.Parse(listedMenuJsonData["UseCounter"].ToString());

                    ListedMenuDto newListedMenu = new ListedMenuDto(id, title, description, rating, useCounter);
                    listedMenuList.Add(newListedMenu);
                }

                return listedMenuList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<ListedMenuDto>();
        }
    }
}
