using Common.Tools;
using Microsoft.Practices.Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LucaHome.UserControls
{
    public partial class MainPageCard : UserControl, INotifyPropertyChanged
    {
        private const string TAG = "MainPageCard";
        private readonly Logger _logger;

        // Help: https://social.msdn.microsoft.com/Forums/vstudio/en-US/8a1648fc-ddd7-427b-a7cd-99f04c506b7c/a-binding-cannot-be-set-on-the-text-property-of-type-webhyperlink-a-binding-can-only-be-set?forum=wpf
        private static readonly DependencyProperty BottomTitleProperty = DependencyProperty.Register("BottomTitle", typeof(string), typeof(MainPageCard));
        private static readonly DependencyProperty BottomDataProperty = DependencyProperty.Register("BottomData", typeof(string), typeof(MainPageCard));

        private string _title;
        private string _description;

        private Uri _cardImage;

        // Help: https://stackoverflow.com/questions/13838884/how-to-bind-a-command-in-wpf
        private ICommand _buttonAddCommand;
        private Visibility _buttonAddVisibility;

        private string _bottomTitle;
        private string _bottomData;

        public MainPageCard()
        {
            _logger = new Logger(TAG);

            _cardImage = new Uri("/Common;component/Assets/Wallpaper/wallpaper.png", UriKind.Relative);

            _title = "Example";
            _description = "Lorem ipsum...";

            _buttonAddCommand = new DelegateCommand(dummyCommand);
            _buttonAddVisibility = Visibility.Collapsed;

            _bottomTitle = string.Empty;
            _bottomData = string.Empty;

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Uri CardImage
        {
            get
            {
                _logger.Debug(string.Format("Get CardImage: {0}", _cardImage));
                return _cardImage;
            }
            set
            {
                _cardImage = value;
                _logger.Debug(string.Format("Set CardImage: {0}", _cardImage));
                OnPropertyChanged("CardImage");
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

        public string Description
        {
            get
            {
                _logger.Debug(string.Format("Get Description: {0}", _description));
                return _description;
            }
            set
            {
                _description = value;
                _logger.Debug(string.Format("Set Description: {0}", _description));
                OnPropertyChanged("Description");
            }
        }

        public ICommand ButtonAddCommand
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonAddCommand: {0}", _buttonAddCommand));
                return _buttonAddCommand;
            }
            set
            {
                _buttonAddCommand = value;
                _logger.Debug(string.Format("Set ButtonAddCommand: {0}", _buttonAddCommand));
                OnPropertyChanged("ButtonAddCommand");
            }
        }

        private void dummyCommand()
        {
            _logger.Error("Please bind a command to this button!");
        }

        public Visibility ButtonAddVisibility
        {
            get
            {
                _logger.Debug(string.Format("Get ButtonAddVisibility: {0}", _buttonAddVisibility));
                return _buttonAddVisibility;
            }
            set
            {
                _buttonAddVisibility = value;
                _logger.Debug(string.Format("Set ButtonAddVisibility: {0}", _buttonAddVisibility));
                OnPropertyChanged("ButtonAddVisibility");
            }
        }

        public string BottomTitle
        {
            get
            {
                _logger.Debug(string.Format("Get BottomTitle: {0}", _bottomTitle));
                return _bottomTitle;
            }
            set
            {
                _bottomTitle = value;
                _logger.Debug(string.Format("Set BottomTitle: {0}", _bottomTitle));
                OnPropertyChanged("BottomTitle");
            }
        }

        public string BottomData
        {
            get
            {
                _logger.Debug(string.Format("Get BottomData: {0}", _bottomData));
                return _bottomData;
            }
            set
            {
                _bottomData = value;
                _logger.Debug(string.Format("Set BottomData: {0}", _bottomData));
                OnPropertyChanged("BottomData");
            }
        }

        private void MainPageCardContent_MouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void MainPageCardContent_MouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
