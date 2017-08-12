using Common.Tools;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Data.Controller
{
    public enum DownloadType
    {
        Birthday, BirthdayAdd, BirthdayUpdate, BirthdayDelete,
        CoinConversion, Coin, CoinAdd, CoinUpdate, CoinDelete,
        MapContent,
        ListedMenu,
        Menu, MenuUpdate, MenuClear,
        Movie, MovieUpdate,
        Schedule, ScheduleSet, ScheduleAdd, ScheduleUpdate, ScheduleDelete,
        Security, SecurityCamera, SecurityCameraControl,
        ShoppingList, ShoppingListAdd, ShoppingListDelete, ShoppingListUpdate,
        Temperature,
        TimerAdd, TimerUpdate, TimerDelete,
        User,
        WirelessSocket, WirelessSocketSet, WirelessSocketAdd, WirelessSocketUpdate, WirelessSocketDelete
    };

    public delegate void DownloadFinishedEventHandler(string response, bool success, DownloadType downloadType);

    public class DownloadController
    {
        private const string TAG = "DownloadController";
        private readonly Logger _logger;

        public DownloadController()
        {
            _logger = new Logger(TAG);
        }

        public event DownloadFinishedEventHandler OnDownloadFinished;

        public async void SendCommandToWebsite(string requestUrl, DownloadType downloadType)
        {
            _logger.Debug("SendCommandToWebsite");
            try
            {
                await sendCommandToWebsiteAsync(requestUrl, downloadType);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                OnDownloadFinished(exception.Message, false, downloadType);
            }
        }

        private async Task sendCommandToWebsiteAsync(string requestUrl, DownloadType downloadType)
        {
            _logger.Debug("sendCommandToWebsiteAsync");

            if (requestUrl == null)
            {
                _logger.Error("RequestUrl may not be null!");
                OnDownloadFinished("ERROR: RequestUrl may not be null!", false, downloadType);
                return;
            }

            if (requestUrl.Length < 15)
            {
                _logger.Error("Invalid requestUrl length!");
                OnDownloadFinished("ERROR: Invalid requestUrl length!", false, downloadType);
                return;
            }

            string data = "";
            HttpClient httpClient = new HttpClient();

            httpClient.Timeout = TimeSpan.FromMilliseconds(3000);
            data = await httpClient.GetStringAsync(requestUrl);

            _logger.Debug(string.Format("ResponseData: {0}", data));

            OnDownloadFinished(data, (data != null), downloadType);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");
        }
    }
}
