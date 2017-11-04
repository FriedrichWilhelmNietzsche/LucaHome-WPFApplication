using Common.Tools;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Data.Controller
{
    public enum DownloadType
    {
        Birthday, BirthdayAdd, BirthdayUpdate, BirthdayDelete,
        CoinConversion, Coin, CoinAdd, CoinUpdate, CoinDelete, CoinTrend,
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

    public delegate void DownloadFinishedEventHandler(string response, bool success, DownloadType downloadType, object additionalData);

    public class DownloadController
    {
        private const string TAG = "DownloadController";
        private readonly Logger _logger;

        public DownloadController()
        {
            _logger = new Logger(TAG);
        }

        public event DownloadFinishedEventHandler OnDownloadFinished;
        private void publishOnDownloadFinished(string response, bool success, DownloadType downloadType, object additionalData)
        {
            OnDownloadFinished?.Invoke(response, success, downloadType, additionalData);
        }

        public async void SendCommandToWebsite(string requestUrl, DownloadType downloadType, object additional)
        {
            _logger.Debug("SendCommandToWebsite");
            try
            {
                await sendCommandToWebsiteAsync(requestUrl, downloadType, additional);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                publishOnDownloadFinished(exception.Message, false, downloadType, additional);
            }
        }

        public void SendCommandToWebsite(string requestUrl, DownloadType downloadType)
        {
            _logger.Debug("SendCommandToWebsite without additional");
            SendCommandToWebsite(requestUrl, downloadType, null);
        }

        private async Task sendCommandToWebsiteAsync(string requestUrl, DownloadType downloadType, object additional)
        {
            _logger.Debug("sendCommandToWebsiteAsync");

            if (requestUrl == null)
            {
                _logger.Error("RequestUrl may not be null!");
                publishOnDownloadFinished("ERROR: RequestUrl may not be null!", false, downloadType, additional);
                return;
            }

            if (requestUrl.Length < 15)
            {
                _logger.Error("Invalid requestUrl length!");
                publishOnDownloadFinished("ERROR: Invalid requestUrl length!", false, downloadType, additional);
                return;
            }

            string data = "";
            HttpClient httpClient = new HttpClient();

            httpClient.Timeout = TimeSpan.FromMilliseconds(3000);
            data = await httpClient.GetStringAsync(requestUrl);

            _logger.Debug(string.Format("ResponseData: {0}", data));

            publishOnDownloadFinished(data, (data != null), downloadType, additional);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");
        }
    }
}
