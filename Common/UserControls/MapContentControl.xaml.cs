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

        private string _buttonText;
        private string _buttonForeground;
        private string _buttonBackground;

        private string _buttonToolTip;

        // Help: https://stackoverflow.com/questions/13838884/how-to-bind-a-command-in-wpf
        private ICommand _buttonCommand;
        private Visibility _buttonVisibility;
        private bool _buttonEnabled;

        public MapContentControl()
        {
            _buttonText = "Jonas";
            _buttonForeground = "white";
            _buttonBackground = "darkblue";

            _buttonToolTip = "This is a map content";

            _buttonCommand = new DelegateCommand(dummyCommand);
            _buttonVisibility = Visibility.Visible;
            _buttonEnabled = true;

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
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                OnPropertyChanged("ButtonText");
            }
        }

        public string ButtonForeground
        {
            get
            {
                return _buttonForeground;
            }
            set
            {
                _buttonForeground = value;
                OnPropertyChanged("ButtonForeground");
            }
        }

        public string ButtonBackground
        {
            get
            {
                return _buttonBackground;
            }
            set
            {
                _buttonBackground = value;
                OnPropertyChanged("ButtonBackground");
            }
        }

        public string ButtonToolTip
        {
            get
            {
                return _buttonToolTip;
            }
            set
            {
                _buttonToolTip = value;
                OnPropertyChanged("ButtonToolTip");
            }
        }

        public ICommand ButtonCommand
        {
            get
            {
                return _buttonCommand;
            }
            set
            {
                _buttonCommand = value;
                OnPropertyChanged("ButtonCommand");
            }
        }

        public Visibility ButtonVisibility
        {
            get
            {
                return _buttonVisibility;
            }
            set
            {
                _buttonVisibility = value;
                OnPropertyChanged("ButtonVisibility");
            }
        }

        public bool ButtonEnabled
        {
            get
            {
                return _buttonEnabled;
            }
            set
            {
                _buttonEnabled = value;
                OnPropertyChanged("ButtonEnabled");
            }
        }

        private void dummyCommand()
        {
            Logger.Instance.Error(TAG, "Please bind a command to this button!");
        }
    }
}
