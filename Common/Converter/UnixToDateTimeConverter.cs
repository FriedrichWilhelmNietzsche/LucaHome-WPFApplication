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

        private static UnixToDateTimeConverter _instance = null;
        private static readonly object _padlock = new object();

        UnixToDateTimeConverter()
        {
            // Empty constructor, nothing needed here
        }

        public static UnixToDateTimeConverter Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new UnixToDateTimeConverter();
                    }

                    return _instance;
                }
            }
        }

        public DateTime UnixTimeStampToDateTime(string unixTimeStampString)
        {
            Logger.Instance.Debug(TAG, string.Format("Trying to convert string UnixTimeStamp %s to DateTime!", unixTimeStampString));

            double unixTimeStamp = 0;
            try
            {
                unixTimeStamp = Convert.ToDouble(unixTimeStampString);
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(TAG, exception.ToString());
            }

            return UnixTimeStampToDateTime(unixTimeStamp);
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            Logger.Instance.Debug(TAG, string.Format("Trying to convert double UnixTimeStamp %d to DateTime!", unixTimeStamp));

            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
