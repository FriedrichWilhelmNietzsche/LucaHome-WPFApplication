using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LucaHome.UserControls
{
    public partial class MagazinCard : UserControl, INotifyPropertyChanged
    {
        private const string TAG = "MagazinCard";
        private readonly Logger _logger;

        private string _title;
        private BitmapImage _magazinIcon;

        // Help: https://stackoverflow.com/questions/13838884/how-to-bind-a-command-in-wpf
        private ICommand _buttonOpenExplorerCommand;
        private ICommand _mouseUpCommand;

        public MagazinCard()
        {
            _logger = new Logger(TAG);

            _title = "Example";
            _magazinIcon = new BitmapImage(new Uri("/Common;component/Assets/Wallpaper/main_image_magazine.png", UriKind.Relative));

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

        public BitmapImage MagazinIcon
        {
            get
            {
                _logger.Debug(string.Format("Get MagazinIcon: {0}", _magazinIcon));
                return _magazinIcon;
            }
            set
            {
                _magazinIcon = value;
                _logger.Debug(string.Format("Set MagazinIcon: {0}", _magazinIcon));
                OnPropertyChanged("MagazinIcon");
            }
        }

        public string Title
        {
            get
            {
                _logger.Debug(string.Format("Get Title: {0}", _title));
                return _title;
            }
            set
            {
                _title = value;
                _logger.Debug(string.Format("Set Title: {0}", _title));
                OnPropertyChanged("Title");
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

        private void MagazinCard_MouseUp(object sender, MouseButtonEventArgs e)
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
