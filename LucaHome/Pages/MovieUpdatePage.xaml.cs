using Common.Dto;
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
using System.ComponentModel;

namespace LucaHome.Pages
{
    public partial class MovieUpdatePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MovieUpdatePage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private MovieDto _updateMovie;

        public MovieUpdatePage(NavigationService navigationService, MovieDto updateMovie)
        {
            _navigationService = navigationService;

            _updateMovie = updateMovie;

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
                    notificationLifetime: TimeSpan.FromSeconds(2),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(2));

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

        public string MovieTitle
        {
            get
            {
                return _updateMovie.Title;
            }
            set
            {
                _updateMovie.Title = value;
                OnPropertyChanged("MovieTitle");
            }
        }

        public string MovieGenre
        {
            get
            {
                return _updateMovie.Genre;
            }
            set
            {
                _updateMovie.Genre = value;
                OnPropertyChanged("MovieGenre");
            }
        }

        public string MovieDescription
        {
            get
            {
                return _updateMovie.Description;
            }
            set
            {
                _updateMovie.Description = value;
                OnPropertyChanged("MovieDescription");
            }
        }

        public int MovieRating
        {
            get
            {
                return _updateMovie.Rating;
            }
            set
            {
                _updateMovie.Rating = value;
                OnPropertyChanged("MovieRating");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MovieService.Instance.OnMovieDownloadFinished -= _onMovieDownloadFinished;
            MovieService.Instance.OnMovieUpdateFinished -= _onMovieUpdateFinished;
        }

        private void UpdateMovie_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            MovieService.Instance.OnMovieUpdateFinished += _onMovieUpdateFinished;
            MovieService.Instance.UpdateMovie(_updateMovie);
        }

        private void _onMovieUpdateFinished(bool success, string response)
        {
            MovieService.Instance.OnMovieUpdateFinished -= _onMovieUpdateFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new movie!");

                MovieService.Instance.OnMovieDownloadFinished += _onMovieDownloadFinished;
                MovieService.Instance.LoadMovieList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating movie failed!\n{0}", response));
            }
        }

        private void _onMovieDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            MovieService.Instance.OnMovieDownloadFinished -= _onMovieDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
