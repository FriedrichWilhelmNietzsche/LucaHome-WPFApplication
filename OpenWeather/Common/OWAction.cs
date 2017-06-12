namespace OpenWeather.Common
{
    public class OWAction
    {
        public const string CALL_FORECAST_WEATHER = "http://api.openweathermap.org/data/2.5/forecast?q={0}&units=metric&APPID=" + OWKeys.OPEN_WEATHER_KEY;
        public const string CALL_CURRENT_WEATHER = "http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&APPID=" + OWKeys.OPEN_WEATHER_KEY;
    }
}
