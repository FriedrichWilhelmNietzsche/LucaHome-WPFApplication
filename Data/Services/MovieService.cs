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
using System;

namespace Data.Services
{
    public delegate void MovieDownloadEventHandler(IList<MovieDto> movieList, bool success);
    public delegate void MovieStartEventHandler(bool success);
    public delegate void MovieAddEventHandler(bool success);

    public class MovieService
    {
        private const string TAG = "MovieService";
        private readonly Logger _logger;

        private readonly AppSettingsService _appSettingsService;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToMovieConverter _jsonDataToMovieConverter;
        private readonly LocalDriveController _localDriveController;
        private readonly WirelessSocketService _wirelessSocketService;

        private static MovieService _instance = null;
        private static readonly object _padlock = new object();

        private IList<MovieDto> _movieList = new List<MovieDto>();
        private DriveInfo _movieDrive;

        MovieService()
        {
            _logger = new Logger(TAG);

            _appSettingsService = AppSettingsService.Instance;
            _downloadController = new DownloadController();
            _jsonDataToMovieConverter = new JsonDataToMovieConverter();
            _localDriveController = new LocalDriveController();
            _wirelessSocketService = WirelessSocketService.Instance;

            _movieDrive = _localDriveController.GetMovieDrive();
        }

        public event MovieDownloadEventHandler OnMovieDownloadFinished;
        public event MovieStartEventHandler OnMovieStartFinished;
        public event MovieAddEventHandler OnMovieAddFinished;

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
                return _movieList;
            }
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

        public void AddMovie(MovieDto newMovie)
        {
            _logger.Debug(string.Format("AddMovie: add new movie {0}", newMovie));
            addMovieAsync(newMovie);
        }

        private void startMovieOnPc(string movieTitle)
        {
            _logger.Debug(string.Format("startMovieOnPc with title {0}", movieTitle));

            string moviePathString = string.Format("{0}{1}", _movieDrive.Name, movieTitle);
            DirectoryInfo moviePath = new DirectoryInfo(moviePathString);

            FileInfo[] movieFiles = _localDriveController.ReadFilesInDir(moviePath);
            if (movieFiles.Length == 0)
            {
                _logger.Error(string.Format("Found no files for movie {0} in directory {1}", movieTitle, moviePath));
                OnMovieStartFinished(false);
            }
            else if (movieFiles.Length == 1)
            {
                string path = string.Format("{0}{1}\\{2}", _movieDrive.Name, movieTitle, movieFiles[0].Name);
                _logger.Debug(string.Format("Opening {0} with associated programm", path));
                Process.Start(@path);
                OnMovieStartFinished(true);
            }
            else
            {
                _logger.Error(string.Format("Please provide only one file in the given directory! Found {0} files in {1}", movieFiles.Length, moviePath));
                OnMovieStartFinished(false);
            }
        }

        private async Task loadMovieListAsync()
        {
            _logger.Debug("loadMovieListAsync");

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnMovieDownloadFinished(null, false);
                return;
            }

            string requestUrl = "http://" + _appSettingsService.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_MOVIES.Action;

            _downloadController.OnDownloadFinished += _movieDownloadFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.Movie);
        }

        private async Task addMovieAsync(MovieDto newMovie)
        {
            _logger.Debug(string.Format("addMovieAsync: add new movie {0}", newMovie));

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnMovieAddFinished(false);
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _appSettingsService.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newMovie.CommandAdd);

            _downloadController.OnDownloadFinished += _movieAddFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.MovieAdd);
        }

        private void _movieDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_movieDownloadFinished");

            if (downloadType != DownloadType.Movie)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _movieDownloadFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnMovieDownloadFinished(null, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnMovieDownloadFinished(null, false);
                return;
            }

            IList<MovieDto> movieList = _jsonDataToMovieConverter.GetList(response, _wirelessSocketService.WirelessSocketList);
            if (movieList == null)
            {
                _logger.Error("Converted movieList is null!");

                OnMovieDownloadFinished(null, false);
                return;
            }

            _movieList = movieList;

            OnMovieDownloadFinished(_movieList, true);
        }

        private void _movieAddFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_movieAddFinished");

            if (downloadType != DownloadType.MovieAdd)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _movieAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnMovieAddFinished(false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnMovieAddFinished(false);
                return;
            }

            OnMovieAddFinished(true);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _movieDownloadFinished;
            _downloadController.OnDownloadFinished -= _movieAddFinished;

            _downloadController.Dispose();
        }
    }
}
