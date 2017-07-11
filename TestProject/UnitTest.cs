using Common.Common;
using Common.Dto;
using Common.Enums;
using Data.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task TestHttpClientGetAsync()
        {
            AppSettingsController appSettingsController = AppSettingsController.Instance;
            UserDto user = appSettingsController.User;

            string requestUrl = "http://" + appSettingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SOCKETS.Action;

            HttpClient httpClient = new HttpClient();
            string data = await httpClient.GetStringAsync(requestUrl);

            Assert.IsNotNull(data);
        }
    }
}
