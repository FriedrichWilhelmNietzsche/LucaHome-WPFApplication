namespace Common.Constants
{
    public class Constants
    {
        // Raspberry Pi Server Connection
        public static string[] SERVER_URLs = new string[] { "http:////192.168.178.22" };
        public static string ACTION_PATH = "/lib/lucahome.php?user=";
        public static string REST_PASSWORD = "&password=";
	    public static string REST_ACTION = "&action=";
	    public static string STATE_ON = "&state=1";
	    public static string STATE_OFF = "&state=0";

        // Further Constants
        public static string CITY = "muenchen";
	    public static string LUCAHOME_SSID = "BellaPeca";
	    public static string ACTIVATED = "Activated";
	    public static string DEACTIVATED = "Deactivated";
	    public static string ACTIVE = "Active";
	    public static string INACTIVE = "Inactive";
    }
}
