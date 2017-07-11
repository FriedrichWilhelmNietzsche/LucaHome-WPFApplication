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
        public void WirelessSocketServiceEventRaisedTest()
        {
            IList<WirelessSocketDto> socketList = null;
            bool downloadSuccess = false;

            WirelessSocketService wirelessSocketService = WirelessSocketService.Instance;
            wirelessSocketService.OnWirelessSocketDownloadFinished += (list, success, response) =>
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
