using Data.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class SpecialicedBookPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "SpecialicedBookPage";

        private readonly NavigationService _navigationService;

        private string _bookListSearchKey = string.Empty;
        private IList<string> _bookList = new List<string>();

        public SpecialicedBookPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SpecialicedBookService.Instance.OnSpecialicedBookListDownloadEventHandler += _onBookListDownloadFinished;

            if (SpecialicedBookService.Instance.BookList == null)
            {
                SpecialicedBookService.Instance.LoadBookList();
                return;
            }

            BookList = SpecialicedBookService.Instance.BookList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SpecialicedBookService.Instance.OnSpecialicedBookListDownloadEventHandler -= _onBookListDownloadFinished;
        }

        public string BookListSearchKey
        {
            get
            {
                return _bookListSearchKey;
            }
            set
            {
                _bookListSearchKey = value;
                OnPropertyChanged("BookListSearchKey");
                BookList = SpecialicedBookService.Instance.FoundBooks(_bookListSearchKey);
            }
        }

        public IList<string> BookList
        {
            get
            {
                return _bookList;
            }
            set
            {
                _bookList = value;
                OnPropertyChanged("BookList");
            }
        }

        private void _onBookListDownloadFinished(IList<string> bookList, bool success, string response)
        {
            BookList = SpecialicedBookService.Instance.BookList;
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string booktitle = (string)senderButton.Tag;
                SpecialicedBookService.Instance.StartReading(booktitle);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            SpecialicedBookService.Instance.LoadBookList();
        }
    }
}
