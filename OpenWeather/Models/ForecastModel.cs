using Common.Tools;
using OpenWeather.Common;
using System.Collections.Generic;

namespace OpenWeather.Models
{
    public class ForecastModel
    {
        private const string TAG = "ForecastModel";
        private Logger _logger;

        private string _city;
        private string _country;
        private IList<ForecastPartModel> _list = new List<ForecastPartModel>();

        public ForecastModel(
            string city, 
            string country, 
            IList<ForecastPartModel> list)
        {
            _logger = new Logger(TAG, OWEnables.LOGGING);

            _city = city;
            _country = country;
            _list = list;
        }

        public string City
        {
            get
            {
                return _city;
            }
        }

        public string Country
        {
            get
            {
                return _country;
            }
        }

        public IList<ForecastPartModel> List
        {
            get
            {
                return _list;
            }
        }
    }
}
