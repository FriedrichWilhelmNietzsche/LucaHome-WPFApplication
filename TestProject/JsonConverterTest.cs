using Common.Converter;
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
    }
}
