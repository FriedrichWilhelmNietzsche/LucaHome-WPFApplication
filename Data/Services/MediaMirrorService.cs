using Common.Tools;
using Data.Controller;

namespace Data.Services
{
    public delegate void MediaMirrorEventHandler(bool success, string response);

    public class MediaMirrorService
    {
        private const string TAG = "MediaMirrorService";
        private readonly Logger _logger;

        private readonly SocketController _socketController;

        private static MediaMirrorService _instance = null;
        private static readonly object _padlock = new object();

        MediaMirrorService()
        {
            _logger = new Logger(TAG);

            _socketController = new SocketController();

            _socketController.OnSocketFinished += _mediaMirrorDownloadFinished;
        }

        public event MediaMirrorEventHandler OnMediaMirrorFinished;
        private void publishOnMediaMirrorFinished(bool success, string response)
        {
            OnMediaMirrorFinished?.Invoke(success, response);
        }

        public static MediaMirrorService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MediaMirrorService();
                    }

                    return _instance;
                }
            }
        }

        public void GetMediaMirrorDto()
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

        private void _mediaMirrorDownloadFinished(string response, bool success)
        {
            _logger.Debug("_mediaMirrorDownloadFinished");

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnMediaMirrorFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnMediaMirrorFinished(false, response);
                return;
            }

            // TODO

            publishOnMediaMirrorFinished(true, response);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _socketController.OnSocketFinished -= _mediaMirrorDownloadFinished;

            _socketController.Dispose();
        }
    }
}
