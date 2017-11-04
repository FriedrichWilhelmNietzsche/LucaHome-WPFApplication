using Common.Converter;
using Common.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestProject
{
    [TestClass]
    public class JsonConverterTest
    {
        private const string TAG = "JsonConverterTest";

        [TestMethod]
        public void JsonDataToCoinConversionConverterTest()
        {
            string response = "{\"BTC\":{\"EUR\":1989.62},\"DASH\":{\"EUR\":144.12},\"ETC\":{\"EUR\":13.12},\"ETH\":{\"EUR\":185.94},\"LTC\":{\"EUR\":35.93},\"XMR\":{\"EUR\":31.17},\"ZEC\":{\"EUR\":174}}";

            JsonDataToCoinConversionConverter jsonDataToCoinConversionConverter = new JsonDataToCoinConversionConverter();
            IList<KeyValuePair<string, double>> conversionList = jsonDataToCoinConversionConverter.GetList(response);

            Assert.AreEqual(conversionList.Count, 7);
        }

        [TestMethod]
        public void JsonDataToCoinTrendConverterTest()
        {
            CoinDto coin = new CoinDto(1, "Jonas", "BTC", 0.25623, 5717.12, CoinDto.Trend.NULL);
            string response = "{\"Response\":\"Success\",\"Type\":100,\"Aggregated\":true,\"Data\":[{\"time\":1509526800,\"close\":5701.01,\"high\":5731.75,\"low\":5567.39,\"open\":5568.18,\"volumefrom\":5069.370000000001,\"volumeto\":28905115.34},{\"time\":1509537600,\"close\":5683.31,\"high\":5706.18,\"low\":5631.76,\"open\":5701.01,\"volumefrom\":3043.34,\"volumeto\":17265365.919999998},{\"time\":1509548400,\"close\":5737.8,\"high\":5759.6,\"low\":5671.74,\"open\":5683.31,\"volumefrom\":2867.68,\"volumeto\":16420272.52},{\"time\":1509559200,\"close\":5727.92,\"high\":5743.72,\"low\":5690.27,\"open\":5743.43,\"volumefrom\":1774.25,\"volumeto\":10127145.39},{\"time\":1509570000,\"close\":5870.89,\"high\":5871.48,\"low\":5717.72,\"open\":5727.23,\"volumefrom\":1840.0499999999997,\"volumeto\":10577935.32},{\"time\":1509580800,\"close\":5893.69,\"high\":5967.44,\"low\":5816.51,\"open\":5870.89,\"volumefrom\":1933.44,\"volumeto\":11355695.91},{\"time\":1509591600,\"close\":5931.23,\"high\":5947.51,\"low\":5885.83,\"open\":5897.27,\"volumefrom\":1056.1100000000001,\"volumeto\":6221662.460000001},{\"time\":1509602400,\"close\":6096.4,\"high\":6098.09,\"low\":5926.82,\"open\":5931.23,\"volumefrom\":3175.85,\"volumeto\":19039197.2},{\"time\":1509613200,\"close\":6039.48,\"high\":6426.61,\"low\":5779.65,\"open\":6093.49,\"volumefrom\":7850.2,\"volumeto\":47803459.3},{\"time\":1509624000,\"close\":6087.96,\"high\":6242.89,\"low\":5831.48,\"open\":6039.14,\"volumefrom\":4806.23,\"volumeto\":29064349.4},{\"time\":1509634800,\"close\":6080.03,\"high\":6165.98,\"low\":5945.54,\"open\":6090.11,\"volumefrom\":3146.4,\"volumeto\":19045486.759999998},{\"time\":1509645600,\"close\":6084.4,\"high\":6123.23,\"low\":5939.34,\"open\":6079.93,\"volumefrom\":2860.61,\"volumeto\":17153807.4},{\"time\":1509656400,\"close\":6077.35,\"high\":6124.34,\"low\":6062.22,\"open\":6084.4,\"volumefrom\":1538.7400000000002,\"volumeto\":9335896.53},{\"time\":1509667200,\"close\":6074.23,\"high\":6101.29,\"low\":6014.6,\"open\":6077.35,\"volumefrom\":1054.1399999999999,\"volumeto\":6332995.869999999},{\"time\":1509678000,\"close\":6227.98,\"high\":6229.75,\"low\":6073.99,\"open\":6074.3,\"volumefrom\":1258.25,\"volumeto\":7727202.23},{\"time\":1509688800,\"close\":6418.95,\"high\":6420.19,\"low\":6218.89,\"open\":6227.98,\"volumefrom\":2811.1400000000003,\"volumeto\":17611338.57},{\"time\":1509699600,\"close\":6335.81,\"high\":6447.05,\"low\":6287.77,\"open\":6418.95,\"volumefrom\":4276.21,\"volumeto\":27126568.7},{\"time\":1509710400,\"close\":6332.87,\"high\":6362.85,\"low\":6246.37,\"open\":6332.68,\"volumefrom\":3022.9300000000003,\"volumeto\":18933861.06},{\"time\":1509721200,\"close\":6360.81,\"high\":6392.17,\"low\":6322.82,\"open\":6332.87,\"volumefrom\":2151.13,\"volumeto\":13640346.049999999},{\"time\":1509732000,\"close\":6302.36,\"high\":6399.26,\"low\":6301.99,\"open\":6360.82,\"volumefrom\":1560.4499999999998,\"volumeto\":9881534.65},{\"time\":1509742800,\"close\":6248.98,\"high\":6311.22,\"low\":6187.69,\"open\":6303.14,\"volumefrom\":2538.9,\"volumeto\":15771730.52},{\"time\":1509753600,\"close\":6219.44,\"high\":6250.01,\"low\":6079.32,\"open\":6248.98,\"volumefrom\":1933.16,\"volumeto\":11848070.45},{\"time\":1509764400,\"close\":6217.52,\"high\":6283.66,\"low\":6202.47,\"open\":6219.44,\"volumefrom\":657.5999999999999,\"volumeto\":4081269.96},{\"time\":1509775200,\"close\":6218.61,\"high\":6275.6,\"low\":6176.74,\"open\":6218.12,\"volumefrom\":834.12,\"volumeto\":5177045.94},{\"time\":1509786000,\"close\":6205.33,\"high\":6243.29,\"low\":6171.01,\"open\":6218.61,\"volumefrom\":875.45,\"volumeto\":5411794.54}],\"TimeTo\":1509793200,\"TimeFrom\":1509526800,\"FirstValueInArray\":true,\"ConversionType\":{\"type\":\"direct\",\"conversionSymbol\":\"\"}}";

            JsonDataToCoinTrendConverter jsonDataToCoinTrendConverter = new JsonDataToCoinTrendConverter();
            coin = jsonDataToCoinTrendConverter.UpdateTrend(coin, response, "EUR");

            Assert.AreNotEqual(coin.CurrentTrend, CoinDto.Trend.NULL);
        }
    }
}
