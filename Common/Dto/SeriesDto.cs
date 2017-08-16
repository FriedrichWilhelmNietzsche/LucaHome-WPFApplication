using System.Windows.Media.Imaging;

namespace Common.Dto
{
    public class SeriesDto
    {
        private const string TAG = "SeriesDirDto";

        private string _seriesName;
        private SeasonDto[] _seriesSeasons;
        private BitmapImage _icon;

        public SeriesDto(string seriesName, SeasonDto[] seriesSeasons, BitmapImage icon)
        {
            _seriesName = seriesName;
            _seriesSeasons = seriesSeasons;
            _icon = icon;
        }

        public string SeriesName
        {
            get
            {
                return _seriesName;
            }
        }

        public SeasonDto[] SeriesSeasons
        {
            get
            {
                return _seriesSeasons;
            }
            set
            {
                _seriesSeasons = value;
            }
        }

        public BitmapImage Icon
        {
            get
            {
                return _icon;
            }
        }

        public override string ToString()
        {
            return string.Format("( {0}: (SeriesName: {1} );(SeriesSeasons: {2} );(Icon: {3} ))", TAG, _seriesName, _seriesSeasons, _icon);
        }
    }
}
