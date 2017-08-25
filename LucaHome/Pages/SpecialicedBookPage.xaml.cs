using Common.Common;
using Common.Tools;
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
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly SpecialicedBookService _specialicedBookService;

        private string _bookListSearchKey = string.Empty;
        private IList<string> _bookList = new List<string>();

        public SpecialicedBookPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _specialicedBookService = SpecialicedBookService.Instance;

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
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _specialicedBookService.OnSpecialicedBookListDownloadEventHandler += _onBookListDownloadFinished;

            if (_specialicedBookService.BookList == null)
            {
                _specialicedBookService.LoadBookList();
                return;
            }

            BookList = _specialicedBookService.BookList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _specialicedBookService.OnSpecialicedBookListDownloadEventHandler -= _onBookListDownloadFinished;
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

                if (_bookListSearchKey != string.Empty)
                {
                    BookList = _specialicedBookService.FoundBooks(_bookListSearchKey);
                }
                else
                {
                    BookList = _specialicedBookService.BookList;
                }
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
            _logger.Debug(string.Format("_onBookListDownloadFinished with object {0} and response {1} was successful {2}", bookList, response, success));
            BookList = _specialicedBookService.BookList;
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string booktitle = (string)senderButton.Tag;
                _specialicedBookService.StartReading(booktitle);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _specialicedBookService.LoadBookList();
        }
    }
}
