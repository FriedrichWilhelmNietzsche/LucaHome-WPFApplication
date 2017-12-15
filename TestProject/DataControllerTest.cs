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
            SettingsController settingsController = SettingsController.Instance;
            string openWeatherCity = settingsController.OpenWeatherCity;

            Assert.AreEqual(openWeatherCity, "Nuremberg");
        }

        [TestMethod]
        public void AppSettingsControllerSaveTest()
        {
            UserDto newTestUser = new UserDto("Jonas Schubert", "Passw0rt!");

            SettingsController settingsController = SettingsController.Instance;
            settingsController.User = newTestUser;

            UserDto loadedUser = settingsController.User;

            Assert.AreEqual(newTestUser.ToString(), loadedUser.ToString());
        }
    }
}
