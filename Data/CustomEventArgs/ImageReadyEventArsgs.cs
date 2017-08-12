using System;
using System.Windows.Media.Imaging;

namespace Data.CustomEventArgs
{
    public class ImageReadyEventArsgs : EventArgs
    {
        public BitmapImage Image { get; set; }
    }
}
