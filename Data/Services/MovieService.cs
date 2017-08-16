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
using Common.Interfaces;

namespace Data.Services
{
    public delegate void MovieDownloadEventHandler(IList<MovieDto> movieList, bool success, string response);
    public delegate void MovieStartEventHandler(bool success, string response);
    public delegate void MovieUpdateEventHandler(bool success, string response);

    public class MovieService
    {
        private const string TAG = "MovieService";
        private readonly Logger _logger;

        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly IJsonDataConverter<MovieDto> _jsonDataToMovieConverter;
        private readonly LocalDriveController _localDriveController;

        private static MovieService _instance = null;
        private static readonly object _padlock = new object();

        private IList<MovieDto> _movieList = new List<MovieDto>();
        private DriveInfo _videothekDrive;

        private Timer _downloadTimer;

        MovieService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToMovieConverter = new JsonDataToMovieConverter();
            _localDriveController = new LocalDriveController();

            _videothekDrive = _localDriveController.GetVideothekDrive();

            _downloadController.OnDownloadFinished += _movieDownloadFinished;

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
            _logger.Debug("LoadMovieList");
            loadMovieListAsync();
        }

        public void StartMovieOnPc(MovieDto movie)
        {
            _logger.Debug(string.Format("Start movie {0}", movie));

            startMovieOnPc(movie.Title);
        }

        public void StartMovieOnPc(string movieTitle)
        {
            _logger.Debug(string.Format("Start movie {0}", movieTitle));

            startMovieOnPc(movieTitle);
        }

        public void UpdateMovie(MovieDto updateMovie)
        {
            _logger.Debug(string.Format("UpdateMovie: updating movie {0}", updateMovie));
            updateMovieAsync(updateMovie);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadMovieListAsync();
        }

        private void startMovieOnPc(string movieTitle)
        {
            _logger.Debug(string.Format("startMovieOnPc with title {0}", movieTitle));

            string moviePathString = string.Format("{0}{1}{2}", _videothekDrive.Name, "Filme\\", movieTitle);
            DirectoryInfo moviePath = new DirectoryInfo(moviePathString);

            string[] extensionArray = new string[] { ".mkv", ".avi", ".mp4" };
            FileInfo[] movieFiles = _localDriveController.ReadFilesInDir(moviePath, extensionArray);
            if (movieFiles.Length == 0)
            {
                _logger.Error(string.Format("Found no files for movie {0} in directory {1}", movieTitle, moviePath));
                publishOnMovieStartFinished(false, "No movie file found!");
            }
            else if (movieFiles.Length == 1)
            {
                string path = string.Format("{0}\\{1}", moviePathString, movieFiles[0].Name);
                _logger.Debug(string.Format("Opening {0} with associated programm", path));
                Process.Start(@path);
                publishOnMovieStartFinished(true, string.Empty);
            }
            else
            {
                _logger.Error(string.Format("Please provide only one file in the given directory! Found {0} files in {1}", movieFiles.Length, moviePath));
                publishOnMovieStartFinished(false, "Too many files found!");
            }
        }

        private async Task loadMovieListAsync()
        {
            _logger.Debug("loadMovieListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnMovieDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_MOVIES.Action;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Movie);
        }

        private async Task updateMovieAsync(MovieDto updateMovie)
        {
            _logger.Debug(string.Format("updateMovieAsync: updating movie {0}", updateMovie));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnMovieUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateMovie.CommandUpdate);

            _downloadController.OnDownloadFinished += _movieUpdateFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MovieUpdate);
        }

        private void _movieDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_movieDownloadFinished");

            if (downloadType != DownloadType.Movie)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnMovieDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnMovieDownloadFinished(null, false, response);
                return;
            }

            IList<MovieDto> movieList = _jsonDataToMovieConverter.GetList(response);
            if (movieList == null)
            {
                _logger.Error("Converted movieList is null!");

                publishOnMovieDownloadFinished(null, false, response);
                return;
            }

            _movieList = movieList;

            publishOnMovieDownloadFinished(_movieList, true, response);
        }

        private void _movieUpdateFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_movieUpdateFinished");

            if (downloadType != DownloadType.MovieUpdate)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _movieUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnMovieUpdateFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Updating was not successful!");

                publishOnMovieUpdateFinished(false, response);
                return;
            }

            publishOnMovieUpdateFinished(true, response);

            loadMovieListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _movieDownloadFinished;
            _downloadController.OnDownloadFinished -= _movieUpdateFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
