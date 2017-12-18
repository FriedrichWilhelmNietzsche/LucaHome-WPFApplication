using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Timers;

namespace Data.Services
{
    public delegate void MovieDownloadEventHandler(IList<MovieDto> movieList, bool success, string response);
    public delegate void MovieStartEventHandler(bool success, string response);
    public delegate void MovieUpdateEventHandler(bool success, string response);

    public class MovieService
    {
        private const string TAG = "MovieService";
        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly DownloadController _downloadController;
        private readonly LocalDriveController _localDriveController;

        private static MovieService _instance = null;
        private static readonly object _padlock = new object();

        private IList<MovieDto> _movieList = new List<MovieDto>();
        private DriveInfo _videothekDrive;

        private Timer _downloadTimer;

        MovieService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _movieDownloadFinished;

            _localDriveController = new LocalDriveController();
            _videothekDrive = _localDriveController.GetVideothekDrive();
            if (_videothekDrive == null)
            {
                Logger.Instance.Error(TAG, "Found no videothek drive!");
            }

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event MovieDownloadEventHandler OnMovieDownloadFinished;
        private void publishOnMovieDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            OnMovieDownloadFinished?.Invoke(movieList, success, response);
        }

        public event MovieStartEventHandler OnMovieStartFinished;
        private void publishOnMovieStartFinished(bool success, string response)
        {
            OnMovieStartFinished?.Invoke(success, response);
        }

        public event MovieUpdateEventHandler OnMovieUpdateFinished;
        private void publishOnMovieUpdateFinished(bool success, string response)
        {
            OnMovieUpdateFinished?.Invoke(success, response);
        }

        public static MovieService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MovieService();
                    }

                    return _instance;
                }
            }
        }

        public IList<MovieDto> MovieList
        {
            get
            {
                return _movieList.OrderBy(movie => movie.Title).ToList();
            }
        }

        public MovieDto GetById(int id)
        {
            MovieDto foundMovie = _movieList
                        .Where(movie => movie.Id == id)
                        .Select(movie => movie)
                        .FirstOrDefault();

            return foundMovie;
        }

        public IList<MovieDto> FoundMovies(string searchKey)
        {
            if (searchKey == string.Empty)
            {
                return _movieList;
            }

            List<MovieDto> foundMovies = _movieList
                        .Where(movie =>
                            movie.Title.Contains(searchKey)
                            || movie.Genre.Contains(searchKey)
                            || movie.Description.Contains(searchKey)
                            || movie.RatingString.Contains(searchKey)
                            || movie.Watched.ToString().Contains(searchKey))
                        .Select(movie => movie)
                        .OrderBy(movie => movie.Title)
                        .ToList();

            return foundMovies;
        }

        public void LoadMovieList()
        {
            loadMovieListAsync();
        }

        public void StartMovieOnPc(MovieDto movie)
        {
            startMovieOnPc(movie.Title);
        }

        public void StartMovieOnPc(string movieTitle)
        {
            startMovieOnPc(movieTitle);
        }

        public void UpdateMovie(MovieDto updateMovie)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateMovie: updating movie {0}", updateMovie));
            updateMovieAsync(updateMovie);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadMovieListAsync();
        }

        private void startMovieOnPc(string movieTitle)
        {
            if (_videothekDrive == null)
            {
                Logger.Instance.Error(TAG, "VideothekDrive is null! Trying to find...");
                _videothekDrive = _localDriveController.GetVideothekDrive();

                if (_videothekDrive == null)
                {
                    Logger.Instance.Error(TAG, "VideothekDrive is still null! Aborting launch...");
                    publishOnMovieStartFinished(false, "No movie drive found! Please check your attached storages!");
                    return;
                }
            }

            string moviePathString = string.Format("{0}{1}{2}", _videothekDrive.Name, "Filme\\", movieTitle);
            DirectoryInfo moviePath = new DirectoryInfo(moviePathString);

            string[] extensionArray = new string[] { ".mkv", ".avi", ".mp4" };
            FileInfo[] movieFiles = _localDriveController.ReadFilesInDir(moviePath, extensionArray);
            if (movieFiles.Length == 0)
            {
                Logger.Instance.Error(TAG, string.Format("Found no files for movie {0} in directory {1}", movieTitle, moviePath));
                publishOnMovieStartFinished(false, "No movie file found!");
            }
            else if (movieFiles.Length == 1)
            {
                string path = string.Format("{0}\\{1}", moviePathString, movieFiles[0].Name);
                Logger.Instance.Debug(TAG, string.Format("Opening {0} with associated programm", path));
                Process.Start(@path);
                publishOnMovieStartFinished(true, string.Empty);
            }
            else
            {
                Logger.Instance.Error(TAG, string.Format("Please provide only one file in the given directory! Found {0} files in {1}", movieFiles.Length, moviePath));
                publishOnMovieStartFinished(false, "Too many files found!");
            }
        }

        private async Task loadMovieListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMovieDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_MOVIES.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Movie);
        }

        private async Task updateMovieAsync(MovieDto updateMovie)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMovieUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateMovie.CommandUpdate);

            _downloadController.OnDownloadFinished += _movieUpdateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MovieUpdate);
        }

        private void _movieDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.Movie)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMovieDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnMovieDownloadFinished(null, false, response);
                return;
            }

            IList<MovieDto> movieList = JsonDataToMovieConverter.Instance.GetList(response);
            if (movieList == null)
            {
                Logger.Instance.Error(TAG, "Converted movieList is null!");
                publishOnMovieDownloadFinished(null, false, response);
                return;
            }

            _movieList = movieList.OrderBy(x => x.Title).ToList();
            publishOnMovieDownloadFinished(_movieList, true, response);
        }

        private void _movieUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MovieUpdate)
            {
                Logger.Instance.Debug(TAG, string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _movieUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMovieUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Updating was not successful!");
                publishOnMovieUpdateFinished(false, response);
                return;
            }

            publishOnMovieUpdateFinished(true, response);
            loadMovieListAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _movieDownloadFinished;
            _downloadController.OnDownloadFinished -= _movieUpdateFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
