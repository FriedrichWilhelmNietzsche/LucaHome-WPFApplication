using Data.CustomEventArgs;
using System;

namespace Data.Misc
{
    public class AutomaticMultiPartReader
    {
        private MultiPartStream _multiPartStream;
        private bool _reading = false;
        private byte[] _currentPart;

        public AutomaticMultiPartReader(MultiPartStream multiPartStream)
        {
            _multiPartStream = multiPartStream;
        }

        public event EventHandler<PartReadyEventArgs> PartReady;

        public async void StartProcessing()
        {
            _reading = true;
            while (_reading)
            {
                _currentPart = await _multiPartStream.NextPartAsync().ConfigureAwait(false);
                OnPartReady();
            }
        }

        public void StopProcessing()
        {
            _multiPartStream.Close();
            _reading = false;
        }

        protected virtual void OnPartReady()
        {
            if (PartReady != null)
            {
                PartReadyEventArgs args = new PartReadyEventArgs();
                args.Part = _currentPart;
                PartReady(this, args);
            }
        }
    }
}
