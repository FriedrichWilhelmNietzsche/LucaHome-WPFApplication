using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace LucaHome.UserControls
{
    public partial class ScrollableImage : UserControl, INotifyPropertyChanged
    {
        private const string TAG = "ScrollableImage";

        private Uri _imageWallpaperSource;

        public ScrollableImage()
        {
            _imageWallpaperSource = new Uri("/Common;component/Assets/Wallpaper/wallpaper.png", UriKind.Relative);

            InitializeComponent();

            ImageScrollViewer.ScrollToHorizontalOffset(67);
            ImageScrollViewer.ScrollToVerticalOffset(220);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Uri ImageWallpaperSource
        {
            get
            {
                return _imageWallpaperSource;
            }
            set
            {
                _imageWallpaperSource = value;
                OnPropertyChanged("ImageWallpaperSource");
            }
        }
    }
}
