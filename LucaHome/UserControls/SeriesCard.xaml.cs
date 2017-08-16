using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LucaHome.UserControls
{
    public partial class SeriesCard : UserControl, INotifyPropertyChanged
    {
        private const string TAG = "SeriesCard";
        private readonly Logger _logger;

        private string _seriesName;
        private BitmapImage _seriesIcon;

        // Help: https://stackoverflow.com/questions/13838884/how-to-bind-a-command-in-wpf
        private ICommand _buttonOpenExplorerCommand;
        private ICommand _mouseUpCommand;

        public SeriesCard()
        {
            _logger = new Logger(TAG);

            _seriesName = "Example";
            _seriesIcon = new BitmapImage(new Uri("/Common;component/Assets/Wallpaper/main_image_series.jpg", UriKind.Relative));

            _buttonOpenExplorerCommand = new DelegateCommand(dummyCommand);
            _mouseUpCommand = new DelegateCommand(dummyCommand);

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BitmapImage SeriesIcon
        {
            get
            {
                _logger.Debug(string.Format("Get SeriesIcon: {0}", _seriesIcon));
                return _seriesIcon;
            }
            set
            {
                _seriesIcon = value;
                _logger.Debug(string.Format("Set SeriesIcon: {0}", _seriesIcon));
                OnPropertyChanged("SeriesIcon");
            }
        }

        public string SeriesName
        {
            get
            {
                _logger.Debug(string.Format("Get SeriesName: {0}", _seriesName));
                return _seriesName;
            }
            set
            {
                _seriesName = value;
                _logger.Debug(string.Format("Set SeriesName: {0}", _seriesName));
                OnPropertyChanged("SeriesName");
            }
        }

        public ICommand ButtonOpenExplorerCommand
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonOpenExplorerCommand: {0}", _buttonOpenExplorerCommand));
                return _buttonOpenExplorerCommand;
            }
            set
            {
                _buttonOpenExplorerCommand = value;
                _logger.Debug(string.Format("Set ButtonOpenExplorerCommand: {0}", _buttonOpenExplorerCommand));
                OnPropertyChanged("ButtonOpenExplorerCommand");
            }
        }

        public ICommand MouseUpCommand
        {
            get
            {
                _logger.Debug(string.Format("Get MouseUpCommand: {0}", _mouseUpCommand));
                return _mouseUpCommand;
            }
            set
            {
                _mouseUpCommand = value;
                _logger.Debug(string.Format("Set MouseUpCommand: {0}", _mouseUpCommand));
                OnPropertyChanged("MouseUpCommand");
            }
        }

        private void SeriesCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseUpCommand.CanExecute(null))
            {
                MouseUpCommand.Execute(null);
            }
        }

        private void dummyCommand()
        {
            _logger.Error("Please bind a command to this button!");
        }
    }
}
