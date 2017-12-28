using Common.Dto;
using Common.Tools;
using Newtonsoft.Json.Linq;
using System;

namespace Common.Converter
{
    public class JsonDataToCoinTrendConverter
    {
        private const string TAG = "JsonDataToCoinTrendConverter";

        private static JsonDataToCoinTrendConverter _instance = null;
        private static readonly object _padlock = new object();

        JsonDataToCoinTrendConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static JsonDataToCoinTrendConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonDataToCoinTrendConverter();
                    }

                    return _instance;
                }
            }
        }

        public CoinDto UpdateTrend(CoinDto coin, string responseString, string currency)
        {
            if (responseString.Length <= 2)
            {
                return coin;
            }

            try
            {
                JObject jsonObject = JObject.Parse(responseString);
                JToken jsonObjectData = jsonObject.GetValue("Data");

                JToken firstEntry = jsonObjectData.First;
                double firstEntryOpen = double.Parse(firstEntry["open"].ToString());

                JToken lastEntry = jsonObjectData.Last;
                double lastEntryClose = double.Parse(lastEntry["close"].ToString());

                if (lastEntryClose - firstEntryOpen > 0)
                {
                    coin.CurrentTrend = CoinDto.Trend.RISE;
                }
                else if (lastEntryClose - firstEntryOpen < 0)
                {
                    coin.CurrentTrend = CoinDto.Trend.FALL;
                }
                else
                {
                    coin.CurrentTrend = CoinDto.Trend.NULL;
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(TAG, exception.Message);
            }

            return coin;
        }
    }
}
