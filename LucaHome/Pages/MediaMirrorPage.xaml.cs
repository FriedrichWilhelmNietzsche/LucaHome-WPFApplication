using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class MediaMirrorPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MediaMirrorPage";

        private readonly NavigationService _navigationService;

        private string _youtubeVideoTitle = "";
        // TODO FIX generic
        private IList<object> _youtubeVideoList;

        public MediaMirrorPage(NavigationService navigationService)
        {
            _navigationService = navigationService;
            DataContext = this;

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string BatteryPercentText
        {
            get
            {
                return "Bat.: 69%";
            }
            set
            {
                OnPropertyChanged("BatteryPercentText");
            }
        }

        public string BatteryPercentTextColor
        {
            get
            {
                return "Black";
            }
            set
            {
                OnPropertyChanged("BatteryPercentTextColor");
            }
        }

        public string ServerVersionText
        {
            get
            {
                return "v2.0.2.170927";
            }
            set
            {
                OnPropertyChanged("ServerVersionText");
            }
        }

        public string VolumeText
        {
            get
            {
                return "Vol.: 6";
            }
            set
            {
                OnPropertyChanged("VolumeText");
            }
        }

        public string BrightnessText
        {
            get
            {
                return "Brightness.: 25%";
            }
            set
            {
                OnPropertyChanged("BrightnessText");
            }
        }

        public string YoutubeVideoTitle
        {
            get
            {
                return _youtubeVideoTitle;
            }
            set
            {
                _youtubeVideoTitle = value;
                OnPropertyChanged("YoutubeVideoTitle");
            }
        }

        // TODO FIX generic
        public IList<object> YoutubeVideoList
        {
            get
            {
                return _youtubeVideoList;
            }
            set
            {
                _youtubeVideoList = value;
                OnPropertyChanged("YoutubeVideoList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonVolumeIncrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonVolumeDecrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonBrightnessIncrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonBrightnessDecrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonReloadServer_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonSearchYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void PlayYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string youtubeVideoTitle = (string)senderButton.Tag;
                // select youtube id and send it to the mediamirror
            }
        }

        private void ButtonPlayYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonPauseYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonStopYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonSendRadioPlay_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonSendRadioStop_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonSendText_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            // TODO
        }
    }
}
