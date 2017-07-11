using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using System.Collections.Generic;
using LucaHome.Rules;

namespace LucaHome.Pages
{
    public partial class MovieUpdatePage : Page
    {
        private const string TAG = "MovieUpdatePage";
        private readonly Logger _logger;

        private readonly MovieService _movieService;
        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private MovieDto _updateMovie;

        public MovieUpdatePage(NavigationService navigationService, MovieDto updateMovie)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _movieService = MovieService.Instance;
            _navigationService = navigationService;

            _updateMovie = updateMovie;

            InitializeComponent();

            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 15,
                    offsetY: 15);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(2),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(2));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            });

            _notifier.ClearMessages();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            TitleTextBox.Text = _updateMovie.Title;
            GenreTextBox.Text = _updateMovie.Genre;
            DescriptionTextBox.Text = _updateMovie.Description;
            MovieRatingBar.Value = _updateMovie.Rating;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _movieService.OnMovieDownloadFinished -= _onMovieDownloadFinished;
            _movieService.OnMovieUpdateFinished -= _onMovieUpdateFinished;
        }

        private void UpdateMovie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("UpdateMovie_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            string movieTitle = TitleTextBox.Text;

            ValidationResult movieTitleResult = new TextBoxLengthRule().Validate(movieTitle, null);
            if (!movieTitleResult.IsValid)
            {
                _notifier.ShowError("Please enter a movie title!");
                return;
            }

            int id = _updateMovie.Id;
            string movieGenre = GenreTextBox.Text;
            string movieDescription = DescriptionTextBox.Text;
            int movieRating = MovieRatingBar.Value;

            MovieDto updateMovie = new MovieDto(id, movieTitle, movieGenre, movieDescription, movieRating, _updateMovie.Watched, _updateMovie.Sockets);

            _movieService.OnMovieUpdateFinished += _onMovieUpdateFinished;
            _movieService.UpdateMovie(updateMovie);
        }

        private void _onMovieUpdateFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_onMovieUpdateFinished was successful {0}", success));

            _movieService.OnMovieUpdateFinished -= _onMovieUpdateFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new movie!");

                _movieService.OnMovieDownloadFinished += _onMovieDownloadFinished;
                _movieService.LoadMovieList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating movie failed!\n{0}", response));
            }
        }

        private void _onMovieDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMovieDownloadFinished with model {0} was successful {1}", movieList, success));

            _movieService.OnMovieDownloadFinished -= _onMovieDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }
    }
}
