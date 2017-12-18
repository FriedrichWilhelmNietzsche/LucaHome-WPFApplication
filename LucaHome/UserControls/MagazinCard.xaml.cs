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

        private string _title;
        private BitmapImage _magazinIcon;

        // Help: https://stackoverflow.com/questions/13838884/how-to-bind-a-command-in-wpf
        private ICommand _buttonOpenExplorerCommand;
        private ICommand _mouseUpCommand;

        public MagazinCard()
        {
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
                return _magazinIcon;
            }
            set
            {
                _magazinIcon = value;
                OnPropertyChanged("MagazinIcon");
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

        public ICommand ButtonOpenExplorerCommand
        {
            get
            {
                return _buttonOpenExplorerCommand;
            }
            set
            {
                _buttonOpenExplorerCommand = value;
                OnPropertyChanged("ButtonOpenExplorerCommand");
            }
        }

        public ICommand MouseUpCommand
        {
            get
            {
                return _mouseUpCommand;
            }
            set
            {
                _mouseUpCommand = value;
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
            Logger.Instance.Error(TAG, "Please bind a command to this button!");
        }
    }
}
