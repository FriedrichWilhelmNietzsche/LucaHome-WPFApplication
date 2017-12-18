using Common.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject
{
    [TestClass]
    public class CommonConverterTest
    {
        private const string TAG = "CommonConverterTest";

        [TestMethod]
        public void TestUnixToDateTimeConvert()
        {
            double unix = 1498845600;
            DateTime dateTime = new DateTime(2017, 6, 30, 20, 0, 0);

            DateTime convertedDateTime = UnixToDateTimeConverter.Instance.UnixTimeStampToDateTime(unix);

            Assert.AreEqual(convertedDateTime, dateTime);
        }

        [TestMethod]
        public void TestUnixStringToDateTimeConvert()
        {
            string unixString = "1498845600";
            DateTime dateTime = new DateTime(2017, 6, 30, 20, 0, 0);

            DateTime convertedDateTime = UnixToDateTimeConverter.Instance.UnixTimeStampToDateTime(unixString);

            Assert.AreEqual(convertedDateTime, dateTime);
        }
    }
}
