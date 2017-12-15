using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToMenuConverter : IJsonDataConverter<MenuDto>
    {
        private const string TAG = "JsonDataToMenuConverter";
        private static string _searchParameter = "{\"Data\":";

        private readonly Logger _logger;

        public JsonDataToMenuConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<MenuDto> GetList(string[] stringArray)
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

        public IList<MenuDto> GetList(string jsonString)
        {
            return parseStringToList(jsonString);
        }

        private IList<MenuDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<MenuDto> menuList = new List<MenuDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                int id = 0;
                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken menuJsonData = child["Menu"];
                    
                    string title = menuJsonData["Title"].ToString();
                    string description = menuJsonData["Description"].ToString();

                    string weekday = menuJsonData["Weekday"].ToString();

                    JToken dateJsonData = menuJsonData["Date"];

                    int day = int.Parse(dateJsonData["Day"].ToString());
                    int month = int.Parse(dateJsonData["Month"].ToString());
                    int year = int.Parse(dateJsonData["Year"].ToString());

                    DateTime date = new DateTime(year, month, day);

                    MenuDto newMenu = new MenuDto(id, title, description, date);
                    menuList.Add(newMenu);

                    id++;
                }

                return menuList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<MenuDto>();
        }
    }
}
