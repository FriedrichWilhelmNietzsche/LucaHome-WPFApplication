using Common.Dto;
using Common.Tools;
using Newtonsoft.Json.Linq;

namespace Common.Converter
{
    public class JsonDataToCoinTrendConverter
    {
        private const string TAG = "JsonDataToCoinTrendConverter";
        private readonly Logger _logger;

        public JsonDataToCoinTrendConverter()
        {
            _logger = new Logger(TAG);
        }

        public CoinDto UpdateTrend(CoinDto coin, string responseString, string currency)
        {
            if (responseString.Length <= 2)
            {
                return coin;
            }

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

            return coin;
        }
    }
}
