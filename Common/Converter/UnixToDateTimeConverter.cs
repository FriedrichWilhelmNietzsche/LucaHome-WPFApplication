using Common.Common;
using Common.Tools;
using System;

/*
 * Reference help
 * https://stackoverflow.com/questions/919244/converting-a-string-to-datetime
 * https://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa
 */

namespace Common.Converter
{
    public class UnixToDateTimeConverter
    {
        private const string TAG = "UnixToDateTimeConverter";
        private readonly Logger _logger;

        public UnixToDateTimeConverter()
        {
            _logger = new Logger(TAG, Enables.LOGGING);
        }

        public DateTime UnixTimeStampToDateTime(string unixTimeStampString)
        {
            _logger.Debug(string.Format("Trying to convert string UnixTimeStamp %s to DateTime!", unixTimeStampString));

            double unixTimeStamp = 0;
            try
            {
                unixTimeStamp = Convert.ToDouble(unixTimeStampString);
            }
            catch(Exception exception)
            {
                _logger.Error(exception.ToString());
            }
            
            return UnixTimeStampToDateTime(unixTimeStamp);
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            _logger.Debug(string.Format("Trying to convert double UnixTimeStamp %d to DateTime!", unixTimeStamp));

            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
