namespace Common.Dto
{
    public class MediaServerDto
    {
        private const string TAG = "MediaServerDto";

        private int _batteryLevel;

        private string _socketName;
        private bool _socketState;

        private int _volume;

        private string _youtubeId;
        private bool _youtubeIsPlaying;
        private int _youtubeVideoCurrentPlayTime;
        private int _youtubeVideoDuration;
        //private ArrayList<PlayedYoutubeVideo> _playedYoutubeIds;

        private bool _radioStreamIsPlaying;
        private int _radioStreamId;

        private bool _sleepTimerEnabled;
        private int _countDownSec;

        private string _serverVersion;

        private int _screenBrightness;

        public MediaServerDto(
            //MediaServerSelection mediaServerSelection,
            int batteryLevel,
            string socketName,
            bool socketState,
            int volume,
            string youtubeId,
            bool youtubeIsPlaying,
            int youtubeVideoCurrentPlayTime,
            int youtubeVideoDuration,
            //ArrayList<PlayedYoutubeVideo> playedYoutubeIds,
            bool radioStreamIsPlaying,
            int radioStreamId,
            bool sleepTimerEnabled,
            int countDownSec,
            string serverVersion,
            int screenBrightness)
        {
            //_mediaServerSelection = mediaServerSelection;

            _batteryLevel = batteryLevel;

            _socketName = socketName;
            _socketState = socketState;

            _volume = volume;

            _youtubeId = youtubeId;
            _youtubeIsPlaying = youtubeIsPlaying;
            _youtubeVideoCurrentPlayTime = youtubeVideoCurrentPlayTime;
            _youtubeVideoDuration = youtubeVideoDuration;
            //_playedYoutubeIds = playedYoutubeIds;

            _radioStreamIsPlaying = radioStreamIsPlaying;
            _radioStreamId = radioStreamId;

            _sleepTimerEnabled = sleepTimerEnabled;
            _countDownSec = countDownSec;

            _serverVersion = serverVersion;

            _screenBrightness = screenBrightness;
        }

        public int BatteryLevel
        {
            get
            {
                return _batteryLevel;
            }
            set
            {
                _batteryLevel = value;
            }
        }

        public string SocketName
        {
            get
            {
                return _socketName;
            }
            set
            {
                _socketName = value;
            }
        }

        public bool SocketState
        {
            get
            {
                return _socketState;
            }
            set
            {
                _socketState = value;
            }
        }

        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
            }
        }

        public string YoutubeId
        {
            get
            {
                return _youtubeId;
            }
            set
            {
                _youtubeId = value;
            }
        }

        public bool YoutubeIsPlaying
        {
            get
            {
                return _youtubeIsPlaying;
            }
            set
            {
                _youtubeIsPlaying = value;
            }
        }

        public int YoutubeVideoCurrentPlayTime
        {
            get
            {
                return _youtubeVideoCurrentPlayTime;
            }
            set
            {
                _youtubeVideoCurrentPlayTime = value;
            }
        }

        public int YoutubeVideoDuration
        {
            get
            {
                return _youtubeVideoDuration;
            }
            set
            {
                _youtubeVideoDuration = value;
            }
        }

        public bool RadioStreamIsPlaying
        {
            get
            {
                return _radioStreamIsPlaying;
            }
            set
            {
                _radioStreamIsPlaying = value;
            }
        }

        public int RadioStreamId
        {
            get
            {
                return _radioStreamId;
            }
            set
            {
                _radioStreamId = value;
            }
        }

        public bool SleepTimerIsEnabled
        {
            get
            {
                return _sleepTimerEnabled;
            }
            set
            {
                _sleepTimerEnabled = value;
            }
        }

        public int SleepTimerCountdownSec
        {
            get
            {
                return _countDownSec;
            }
            set
            {
                _countDownSec = value;
            }
        }

        public string ServerVersion
        {
            get
            {
                return _serverVersion;
            }
            set
            {
                _serverVersion = value;
            }
        }

        public int ScreenBrightness
        {
            get
            {
                return _screenBrightness;
            }
            set
            {
                _screenBrightness = value;
            }
        }
    }
}
