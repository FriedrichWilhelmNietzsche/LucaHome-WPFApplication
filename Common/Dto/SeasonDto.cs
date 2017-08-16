namespace Common.Dto
{
    public class SeasonDto
    {
        private const string TAG = "SeasonDto";

        private string _season;
        private string[] _episodes;

        public SeasonDto(string season, string[] episodes)
        {
            _season = season;
            _episodes = episodes;
        }

        public string Season
        {
            get
            {
                return _season;
            }
        }

        public string[] Episodes
        {
            get
            {
                return _episodes;
            }
            set
            {
                _episodes = value;
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (Season: {1} );(Episodes: {2} ))", TAG, _season, _episodes);
        }
    }
}
