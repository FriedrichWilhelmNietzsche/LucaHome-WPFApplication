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
    public partial class MovieAddPage : Page
    {
        private const string TAG = "MovieAddPage";
        private readonly Logger _logger;

        private readonly MovieService _movieService;
        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private string _movieTitle = string.Empty;
        private string _movieGenre = string.Empty;
        private string _movieDescription = string.Empty;
        private int _movieRating = 0;

        public MovieAddPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _movieService = MovieService.Instance;
            _navigationService = navigationService;

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

            TitleTextBox.Text = _movieTitle;
            GenreTextBox.Text = _movieGenre;
            DescriptionTextBox.Text = _movieDescription;
            MovieRatingBar.Value = _movieRating;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _movieService.OnMovieAddFinished -= _onMovieAddFinished;
            _movieService.OnMovieDownloadFinished -= _onMovieDownloadFinished;
        }

        private void SaveMovie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("SaveMovie_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            string movieTitle = TitleTextBox.Text;

            ValidationResult movieTitleResult = new TextBoxLengthRule().Validate(movieTitle, null);
            if (!movieTitleResult.IsValid)
            {
                _notifier.ShowError("Please enter a movie title!");
                return;
            }

            int id = _movieService.MovieList.Count;
            string movieGenre = GenreTextBox.Text;
            string movieDescription = DescriptionTextBox.Text;
            int movieRating = MovieRatingBar.Value;

            MovieDto newMovie = new MovieDto(id, movieTitle, movieGenre, movieDescription, movieRating, 0, new WirelessSocketDto[] { });

            _movieService.OnMovieAddFinished += _onMovieAddFinished;
        }

        private void _onMovieAddFinished(bool success)
        {
            _logger.Debug(string.Format("_onMovieAddFinished was successful {0}", success));

            _movieService.OnMovieAddFinished -= _onMovieAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new movie!");

                _movieService.OnMovieDownloadFinished += _onMovieDownloadFinished;
                _movieService.LoadMovieList();
            }
            else
            {
                _notifier.ShowError("Adding movie failed!");
            }
        }

        private void _onMovieDownloadFinished(IList<MovieDto> movieList, bool success)
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
