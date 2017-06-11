using Common.Common;

namespace Common.Tools
{
    public class Logger
    {
        private const string TAG = "Logger";

        enum Level { DEBUG, INFORMATION, WARNING, ERROR};

        private string _tag;
        private bool _enabled;
        private int _logLevel;

        public Logger(string tag, bool enabled, int logLevel)
        {
            _tag = tag;
            _enabled = enabled;
            _logLevel = logLevel;
        }

        public Logger(string tag, bool enabled) : this(tag, enabled, (int)Level.DEBUG)
        {
        }

        public Logger(string tag) : this(tag, Enables.LOGGING, (int)Level.DEBUG)
        {
        }

        public Logger() : this(TAG, Enables.LOGGING, (int)Level.DEBUG)
        {
        }

        public string Tag
        {
            get
            {
                return _tag;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public int LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = (int)value;
            }
        }

        public void Debug(string message)
        {
            if (!_enabled)
            {
                return;
            }

            if (_logLevel > (int)Level.DEBUG)
            {
                return;
            }

            // TODO add write to file, colored output, etc.
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Information(string message)
        {
            if (!_enabled)
            {
                return;
            }

            if(_logLevel > (int)Level.INFORMATION)
            {
                return;
            }

            // TODO add write to file, colored output, etc.
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Warning(string message)
        {
            if (!_enabled)
            {
                return;
            }

            if (_logLevel > (int)Level.WARNING)
            {
                return;
            }

            // TODO add write to file, colored output, etc.
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string message)
        {
            if (!_enabled)
            {
                return;
            }

            if (_logLevel > (int)Level.ERROR)
            {
                return;
            }

            // TODO add write to file, colored output, etc.
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
