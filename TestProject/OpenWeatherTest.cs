using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenWeather.Converter;
using OpenWeather.Downloader;
using OpenWeather.Models;

namespace TestProject
{
    [TestClass]
    public class OpenWeatherTest
    {
        private const string TAG = "OpenWeatherTest";

        [TestMethod]
        public void TestDownloadCurrentWeather()
        {
            string city = "Munich, DE";

            OpenWeatherDownloader downloader = new OpenWeatherDownloader(city);
            string currentWeatherJson = downloader.DownloadCurrentWeatherJson();

            Assert.AreNotEqual(currentWeatherJson, string.Empty);
        }

        [TestMethod]
        public void TestDownloadForecastWeather()
        {
            string city = "Munich, DE";

            OpenWeatherDownloader downloader = new OpenWeatherDownloader(city);
            string forecastWeatherJson = downloader.DownloadForecastWeatherJson();

            Assert.AreNotEqual(forecastWeatherJson, string.Empty);
        }

        [TestMethod]
        public void TestConvertJsonToCurrentWeather()
        {
            string city = "Munich, DE";

            OpenWeatherDownloader downloader = new OpenWeatherDownloader(city);
            string forecastWeatherJson = downloader.DownloadCurrentWeatherJson();

            JsonToWeatherConverter jsonToWeatherConverter = new JsonToWeatherConverter();
            WeatherModel currentWeather = jsonToWeatherConverter.ConvertFromJsonToCurrentWeather(forecastWeatherJson);

            Assert.AreNotEqual(currentWeather, null);
        }

        [TestMethod]
        public void TestConvertJsonToForecastWeather()
        {
            string city = "Munich, DE";

            OpenWeatherDownloader downloader = new OpenWeatherDownloader(city);
            string forecastWeatherJson = downloader.DownloadForecastWeatherJson();

            JsonToWeatherConverter jsonToWeatherConverter = new JsonToWeatherConverter();
            ForecastModel forecastWeather = jsonToWeatherConverter.ConvertFromJsonToForecastWeather(forecastWeatherJson);

            Assert.AreNotEqual(forecastWeather, null);
        }
    }
}
