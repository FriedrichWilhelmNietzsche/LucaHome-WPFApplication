using Common.Common;
using Common.Dto;
using Common.Enums;
using Data.Services;
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
            AppSettingsService appSettingsService = AppSettingsService.Instance;
            UserDto user = appSettingsService.User;

            string requestUrl = "http://" + appSettingsService.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SOCKETS.Action;
            
            HttpClient httpClient = new HttpClient();
            string data = await httpClient.GetStringAsync(requestUrl);

            Assert.IsNotNull(data);
        }
    }
}
