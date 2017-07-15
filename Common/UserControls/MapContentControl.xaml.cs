using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Common.UserControls
{
    public partial class MapContentControl : UserControl, INotifyPropertyChanged
    {
        private const string TAG = "MapContent";
        private readonly Logger _logger;

        private string _buttonText;
        private string _buttonToolTip;

        // Help: https://stackoverflow.com/questions/13838884/how-to-bind-a-command-in-wpf
        private ICommand _buttonCommand;
        private Visibility _buttonVisibility;
        private bool _buttonEnabled;

        public MapContentControl()
        {
            _logger = new Logger(TAG);

            _buttonText = "JON";
            _buttonToolTip = "This is a map content";

            _buttonCommand = new DelegateCommand(dummyCommand);
            _buttonVisibility = Visibility.Visible;

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ButtonText
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonText: {0}", _buttonText));
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                _logger.Debug(string.Format("Set ButtonText: {0}", _buttonText));
                OnPropertyChanged("ButtonText");
            }
        }

        public string ButtonToolTip
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonToolTip: {0}", _buttonToolTip));
                return _buttonToolTip;
            }
            set
            {
                _buttonToolTip = value;
                _logger.Debug(string.Format("Set ButtonToolTip: {0}", _buttonToolTip));
                OnPropertyChanged("ButtonToolTip");
            }
        }

        public ICommand ButtonCommand
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonCommand: {0}", _buttonCommand));
                return _buttonCommand;
            }
            set
            {
                _buttonCommand = value;
                _logger.Debug(string.Format("Set ButtonCommand: {0}", _buttonCommand));
                OnPropertyChanged("ButtonCommand");
            }
        }

        public Visibility ButtonVisibility
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonVisibility: {0}", _buttonVisibility));
                return _buttonVisibility;
            }
            set
            {
                _buttonVisibility = value;
                _logger.Debug(string.Format("Set ButtonVisibility: {0}", _buttonVisibility));
                OnPropertyChanged("ButtonVisibility");
            }
        }

        public bool ButtonEnabled
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonEnabled: {0}", _buttonEnabled));
                return _buttonEnabled;
            }
            set
            {
                _buttonEnabled = value;
                _logger.Debug(string.Format("Set ButtonEnabled: {0}", _buttonEnabled));
                OnPropertyChanged("ButtonEnabled");
            }
        }

        private void dummyCommand()
        {
            _logger.Error("Please bind a command to this button!");
        }
    }
}
