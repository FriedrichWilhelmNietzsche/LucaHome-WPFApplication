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
        ListedMenu, ListedMenuAdd, ListedMenuUpdate, ListedMenuDelete,
        Menu, MenuUpdate, MenuClear,
        Movie, MovieUpdate,
        Schedule, ScheduleSet, ScheduleAdd, ScheduleUpdate, ScheduleDelete,
        Security, SecurityCamera, SecurityCameraControl,
        ShoppingList, ShoppingListAdd, ShoppingListDelete, ShoppingListUpdate,
        Temperature,
        TimerAdd, TimerUpdate, TimerDelete,
        User,
        WirelessSocket, WirelessSocketSet, WirelessSocketAdd, WirelessSocketUpdate, WirelessSocketDelete,
        WirelessSwitch, WirelessSwitchToggle, WirelessSwitchAdd, WirelessSwitchUpdate, WirelessSwitchDelete
    };

    public delegate void DownloadFinishedEventHandler(string response, bool success, DownloadType downloadType, object additionalData);

    public class DownloadController
    {
        private const string TAG = "DownloadController";

        public DownloadController()
        {
            // Empty constructor, nothing needed here
        }

        public event DownloadFinishedEventHandler OnDownloadFinished;
        private void publishOnDownloadFinished(string response, bool success, DownloadType downloadType, object additionalData)
        {
            OnDownloadFinished?.Invoke(response, success, downloadType, additionalData);
        }

        public async void SendCommandToWebsite(string requestUrl, DownloadType downloadType, object additional)
        {
            try
            {
                await sendCommandToWebsiteAsync(requestUrl, downloadType, additional);
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(TAG, exception.Message);
                publishOnDownloadFinished(exception.Message, false, downloadType, additional);
            }
        }

        public void SendCommandToWebsite(string requestUrl, DownloadType downloadType)
        {
            SendCommandToWebsite(requestUrl, downloadType, null);
        }

        private async Task sendCommandToWebsiteAsync(string requestUrl, DownloadType downloadType, object additional)
        {
            if (requestUrl == null)
            {
                Logger.Instance.Error(TAG, "RequestUrl may not be null!");
                publishOnDownloadFinished("ERROR: RequestUrl may not be null!", false, downloadType, additional);
                return;
            }

            if (requestUrl.Length < 15)
            {
                Logger.Instance.Error(TAG, "Invalid requestUrl length!");
                publishOnDownloadFinished("ERROR: Invalid requestUrl length!", false, downloadType, additional);
                return;
            }

            string data = "";
            HttpClient httpClient = new HttpClient();

            httpClient.Timeout = TimeSpan.FromMilliseconds(3000);
            data = await httpClient.GetStringAsync(requestUrl);

            publishOnDownloadFinished(data, (data != null), downloadType, additional);
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");
        }
    }
}
