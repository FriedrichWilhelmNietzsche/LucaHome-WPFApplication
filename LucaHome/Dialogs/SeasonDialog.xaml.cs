using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace LucaHome.Dialogs
{
    public partial class SeasonDialog : Window, INotifyPropertyChanged
    {
        private const string TAG = "SeasonDialog";
        private readonly Logger _logger;

        private readonly SeriesService _seriesService;

        IList<string> _episodes;

        public SeasonDialog(SeasonDto season)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _seriesService = SeriesService.Instance;

            _episodes = season.Episodes;

            InitializeComponent();
            DataContext = this;

            Title.Text = season.Season;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public IList<string> Episodes
        {
            get
            {
                return _episodes;
            }
            set
            {
                _episodes = value;
                OnPropertyChanged("Episodes");
            }
        }

        private void Watch_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string command = (string)senderButton.Tag;
                Process.Start(command);
                Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Close_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            Close();
        }
    }
}
