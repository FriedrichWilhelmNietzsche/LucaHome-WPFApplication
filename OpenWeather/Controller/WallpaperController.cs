using Common.Tools;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

/*
 * Helpful links: 
 * https://codereview.stackexchange.com/questions/60755/set-desktop-background
 * https://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
 */

namespace OpenWeather.Controller
{
    internal sealed class NativeMethods
    {
        // I am not at all intimate with this particular method of the Windows API so I did not 
        // tweak the signature. You should research this, though. 
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni);
    }

    public enum Style : int
    {
        Fill, Fit, Span, Stretch, Tile, Center
    }

    public class WallpaperController
    {
        private const String TAG = "WallpaperController";
        private readonly Logger _logger;

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public WallpaperController()
        {
            _logger = new Logger(TAG);
            _logger.Information("Created new WallpaperController");
        }

        public static void SetDesktopWallpaperFromBitmap(Bitmap bitmap, Style style)
        {
            Image image = (Image)bitmap;
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            image.Save(tempPath, ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Span) // Windows 8 or newer only!
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Stretch)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Tile)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.Center)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
