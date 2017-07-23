using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 */

namespace LucaHome.Pages
{
    public partial class MoviePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MoviePage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly MovieService _movieService;

        private readonly Notifier _notifier;

        private string _movieSearchKey = string.Empty;
        private IList<MovieDto> _movieList = new List<MovieDto>();

        public MoviePage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _movieService = MovieService.Instance;

            InitializeComponent();
            DataContext = this;

            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 15,
                    offsetY: 15);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            });

            _notifier.ClearMessages();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string MovieSearchKey
        {
            get
            {
                return _movieSearchKey;
            }
            set
            {
                _movieSearchKey = value;
                OnPropertyChanged("MovieSearchKey");

                if (_movieSearchKey != string.Empty)
                {
                    MovieList = _movieService.FoundMovies(_movieSearchKey);
                }
                else
                {
                    MovieList = _movieService.MovieList;
                }
            }
        }

        public IList<MovieDto> MovieList
        {
            get
            {
                return _movieList;
            }
            set
            {
                _movieList = value;
                OnPropertyChanged("MovieList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _movieService.OnMovieDownloadFinished += _movieListDownloadFinished;
            _movieService.OnMovieStartFinished += _movieStartFinished;

            if (_movieService.MovieList == null)
            {
                _movieService.LoadMovieList();
                return;
            }

            MovieList = _movieService.MovieList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _movieService.OnMovieDownloadFinished -= _movieListDownloadFinished;
            _movieService.OnMovieStartFinished -= _movieStartFinished;
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string movieTitle = (string)senderButton.Tag;
                _movieService.StartMovieOnPc(movieTitle);
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
            _movieService.LoadMovieList();
        }

        private void ButtonUpdateMovie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonUpdateMovie_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int movieId = (int)senderButton.Tag;
                MovieDto updateMovie = _movieService.GetById(movieId);
                _logger.Warning(string.Format("Updating movie {0}!", updateMovie));

                MovieUpdatePage movieUpdatePage = new MovieUpdatePage(_navigationService, updateMovie);
                _navigationService.Navigate(movieUpdatePage);
            }
        }

        private void _movieListDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            _logger.Debug(string.Format("_movieListDownloadFinished with model {0} was successful: {1}", movieList, success));
            MovieList = _movieService.MovieList;
        }

        private void _movieStartFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_movieStartFinished was successful: {0}", success));
            if (success)
            {
                _notifier.ShowSuccess("Successfully started movie");
            }
            else
            {
                _notifier.ShowError(string.Format("Failed to start movie!\n{0}", response));
            }
        }
    }
}
