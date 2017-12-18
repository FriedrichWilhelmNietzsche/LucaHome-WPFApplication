using Common.Dto;
using Data.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LucaHome.Dialogs
{
    public partial class SeasonDialog : Window, INotifyPropertyChanged
    {
        private const string TAG = "SeasonDialog";

        IList<string> _episodes;

        public SeasonDialog(SeasonDto season)
        {
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
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                string episode = (string)senderButton.Tag;
                SeriesService.Instance.WatchEpisode(episode);
                Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            Close();
        }
    }
}
