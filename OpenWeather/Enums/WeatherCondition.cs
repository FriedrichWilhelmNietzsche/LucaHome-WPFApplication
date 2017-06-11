using System;
using System.Collections.Generic;

namespace Common.Enums
{
    public class WeatherCondition
    {
        public static readonly WeatherCondition NULL = new WeatherCondition(
            0,
            "Null",
            new Uri("Assets/Images/wallpaper_dummy.png", UriKind.Relative),
            new Uri("Assets/Images/weather_dummy.png", UriKind.Relative));
        public static readonly WeatherCondition CLEAR = new WeatherCondition(
            1,
            "Clear",
            new Uri("Assets/Images/wallpaper_clear.png", UriKind.Relative),
            new Uri("Assets/Images/weather_clear.png", UriKind.Relative));
        public static readonly WeatherCondition CLOUD = new WeatherCondition(
            2,
            "Cloud",
            new Uri("Assets/Images/wallpaper_cloud.png", UriKind.Relative),
            new Uri("Assets/Images/weather_cloud.png", UriKind.Relative));
        public static readonly WeatherCondition DRIZZLE = new WeatherCondition(
            3,
            "Drizzle",
            new Uri("Assets/Images/wallpaper_drizzle.png", UriKind.Relative),
            new Uri("Assets/Images/weather_drizzle.png", UriKind.Relative));
        public static readonly WeatherCondition FOG = new WeatherCondition(
            4,
            "Fog",
            new Uri("Assets/Images/wallpaper_fog.png", UriKind.Relative),
            new Uri("Assets/Images/weather_fog.png", UriKind.Relative));
        public static readonly WeatherCondition HAZE = new WeatherCondition(
            5,
            "Haze",
            new Uri("Assets/Images/wallpaper_haze.png", UriKind.Relative),
            new Uri("Assets/Images/weather_haze.png", UriKind.Relative));
        public static readonly WeatherCondition MIST = new WeatherCondition(
            6,
            "Mist",
            new Uri("Assets/Images/wallpaper_mist.png", UriKind.Relative),
            new Uri("Assets/Images/weather_fog.png", UriKind.Relative));
        public static readonly WeatherCondition RAIN = new WeatherCondition(
            7,
            "Rain",
            new Uri("Assets/Images/wallpaper_rain.png", UriKind.Relative),
            new Uri("Assets/Images/weather_rain.png", UriKind.Relative));
        public static readonly WeatherCondition SLEET = new WeatherCondition(
            8,
            "Sleet",
            new Uri("Assets/Images/wallpaper_sleet.png", UriKind.Relative),
            new Uri("Assets/Images/weather_sleet.png", UriKind.Relative));
        public static readonly WeatherCondition SNOW = new WeatherCondition(
            9,
            "Snow",
            new Uri("Assets/Images/wallpaper_snow.png", UriKind.Relative),
            new Uri("Assets/Images/weather_snow.png", UriKind.Relative));
        public static readonly WeatherCondition SUN = new WeatherCondition(
            10,
            "Sun",
            new Uri("Assets/Images/wallpaper_sun.png", UriKind.Relative),
            new Uri("Assets/Images/weather_clear.png", UriKind.Relative));
        public static readonly WeatherCondition THUNDERSTORM = new WeatherCondition(
            11,
            "Thunderstorm",
            new Uri("Assets/Images/wallpaper_thunderstorm.png", UriKind.Relative),
            new Uri("Assets/Images/weather_thunderstorm.png", UriKind.Relative));

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
        private readonly Uri _icon;
        private readonly Uri _wallpaper;

        WeatherCondition(int id, string description, Uri icon, Uri wallpaper)
        {
            _id = id;
            _description = description;
            _icon = icon;
            _wallpaper = wallpaper;
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

        public override string ToString()
        {
            return _description;
        }
    }
}
