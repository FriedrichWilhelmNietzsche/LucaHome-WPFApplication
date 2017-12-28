using System;
using System.ComponentModel;
using System.Windows.Controls;
using LiveCharts;

namespace LucaHome.UserControls
{
    public partial class GraphCard : UserControl, INotifyPropertyChanged
    {
        private const string TAG = "GraphCard";

        private string _graphMeterId;
        private string _graphMeterType;
        private SeriesCollection _graphSeries;
        private string _graphMeterDetails;
        public string[] _graphLabels;
        public Func<double, string> _graphYFormatter;

        public GraphCard()
        {
            _graphMeterId = "";
            _graphMeterType = "";
            _graphSeries = new SeriesCollection();
            _graphMeterDetails = "";
            _graphLabels = new string[] { };
            _graphYFormatter = value => value.ToString();

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GraphMeterId
        {
            get
            {
                return _graphMeterId;
            }
            set
            {
                _graphMeterId = value;
                OnPropertyChanged("GraphMeterId");
            }
        }

        public string GraphMeterType
        {
            get
            {
                return _graphMeterType;
            }
            set
            {
                _graphMeterType = value;
                OnPropertyChanged("GraphMeterType");
            }
        }

        public SeriesCollection GraphSeries
        {
            get
            {
                return _graphSeries;
            }
            set
            {
                _graphSeries = value;
                OnPropertyChanged("GraphSeries");
            }
        }

        public string GraphMeterDetails
        {
            get
            {
                return _graphMeterDetails;
            }
            set
            {
                _graphMeterDetails = value;
                OnPropertyChanged("GraphMeterDetails");
            }
        }

        public string[] GraphLabels
        {
            get
            {
                return _graphLabels;
            }
            set
            {
                _graphLabels = value;
                OnPropertyChanged("GraphLabels");
            }
        }

        public Func<double, string> GraphYFormatter
        {
            get
            {
                return _graphYFormatter;
            }
            set
            {
                _graphYFormatter = value;
                OnPropertyChanged("GraphYFormatter");
            }
        }
    }
}