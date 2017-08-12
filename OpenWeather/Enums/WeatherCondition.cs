using System;
using System.Collections.Generic;

namespace Common.Enums
{
    public class WeatherCondition
    {
        public static readonly WeatherCondition NULL = new WeatherCondition(
            0,
            "Null",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_dummy.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_dummy.png", UriKind.Relative),
            "Null",
            "Null",
            "Null");

        public static readonly WeatherCondition CLEAR = new WeatherCondition(
            1,
            "Clear",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_clear.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_clear.png", UriKind.Relative),
            "Go to the park or river and enjoy the clear weather today!",
            "Today will be clear! Get out for lunch!",
            "Enjoy your day after work! Today will be clear!");

        public static readonly WeatherCondition CLOUD = new WeatherCondition(
            2,
            "Cloud",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_cloud.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_cloud.png", UriKind.Relative),
            "Sun is hiding today.",
            "No sun today. Not bad to work...",
            "No sun after work today, cloudy!");

        public static readonly WeatherCondition DRIZZLE = new WeatherCondition(
            3,
            "Drizzle",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_drizzle.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_drizzle.png", UriKind.Relative),
            "It's a cold and rainy day!",
            "There will be drizzle today!",
            "There will be drizzle after work today!");

        public static readonly WeatherCondition FOG = new WeatherCondition(
            4,
            "Fog",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_fog.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_fog.png", UriKind.Relative),
            "You're not gonna see your hand today! :P",
            "Find your way to work today :P",
            "Find your way to home from work today :P");

        public static readonly WeatherCondition HAZE = new WeatherCondition(
            5,
            "Haze",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_haze.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_haze.png", UriKind.Relative),
            "Will be haze today!",
            "Search your way, master!",
            "Haze on your way from work today!");

        public static readonly WeatherCondition MIST = new WeatherCondition(
            6,
            "Mist",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_mist.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_fog.png", UriKind.Relative),
            "Will be misty today!",
            "Watch out today!",
            "Mist on your way from work today!");

        public static readonly WeatherCondition RAIN = new WeatherCondition(
            7,
            "Rain",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_rain.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_rain.png", UriKind.Relative),
            "It's a rainy day! Chill at home ;)",
            "It will rain today! Take an umbrella or take your car to work.",
            "It will rain after work today!");

        public static readonly WeatherCondition SLEET = new WeatherCondition(
            8,
            "Sleet",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_sleet.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_sleet.png", UriKind.Relative),
            "Today will be a freezy and slittering day!",
            "Take care outside today!",
            "Slittering way home from work today!");

        public static readonly WeatherCondition SNOW = new WeatherCondition(
            9,
            "Snow",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_snow.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_snow.png", UriKind.Relative),
            "Today will be a snowy day!",
            "Snow today. Think twice taking your bike!",
            "Today will be a snowy way back from work!");

        public static readonly WeatherCondition SUN = new WeatherCondition(
            10,
            "Sun",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_sun.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_clear.png", UriKind.Relative),
            "Enjoy the sunny weather today and chill!",
            "Today will be sunny! Get out for lunch!",
            "Enjoy your afterwork today! Will be sunny!");

        public static readonly WeatherCondition THUNDERSTORM = new WeatherCondition(
            11,
            "Thunderstorm",
            new Uri("/OpenWeather;component/Assets/Images/weather_wallpaper_thunderstorm.png", UriKind.Relative),
            new Uri("/OpenWeather;component/Assets/Images/weather_thunderstorm.png", UriKind.Relative),
            "Thunder is coming today!",
            "Prepare for a thunderstorm today!",
            "Today afternoon will be a thunderstorm!");

        public static IEnumerable<WeatherCondition> Values
        {
            get
            {
                yield return NULL;
                yield return CLEAR;
                yield return CLOUD;
                yield return DRIZZLE;
                yield return FOG;
                yield return HAZE;
                yield return MIST;
                yield return RAIN;
                yield return SLEET;
                yield return SNOW;
                yield return SUN;
                yield return THUNDERSTORM;
            }
        }

        private readonly int _id;
        private readonly string _description;
        private readonly Uri _wallpaper;
        private readonly Uri _icon;

        private readonly string _weekendTip;
        private readonly string _workdayTip;
        private readonly string _workdayAfterWorkTip;

        WeatherCondition(int id, string description, Uri wallpaper, Uri icon, string weekendTip, string workdayTip, string workdayAfterWorkTip)
        {
            _id = id;
            _description = description;
            _wallpaper = wallpaper;
            _icon = icon;

            _weekendTip = weekendTip;
            _workdayTip = workdayTip;
            _workdayAfterWorkTip = workdayAfterWorkTip;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public Uri Icon
        {
            get
            {
                return _icon;
            }
        }

        public Uri Wallpaper
        {
            get
            {
                return _wallpaper;
            }
        }

        public string WeekendTip
        {
            get
            {
                return _weekendTip;
            }
        }

        public string WorkdayTip
        {
            get
            {
                return _workdayTip;
            }
        }

        public string WorkdayAfterWorkTip
        {
            get
            {
                return _workdayAfterWorkTip;
            }
        }

        public static WeatherCondition GetById(int id)
        {
            foreach (WeatherCondition entry in Values)
            {
                if (entry.Id == id)
                {
                    return entry;
                }
            }

            return NULL;
        }

        public static WeatherCondition GetByDescription(string description)
        {
            foreach (WeatherCondition entry in Values)
            {
                if (entry.Description.ToLower().Equals(description.ToLower())
                    || entry.Description.ToLower().Contains(description.ToLower())
                    || description.ToLower().Equals(entry.Description.ToLower())
                    || description.ToLower().Contains(entry.Description.ToLower())
                    || entry.Description.ToUpper().Equals(description.ToUpper())
                    || entry.Description.ToUpper().Contains(description.ToUpper())
                    || description.ToUpper().Equals(entry.Description.ToUpper())
                    || description.ToUpper().Contains(entry.Description.ToUpper()))
                {
                    return entry;
                }
            }

            return NULL;
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
