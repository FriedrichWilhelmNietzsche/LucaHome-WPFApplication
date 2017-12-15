using System.Collections.Generic;

namespace Common.Common
{
    public class Constants
    {
        // ========== DOWNLOADS ==========
        public static int DOWNLOAD_STEPS = 18;

        // ========== RASPBERRY CONNECTION ==========
        public static string ACTION_PATH = "/lib/lucahome.php?user=";
        public static string REST_PASSWORD = "&password=";
        public static string REST_ACTION = "&action=";
        public static string STATE_ON = "&state=1";
        public static string STATE_OFF = "&state=0";

        // ========== FURTHER DATA ==========
        public static string CITY = "Nuremberg";
        public static string LUCAHOME_SSID = "GuepardoHome";
        public static string ACTIVATED = "Activated";
        public static string DEACTIVATED = "Deactivated";
        public static string ACTIVE = "Active";
        public static string INACTIVE = "Inactive";

        // ========== MEDIAMIRROR DATA ==========
        public static int MEDIAMIRROR_SERVERPORT = 8080;

	    // ========== YOUTUBE DATA ==========
	    public static string YOUTUBE_SEARCH = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=%d&q=%s&key=%s";
	    public static int YOUTUBE_MAX_RESULTS = 25;
    }
}
