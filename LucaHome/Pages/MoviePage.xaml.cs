using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using LucaHome.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private readonly MovieAddPage _movieAddPage;

        public MoviePage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _movieService = MovieService.Instance;

            _movieAddPage = new MovieAddPage(_navigationService);

            InitializeComponent();

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
                return string.Empty;
            }
            set
            {
                _movieSearchKey = value;
                OnPropertyChanged("MovieSearchKey");

                if (_movieSearchKey != string.Empty)
                {
                    setList(_movieService.FoundMovies(_movieSearchKey));
                }
                else
                {
                    setList(_movieService.MovieList);
                }
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

            setList(_movieService.MovieList);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _movieService.OnMovieDownloadFinished -= _movieListDownloadFinished;
            _movieService.OnMovieStartFinished -= _movieStartFinished;
            _movieService.OnMovieDeleteFinished -= _onMovieDeleteFinished;
        }

        private void setList(IList<MovieDto> movieList)
        {
            _logger.Debug("setList");

            MovieList.Items.Clear();

            foreach (MovieDto entry in movieList)
            {
                MovieList.Items.Add(entry);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                string movieTitle = (String)senderButton.Tag;
                _movieService.StartMovieOnPc(movieTitle);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonAdd_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.Navigate(_movieAddPage);
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _movieService.LoadMovieList();
        }

        private void SearchMovieTextBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            _logger.Debug(string.Format("SearchMovieTextBox_KeyDown with sender {0} and keyEventArgs: {1}", sender, keyEventArgs));

            if (keyEventArgs.Key == Key.Enter && keyEventArgs.IsDown)
            {
                MovieSearchKey = SearchMovieTextBox.Text;
            }
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

        private void ButtonDeleteMovie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonUpdateMovie_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int movieId = (int)senderButton.Tag;
                MovieDto deleteMovie = _movieService.GetById(movieId);
                _logger.Warning(string.Format("Asking for deleting movie {0}!", deleteMovie));

                DeleteDialog movieDeleteDialog = new DeleteDialog("Delete movie?",
                    string.Format("Movie: {0}\nGenre: {1}\nDescription: {2}", deleteMovie.Title, deleteMovie.Genre, deleteMovie.Description));
                movieDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                movieDeleteDialog.ShowDialog();

                var confirmDelete = movieDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    _movieService.OnMovieDeleteFinished += _onMovieDeleteFinished;
                    _movieService.DeleteMovie(deleteMovie);
                }
            }
        }

        private void _movieListDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            _logger.Debug(string.Format("_movieListDownloadFinished with model {0} was successful: {1}", movieList, success));

            setList(_movieService.MovieList);
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

        private void _onMovieDeleteFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_onMovieDeleteFinished with response {0} was successful: {1}", response, success));
            _movieService.OnMovieDeleteFinished -= _onMovieDeleteFinished;
            _movieService.LoadMovieList();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
