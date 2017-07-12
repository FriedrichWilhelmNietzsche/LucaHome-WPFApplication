using System.Collections.Generic;

namespace Common.Common
{
    public class Constants
    {
        // ========== DOWNLOADS ==========
        public static int DOWNLOAD_STEPS = 9;

        // ========== RASPBERRY CONNECTION ==========
        public static string ACTION_PATH = "/lib/lucahome.php?user=";
        public static string REST_PASSWORD = "&password=";
        public static string REST_ACTION = "&action=";
        public static string STATE_ON = "&state=1";
        public static string STATE_OFF = "&state=0";

        // ========== FURTHER DATA ==========
        public static string CITY = "Munich, DE";
        public static string LUCAHOME_SSID = "BellaPeca";
        public static string ACTIVATED = "Activated";
        public static string DEACTIVATED = "Deactivated";
        public static string ACTIVE = "Active";
        public static string INACTIVE = "Inactive";

        // ========== MEDIAMIRROR DATA ==========
        public static IList<string> MEDIAMIRROR_PLAYER = new List<string>() { "1", "2" };
        public static int MEDIAMIRROR_SERVERPORT = 8080;
        public static string DIR_UP = "UP";
	    public static string DIR_DOWN = "DOWN";
	    public static string DIR_RIGHT = "RIGHT";
	    public static string DIR_LEFT = "LEFT";
	    public static string DIR_ROTATE = "ROTATE";

	    // ========== YOUTUBE DATA ==========
	    public static string YOUTUBE_SEARCH = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=%d&q=%s&key=%s";
	    public static int YOUTUBE_MAX_RESULTS = 15;
    }
}
