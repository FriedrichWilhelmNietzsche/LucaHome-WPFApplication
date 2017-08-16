using Common.Dto;
using Common.Tools;
using Data.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Media.Imaging;

namespace Data.Services
{
    public class SeriesService
    {
        public delegate void SeriesListDownloadEventHandler(IList<SeriesDto> seriesList, bool success, string response);

        private const string TAG = "SeriesService";
        private readonly Logger _logger;

        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly LocalDriveController _localDriveController;

        private static SeriesService _instance = null;
        private static readonly object _padlock = new object();

        private DriveInfo _seriesDrive;
        private string _seriesDir = string.Empty;
        private IList<SeriesDto> _seriesList = new List<SeriesDto>();

        private Timer _reloadTimer;

        SeriesService()
        {
            _logger = new Logger(TAG);

            _localDriveController = new LocalDriveController();

            _seriesDrive = _localDriveController.GetVideothekDrive();
            _seriesDir = _seriesDrive.Name + "Serien";

            _reloadTimer = new Timer(TIMEOUT);
            _reloadTimer.Elapsed += _reloadTimer_Elapsed;
            _reloadTimer.AutoReset = true;
            _reloadTimer.Start();
        }

        public event SeriesListDownloadEventHandler OnSeriesListDownloadFinished;
        private void publishOnSeriesListDownloadFinished(IList<SeriesDto> seriesList, bool success, string response)
        {
            OnSeriesListDownloadFinished?.Invoke(seriesList, success, response);
        }

        public static SeriesService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new SeriesService();
                    }

                    return _instance;
                }
            }
        }

        public string SeriesDir
        {
            get
            {
                return _seriesDir;
            }
        }

        public IList<SeriesDto> SeriesList
        {
            get
            {
                return _seriesList;
            }
        }

        public SeriesDto GetByName(string name)
        {
            SeriesDto foundSeries = _seriesList
                        .Where(series => series.SeriesName.Contains(name))
                        .Select(series => series)
                        .FirstOrDefault();

            return foundSeries;
        }

        public IList<SeriesDto> FoundSeries(string searchKey)
        {
            List<SeriesDto> foundSeries = _seriesList
                        .Where(series =>
                            series.SeriesName.Contains(searchKey)
                            || series.Icon.ToString().Contains(searchKey))
                        .Select(series => series)
                        .ToList();

            return foundSeries;
        }

        public void LoadSeriesList()
        {
            string[] extensionArray = new string[] { ".mp4", ".mkv" };
            _seriesList.Clear();

            string[] seriesList = _localDriveController.ReadDirInDir(_seriesDir);

            for (int seriesIndex = 0; seriesIndex < seriesList.Length; seriesIndex++)
            {
                string serieName = seriesList[seriesIndex];

                string[] seasonStringList = _localDriveController.ReadDirInDir(serieName);
                SeasonDto[] seasonList = new SeasonDto[seasonStringList.Length];

                for (int seasonIndex = 0; seasonIndex < seasonStringList.Length; seasonIndex++)
                {
                    string season = seasonStringList[seasonIndex];

                    string[] episodeList = _localDriveController.ReadFileNamesInDir(season, extensionArray);

                    season = season.Replace(serieName, "").Replace(_seriesDir, "").Replace("\\", "");
                    SeasonDto seasonDto = new SeasonDto(season, episodeList);
                    seasonList[seasonIndex] = seasonDto;
                }

                Uri iconUri = new Uri(string.Format("{0}\\_icon.jpg", serieName), UriKind.Absolute);
                BitmapImage icon = new BitmapImage(iconUri);

                serieName = serieName.Replace(_seriesDir, "").Replace("\\", "");

                SeriesDto newEntry = new SeriesDto(
                    serieName,
                    seasonList,
                    icon);
                _seriesList.Add(newEntry);
            }

            _seriesList = _seriesList
                .OrderBy(entry => entry.SeriesName)
                .ToArray();

            publishOnSeriesListDownloadFinished(_seriesList, true, "Success");
        }

        private void _reloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_reloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            LoadSeriesList();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _reloadTimer.Elapsed -= _reloadTimer_Elapsed;
            _reloadTimer.AutoReset = false;
            _reloadTimer.Stop();
        }
    }
}
