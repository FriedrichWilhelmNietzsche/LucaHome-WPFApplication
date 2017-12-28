using Common.Dto;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Common.Converter
{
    public class JsonDataToCoinConverter
    {
        private const string TAG = "JsonDataToCoinConverter";
        private static string _searchParameter = "{\"Data\":";

        private static JsonDataToCoinConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToCoinConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToCoinConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToCoinConverter();
                    }

                    return _instance;
                }
            }
        }

        public IList<CoinDto> GetList(string[] stringArray, IList<KeyValuePair<string, double>> conversionList)
        {
            if (StringHelper.StringsAreEqual(stringArray))
            {
                return parseStringToList(stringArray[0], conversionList);
            }
            else
            {
                string usedEntry = StringHelper.SelectString(stringArray, _searchParameter);
                return parseStringToList(usedEntry, conversionList);
            }
        }

        public IList<CoinDto> GetList(string responseString, IList<KeyValuePair<string, double>> conversionList)
        {
            return parseStringToList(responseString, conversionList);
        }

        private IList<CoinDto> parseStringToList(string value, IList<KeyValuePair<string, double>> conversionList)
        {
            if (!value.Contains("Error"))
            {
                IList<CoinDto> coinList = new List<CoinDto>();

                try
                {
                    JObject jsonObject = JObject.Parse(value);
                    JToken jsonObjectData = jsonObject.GetValue("Data");

                    foreach (JToken child in jsonObjectData.Children())
                    {
                        JToken coinJsonData = child["Coin"];

                        int id = int.Parse(coinJsonData["Id"].ToString());

                        string user = coinJsonData["User"].ToString();
                        string type = coinJsonData["Type"].ToString();

                        double amount = double.Parse(coinJsonData["Amount"].ToString());

                        double currentConversion = 0;
                        foreach (KeyValuePair<string, double> entry in conversionList)
                        {
                            if (entry.Key.Contains(type))
                            {
                                currentConversion = entry.Value;
                                break;
                            }
                        }

                        CoinDto newCoin = new CoinDto(id, user, type, amount, currentConversion, CoinDto.Trend.NULL);
                        coinList.Add(newCoin);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error(TAG, exception.Message);
                }

                return coinList;
            }

            Logger.Instance.Error(TAG, string.Format("{0} has an error!", value));

            return new List<CoinDto>();
        }
    }
}
