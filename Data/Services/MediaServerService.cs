using Common.Tools;
using Data.Controller;

namespace Data.Services
{
    public delegate void MediaServerEventHandler(bool success, string response);

    public class MediaServerService
    {
        private const string TAG = "MediaServerService";

        private readonly SocketController _socketController;

        private static MediaServerService _instance = null;
        private static readonly object _padlock = new object();

        MediaServerService()
        {
            _socketController = new SocketController();
            _socketController.OnSocketFinished += _mediaServerDownloadFinished;
        }

        public event MediaServerEventHandler OnMediaServerFinished;
        private void publishOnMediaServerFinished(bool success, string response)
        {
            OnMediaServerFinished?.Invoke(success, response);
        }

        public static MediaServerService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MediaServerService();
                    }

                    return _instance;
                }
            }
        }

        public void GetMediaServerDto()
        {

        }

        public void GetSavedYoutubeIDs()
        {

        }

        public void GetBatteryLevel()
        {

        }

        public void GetServerVersion()
        {

        }

        public void IncreaseVolume()
        {

        }

        public void DecreaseVolume()
        {

        }

        public void MuteVolume()
        {

        }

        public void UnmuteVolume()
        {

        }

        public void GetCurrentVolume()
        {

        }

        public void IncreaseBrightness()
        {

        }

        public void DecreaseBrightness()
        {

        }

        public void GetCurrentBrightness()
        {

        }

        public void ShowYoutubeVideo(string youtubeId)
        {

        }

        public void PlayYoutubeVideo()
        {

        }

        public void PauseYoutubeVideo()
        {

        }

        public void StopYoutubeVideo()
        {

        }

        public void SetYoutubeVideoPlayPosition(int playPosition)
        {

        }

        public void PlaySeaSound()
        {

        }

        public void StopSeaSound()
        {

        }

        public void IsSeaSoundPlaying()
        {

        }

        public void GetSeaSoundCountdown()
        {

        }

        public void ShowCenterText(string centerText)
        {

        }

        public void SetRssFeed(string rssFeed)
        {

        }

        public void ShowRadioStream(string radioStream)
        {

        }

        public void PlayRadioStream()
        {

        }

        public void StopRadioStream()
        {

        }

        private void _mediaServerDownloadFinished(string response, bool success)
        {
            Logger.Instance.Debug(TAG, "_mediaServerDownloadFinished");

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);

                publishOnMediaServerFinished(false, response);
                return;
            }

            Logger.Instance.Debug(TAG, string.Format("response: {0}", response));

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");

                publishOnMediaServerFinished(false, response);
                return;
            }

            // TODO

            publishOnMediaServerFinished(true, response);
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");
            _socketController.OnSocketFinished -= _mediaServerDownloadFinished;
            _socketController.Dispose();
        }
    }
}
