using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToBirthdayConverter : IJsonDataConverter<BirthdayDto>
    {
        private const string TAG = "JsonDataToBirthdayConverter";
        private static string _searchParameter = "{\"Data\":";

        private readonly Logger _logger;
        
        public JsonDataToBirthdayConverter()
        {
            _logger = new Logger(TAG);
        }

        public IList<BirthdayDto> GetList(string[] stringArray)
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

        public IList<BirthdayDto> GetList(string responseString)
        {
            return parseStringToList(responseString);
        }

        private IList<BirthdayDto> parseStringToList(string value)
        {
            if (!value.Contains("Error"))
            {
                IList<BirthdayDto> birthdayList = new List<BirthdayDto>();

                JObject jsonObject = JObject.Parse(value);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                foreach (JToken child in jsonObjectData.Children())
                {
                    JToken birthdayJsonData = child["Birthday"];

                    int id = int.Parse(birthdayJsonData["ID"].ToString());

                    string name = birthdayJsonData["Name"].ToString();

                    bool remindMe = birthdayJsonData["RemindMe"].ToString() == "1";
                    bool sendMail = birthdayJsonData["SendMail"].ToString() == "1";

                    JToken birthdayJsonDate = birthdayJsonData["Date"];

                    int day = int.Parse(birthdayJsonDate["Day"].ToString());
                    int month = int.Parse(birthdayJsonDate["Month"].ToString());
                    int year = int.Parse(birthdayJsonDate["Year"].ToString());

                    DateTime birthdayDate = new DateTime(year, month, day);

                    BirthdayDto newBirthday = new BirthdayDto(id, name, remindMe, sendMail, birthdayDate);
                    birthdayList.Add(newBirthday);
                }

                return birthdayList;
            }

            _logger.Error(string.Format("{0} has an error!", value));

            return new List<BirthdayDto>();
        }
    }
}
