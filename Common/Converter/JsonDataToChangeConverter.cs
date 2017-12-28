using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToChangeConverter : IJsonDataConverter<ChangeDto>
    {
        private const string TAG = "JsonDataToChangeConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToChangeConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToChangeConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToChangeConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToChangeConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<ChangeDto> GetList(string[] stringArray)
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

        public IList<ChangeDto> GetList(string responseString)
        {
            return parseStringToList(responseString);
        }

        private IList<ChangeDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<ChangeDto> changeList = new List<ChangeDto>();

                try
                {
                    JObject jsonObject = JObject.Parse(value);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    int id = 0;
                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken changeJsonData = child["Change"];

                        string type = changeJsonData["Type"].ToString();
                        string user = changeJsonData["UserName"].ToString();

                        JToken changeJsonDate = changeJsonData["Date"];

                        int day = int.Parse(changeJsonDate["Day"].ToString());
                        int month = int.Parse(changeJsonDate["Month"].ToString());
                        int year = int.Parse(changeJsonDate["Year"].ToString());

                        JToken changeJsonTime = changeJsonData["Time"];

                        int hour = int.Parse(changeJsonTime["Hour"].ToString());
                        int minute = int.Parse(changeJsonTime["Minute"].ToString());

                        DateTime dateTime = new DateTime(year, month, day, hour, minute, 0);

                        ChangeDto newChange = new ChangeDto(id, type, dateTime, user);
                        changeList.Add(newChange);

                        id++;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return changeList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<ChangeDto>();
        }
    }
}
