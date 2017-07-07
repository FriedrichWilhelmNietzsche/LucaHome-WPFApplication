using Common.Dto;
using Data.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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

            AppSettingsService appSettingsService = AppSettingsService.Instance;
            appSettingsService.User = newTestUser;
            bool enteredUserData = appSettingsService.EnteredUserData;

            Assert.IsFalse(enteredUserData);
        }

        [TestMethod]
        public void AppSettingsServiceLoadTest()
        {
            AppSettingsService appSettingsService = AppSettingsService.Instance;
            string openWeatherCity = appSettingsService.OpenWeatherCity;

            Assert.AreEqual(openWeatherCity, "Munich, DE");
        }

        [TestMethod]
        public void AppSettingsServiceSaveTest()
        {
            UserDto newTestUser = new UserDto("Jonas Schubert", "Passw0rt!");

            AppSettingsService appSettingsService = AppSettingsService.Instance;
            appSettingsService.User = newTestUser;

            UserDto loadedUser = appSettingsService.User;

            Assert.AreEqual(newTestUser.ToString(), loadedUser.ToString());
        }

        [TestMethod]
        public void WirelessSocketServiceEventRaisedTest()
        {
            IList<WirelessSocketDto> socketList = null;
            bool downloadSuccess = false;

            WirelessSocketService wirelessSocketService = WirelessSocketService.Instance;
            wirelessSocketService.OnWirelessSocketDownloadFinished += (list, success) =>
            {
                socketList = list;
                downloadSuccess = success;
            };
            wirelessSocketService.LoadWirelessSocketList();

            Assert.IsNotNull(socketList);
            Assert.IsTrue(downloadSuccess);
        }
    }
}
