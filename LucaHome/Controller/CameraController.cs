using Common.Tools;
using Data.CustomEventArgs;
using Data.Misc;
using System;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;

// Really helpful https://blogs.infosupport.com/writing-an-ip-camera-viewer-in-c-5-0/

namespace LucaHome.Controller
{
    public class CameraController
    {
        private const string TAG = "CameraController";
        private readonly Logger _logger;

        private string _url;
        private HttpClient _httpClient;
        private AutomaticMultiPartReader _automaticMultiPartReader;
        private BitmapImage _currentFrame;

        public CameraController()
        {
            _logger = new Logger(TAG);
        }

        public event EventHandler<ImageReadyEventArsgs> ImageReady;

        public void Initialize(string url)
        {
            _url = url;
            _logger.Debug(String.Format("Initialized with url {0}", _url));

            WebRequestHandler handler = new WebRequestHandler();

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(_url);
            _httpClient.Timeout = TimeSpan.FromMilliseconds(-1);
        }

        public async void StartProcessing()
        {
            try
            {
                HttpResponseMessage resultMessage = await _httpClient.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);

                //because of the configure await the rest of this method happens on a background thread.
                resultMessage.EnsureSuccessStatusCode();

                // check the response type
                if (!resultMessage.Content.Headers.ContentType.MediaType.Contains("multipart"))
                {
                    throw new ArgumentException("The camera did not return a mjpeg stream");
                }
                else
                {
                    _automaticMultiPartReader = new AutomaticMultiPartReader(new MultiPartStream(await resultMessage.Content.ReadAsStreamAsync()));
                    _automaticMultiPartReader.PartReady += _readerPartReady;
                    _automaticMultiPartReader.StartProcessing();
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
            }
        }

        public void StopProcessing()
        {
            if (_automaticMultiPartReader != null)
            {
                _automaticMultiPartReader.StopProcessing();
                _automaticMultiPartReader = null;
            }
        }

        protected void OnImageReady()
        {
            ImageReady?.Invoke(this, new ImageReadyEventArsgs() { Image = _currentFrame });
        }

        private void _readerPartReady(object sender, PartReadyEventArgs partReadyEventArgs)
        {
            //let's get this events back on the UI thread
            Stream frameStream = new MemoryStream(partReadyEventArgs.Part);

            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                _currentFrame = new BitmapImage();
                _currentFrame.BeginInit();
                _currentFrame.StreamSource = frameStream;
                _currentFrame.EndInit();

                OnImageReady();
            }));
        }
    }
}
