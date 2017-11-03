using Common.Common;
using Common.Tools;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class MediaMirrorPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MediaMirrorPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;

        public MediaMirrorPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

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

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} with arguments {1}", sender, routedEventArgs));
            // TODO
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} with arguments {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonVolumeIncrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonVolumeIncrease_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonVolumeDecrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonVolumeDecrease_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonBrightnessIncrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBrightnessIncrease_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonBrightnessDecrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBrightnessDecrease_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonReloadServer_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReloadServer_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonPlayYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonPlayYoutubeVideo_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonPauseYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonPauseYoutubeVideo_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonStopYoutubeVideo_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonStopYoutubeVideo_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonSendRadioPlay_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonSendRadioPlay_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonSendRadioStop_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonSendRadioStop_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonSendText_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonSendText_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            // TODO
        }
    }
}
