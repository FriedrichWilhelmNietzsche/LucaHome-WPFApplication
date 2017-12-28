using Common.Dto;
using Common.Interfaces;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToMoneyMeterDataConverter : IJsonDataConverter<MoneyMeterDataDto>
    {
        private const string TAG = "JsonDataToMoneyMeterDataConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToMoneyMeterDataConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToMoneyMeterDataConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToMoneyMeterDataConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToMoneyMeterDataConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<MoneyMeterDataDto> GetList(string[] stringArray)
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

        public IList<MoneyMeterDataDto> GetList(string responseString)
        {
            return parseStringToList(responseString);
        }

        private IList<MoneyMeterDataDto> parseStringToList(string jsonString)
        {
            if (!jsonString.Contains("Error"))
            {
                IList<MoneyMeterDataDto> moneyMeterDataList = new List<MoneyMeterDataDto>();

                try
                {
                    JObject jsonObject = JObject.Parse(jsonString);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken moneyMeterDataJsonData = child["MoneyMeterData"];

                        int id = int.Parse(moneyMeterDataJsonData["Id"].ToString());
                        int typeId = int.Parse(moneyMeterDataJsonData["TypeId"].ToString());

                        string bank = moneyMeterDataJsonData["Bank"].ToString();
                        string plan = moneyMeterDataJsonData["Plan"].ToString();
                        double amount = double.Parse(moneyMeterDataJsonData["Amount"].ToString());
                        string unit = moneyMeterDataJsonData["Unit"].ToString();

                        JToken moneyMeterDataJsonDate = moneyMeterDataJsonData["Date"];

                        int day = int.Parse(moneyMeterDataJsonDate["Day"].ToString());
                        int month = int.Parse(moneyMeterDataJsonDate["Month"].ToString());
                        int year = int.Parse(moneyMeterDataJsonDate["Year"].ToString());

                        DateTime saveDate = new DateTime(year, month, day);

                        string user = moneyMeterDataJsonData["User"].ToString();

                        MoneyMeterDataDto newMoneyMeterData = new MoneyMeterDataDto(id, typeId, bank, plan, amount, unit, saveDate, user);
                        moneyMeterDataList.Add(newMoneyMeterData);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return moneyMeterDataList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", jsonString));

            return new List<MoneyMeterDataDto>();
        }
    }
}
