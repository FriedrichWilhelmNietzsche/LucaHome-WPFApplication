using Common.Dto;
using Data.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class DataControllerTest
    {
        private const string TAG = "DataControllerTest";

        [TestMethod]
        public void AppSettingsControllerLoadTest()
        {
            AppSettingsController appSettingsController = AppSettingsController.Instance;
            string openWeatherCity = appSettingsController.OpenWeatherCity;

            Assert.AreEqual(openWeatherCity, "Munich, DE");
        }

        [TestMethod]
        public void AppSettingsControllerSaveTest()
        {
            UserDto newTestUser = new UserDto("Jonas Schubert", "Passw0rt!");

            AppSettingsController appSettingsController = AppSettingsController.Instance;
            appSettingsController.User = newTestUser;

            UserDto loadedUser = appSettingsController.User;

            Assert.AreEqual(newTestUser.ToString(), loadedUser.ToString());
        }
    }
}
