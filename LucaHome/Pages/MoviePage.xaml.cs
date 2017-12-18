using Common.Dto;
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

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private string _movieSearchKey = string.Empty;
        private IList<MovieDto> _movieList = new List<MovieDto>();

        public MoviePage(NavigationService navigationService)
        {
            _navigationService = navigationService;

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
                MovieList = MovieService.Instance.FoundMovies(_movieSearchKey);
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
            MovieService.Instance.OnMovieDownloadFinished += _movieListDownloadFinished;
            MovieService.Instance.OnMovieStartFinished += _movieStartFinished;

            if (MovieService.Instance.MovieList == null)
            {
                MovieService.Instance.LoadMovieList();
                return;
            }

            MovieList = MovieService.Instance.MovieList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MovieService.Instance.OnMovieDownloadFinished -= _movieListDownloadFinished;
            MovieService.Instance.OnMovieStartFinished -= _movieStartFinished;
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                string movieTitle = (string)senderButton.Tag;
                MovieService.Instance.StartMovieOnPc(movieTitle);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            MovieService.Instance.LoadMovieList();
        }

        private void ButtonUpdateMovie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int movieId = (int)senderButton.Tag;
                MovieDto updateMovie = MovieService.Instance.GetById(movieId);

                MovieUpdatePage movieUpdatePage = new MovieUpdatePage(_navigationService, updateMovie);
                _navigationService.Navigate(movieUpdatePage);
            }
        }

        private void _movieListDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            MovieList = MovieService.Instance.MovieList;
        }

        private void _movieStartFinished(bool success, string response)
        {
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
