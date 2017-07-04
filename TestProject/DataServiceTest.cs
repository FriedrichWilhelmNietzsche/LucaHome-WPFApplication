using Common.Dto;
using Data.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class DataServiceTest
    {
        private const string TAG = "DataServiceTest";

        [TestMethod]
        public void AppSettingsServiceEnteredDataTest()
        {
            UserDto newTestUser = new UserDto("NA", "NA!");

            AppSettingsService appSettingsService = new AppSettingsService();
            appSettingsService.User = newTestUser;
            bool enteredUserData = appSettingsService.EnteredUserData;

            Assert.IsFalse(enteredUserData);
        }

        [TestMethod]
        public void AppSettingsServiceLoadTest()
        {
            AppSettingsService appSettingsService = new AppSettingsService();
            string openWeatherCity = appSettingsService.OpenWeatherCity;

            Assert.AreEqual(openWeatherCity, "Munich, DE");
        }

        [TestMethod]
        public void AppSettingsServiceSaveTest()
        {
            UserDto newTestUser = new UserDto("Jonas Schubert", "Passw0rt!");

            AppSettingsService appSettingsService = new AppSettingsService();
            appSettingsService.User = newTestUser;

            UserDto loadedUser = appSettingsService.User;

            Assert.AreEqual(newTestUser.ToString(), loadedUser.ToString());
        }
    }
}
