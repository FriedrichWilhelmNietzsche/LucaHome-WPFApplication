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

        // Help: https://social.msdn.microsoft.com/Forums/vstudio/en-US/8a1648fc-ddd7-427b-a7cd-99f04c506b7c/a-binding-cannot-be-set-on-the-text-property-of-type-webhyperlink-a-binding-can-only-be-set?forum=wpf
        private static readonly DependencyProperty BottomTitleProperty = DependencyProperty.Register("BottomTitle", typeof(string), typeof(MainPageCard));
        private static readonly DependencyProperty BottomDataProperty = DependencyProperty.Register("BottomData", typeof(string), typeof(MainPageCard));

        private string _title;
        private string _description;

        private Uri _cardImage;

        // Help: https://stackoverflow.com/questions/13838884/how-to-bind-a-command-in-wpf
        private ICommand _buttonAddCommand;
        private Visibility _buttonAddVisibility;

        private ICommand _buttonMapCommand;
        private Visibility _buttonMapVisibility;

        private string _bottomTitle;
        private string _bottomData;

        public MainPageCard()
        {
            _cardImage = new Uri("/Common;component/Assets/Wallpaper/wallpaper.png", UriKind.Relative);

            _title = "Example";
            _description = "Lorem ipsum...";

            _buttonAddCommand = new DelegateCommand(dummyCommand);
            _buttonAddVisibility = Visibility.Collapsed;

            _buttonMapCommand = new DelegateCommand(dummyCommand);
            _buttonMapVisibility = Visibility.Collapsed;

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
                return _cardImage;
            }
            set
            {
                _cardImage = value;
                OnPropertyChanged("CardImage");
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public ICommand ButtonMapCommand
        {
            get
            {
                return _buttonMapCommand;
            }
            set
            {
                _buttonMapCommand = value;
                OnPropertyChanged("ButtonMapCommand");
            }
        }

        public Visibility ButtonMapVisibility
        {
            get
            {
                return _buttonMapVisibility;
            }
            set
            {
                _buttonMapVisibility = value;
                OnPropertyChanged("ButtonMapVisibility");
            }
        }

        public ICommand ButtonAddCommand
        {
            get
            {
                return _buttonAddCommand;
            }
            set
            {
                _buttonAddCommand = value;
                OnPropertyChanged("ButtonAddCommand");
            }
        }

        public Visibility ButtonAddVisibility
        {
            get
            {
                return _buttonAddVisibility;
            }
            set
            {
                _buttonAddVisibility = value;
                OnPropertyChanged("ButtonAddVisibility");
            }
        }

        public string BottomTitle
        {
            get
            {
                return _bottomTitle;
            }
            set
            {
                _bottomTitle = value;
                OnPropertyChanged("BottomTitle");
            }
        }

        public string BottomData
        {
            get
            {
                return _bottomData;
            }
            set
            {
                _bottomData = value;
                OnPropertyChanged("BottomData");
            }
        }

        private void dummyCommand()
        {
            Logger.Instance.Error(TAG, "Please bind a command to this button!");
        }
    }
}
