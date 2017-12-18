namespace Common.Tools
{
    public class Logger
    {
        public enum Level { DEBUG, INFORMATION, WARNING, ERROR };

        private bool _enabled;
        private Level _logLevel;

        private static Logger _instance = null;
        private static readonly object _padlock = new object();

        Logger()
        {
            _enabled = true;
            _logLevel = Level.DEBUG;
        }

        public static Logger Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }

                    return _instance;
                }
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

        public Level LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = value;
            }
        }

        public void Debug(string tag, string message)
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
            System.Diagnostics.Debug.WriteLine(string.Format("TAG: {0} | Message: {1}", tag, message));
        }

        public void Information(string tag, string message)
        {
            if (!_enabled)
            {
                return;
            }

            if (_logLevel > Level.INFORMATION)
            {
                return;
            }

            // TODO add write to file, colored output, etc.
            System.Diagnostics.Debug.WriteLine(string.Format("TAG: {0} | Message: {1}", tag, message));
        }

        public void Warning(string tag, string message)
        {
            if (!_enabled)
            {
                return;
            }

            if (_logLevel > Level.WARNING)
            {
                return;
            }

            // TODO add write to file, colored output, etc.
            System.Diagnostics.Debug.WriteLine(string.Format("TAG: {0} | Message: {1}", tag, message));
        }

        public void Error(string tag, string message)
        {
            if (!_enabled)
            {
                return;
            }

            if (_logLevel > Level.ERROR)
            {
                return;
            }

            // TODO add write to file, colored output, etc.
            System.Diagnostics.Debug.WriteLine(string.Format("TAG: {0} | Message: {1}", tag, message));
        }
    }
}
