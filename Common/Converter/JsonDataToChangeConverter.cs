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

        private readonly Logger _logger;

        public JsonDataToChangeConverter()
        {
            _logger = new Logger(TAG);
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

                    DateTime dateTime = new DateTime(year, month, day);

                    ChangeDto newChange = new ChangeDto(id, type, dateTime, user);
                    changeList.Add(newChange);

                    id++;
                }

                return changeList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<ChangeDto>();
        }
    }
}
