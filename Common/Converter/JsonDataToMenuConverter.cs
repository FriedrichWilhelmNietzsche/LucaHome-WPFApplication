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

        private static JsonDataToMenuConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToMenuConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToMenuConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToMenuConverter();
                    }

                    return _instance;
                }
            }
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

                try
                {
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
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return menuList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<MenuDto>();
        }
    }
}
