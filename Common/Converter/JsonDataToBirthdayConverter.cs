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

        private static JsonDataToBirthdayConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToBirthdayConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToBirthdayConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToBirthdayConverter();
                    }

                    return _instance;
                }
            }
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

                try
                {
                    JObject jsonObject = JObject.Parse(value);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken birthdayJsonData = child["Birthday"];

                        int id = int.Parse(birthdayJsonData["Id"].ToString());

                        string name = birthdayJsonData["Name"].ToString();
                        string group = birthdayJsonData["Group"].ToString();

                        bool remindMe = birthdayJsonData["RemindMe"].ToString() == "1";
                        bool sentMail = birthdayJsonData["SentMail"].ToString() == "1";

                        JToken birthdayJsonDate = birthdayJsonData["Date"];

                        int day = int.Parse(birthdayJsonDate["Day"].ToString());
                        int month = int.Parse(birthdayJsonDate["Month"].ToString());
                        int year = int.Parse(birthdayJsonDate["Year"].ToString());

                        DateTime birthdayDate = new DateTime(year, month, day);

                        BirthdayDto newBirthday = new BirthdayDto(id, name, birthdayDate, group, remindMe, sentMail);
                        birthdayList.Add(newBirthday);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return birthdayList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<BirthdayDto>();
        }
    }
}
