using Common.Dto;
using Common.Tools;
using Data.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Media.Imaging;

namespace Data.Services
{
    public class LibraryService
    {
        public delegate void MagazinListDownloadEventHandler(IList<MagazinDirDto> magazinList, bool success, string response);
        public delegate void LibraryServiceErrorEventHandler(string error);

        private const string TAG = "LibraryService";
        private readonly Logger _logger;

        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly LocalDriveController _localDriveController;

        private static LibraryService _instance = null;
        private static readonly object _padlock = new object();

        private DriveInfo _libraryDrive;
        private string _magazineDir = string.Empty;
        private IList<MagazinDirDto> _magazinList = new List<MagazinDirDto>();

        private Timer _reloadTimer;

        LibraryService()
        {
            _logger = new Logger(TAG);

            _localDriveController = new LocalDriveController();

            _libraryDrive = _localDriveController.GetLibraryDrive();
            if (_libraryDrive == null)
            {
                _logger.Error("Found no library drive!");
            }
            else
            {
                _magazineDir = _libraryDrive.Name + "Magazine";
            }

            _reloadTimer = new Timer(TIMEOUT);
            _reloadTimer.Elapsed += _reloadTimer_Elapsed;
            _reloadTimer.AutoReset = true;
            _reloadTimer.Start();
        }

        public event MagazinListDownloadEventHandler OnMagazinListDownloadFinished;
        private void publishOnMagazinListDownloadFinished(IList<MagazinDirDto> magazinList, bool success, string response)
        {
            OnMagazinListDownloadFinished?.Invoke(magazinList, success, response);
        }

        public event LibraryServiceErrorEventHandler OnLibraryServiceError;
        private void publishOnLibraryServiceErrord(string error)
        {
            OnLibraryServiceError?.Invoke(error);
        }

        public static LibraryService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new LibraryService();
                    }

                    return _instance;
                }
            }
        }

        public string MagazinDir
        {
            get
            {
                return _magazineDir;
            }
        }

        public IList<MagazinDirDto> MagazinList
        {
            get
            {
                return _magazinList;
            }
        }

        public MagazinDirDto GetByName(string name)
        {
            MagazinDirDto foundMagazin = _magazinList
                        .Where(magazin => magazin.DirName.Contains(name))
                        .Select(magazin => magazin)
                        .FirstOrDefault();

            return foundMagazin;
        }

        public IList<MagazinDirDto> FoundMagazins(string searchKey)
        {
            List<MagazinDirDto> foundMagazins = _magazinList
                        .Where(magazin =>
                            magazin.DirName.Contains(searchKey)
                            || magazin.Icon.ToString().Contains(searchKey)
                            || magazin.DirContent.Select(entry => entry).Contains(searchKey))
                        .Select(magazin => magazin)
                        .ToList();

            return foundMagazins;
        }

        public void StartReading(string directory, string title)
        {
            if (!directoryAvailable())
            {
                return;
            }

            if (directory == null || title == null
                || directory == string.Empty || title == string.Empty)
            {
                _logger.Error("Diretory or title is null or empty!");
                publishOnLibraryServiceErrord("Diretory or title is null or empty!");
                return;
            }

            string command = string.Format(@"{0}\{1}\{2}", _magazineDir, directory, title);
            Process.Start(command);
        }

        public void LoadMagazinList()
        {
            if (!directoryAvailable())
            {
                publishOnMagazinListDownloadFinished(null, false, "Library directory is not available!");
                return;
            }

            string[] extensionArray = new string[] { ".pdf", ".epub" };
            _magazinList.Clear();
            string[] magazinSubDirList = _localDriveController.ReadDirInDir(_magazineDir);

            foreach (string entry in magazinSubDirList)
            {
                string[] magazinList = _localDriveController.ReadFileNamesInDir(entry, extensionArray);
                Uri iconUri = new Uri(string.Format("{0}\\_icon.jpg", entry), UriKind.Absolute);

                magazinList = magazinList
                    .Select(fileName => fileName.Replace(entry, "").Replace("\\", ""))
                    .OrderBy(fileName => fileName)
                    .ToArray();
                string magazinName = entry.Replace(_magazineDir, "").Replace("\\", "");
                BitmapImage icon = new BitmapImage(iconUri);

                MagazinDirDto newEntry = new MagazinDirDto(
                    magazinName,
                    magazinList,
                    icon);
                _magazinList.Add(newEntry);
            }

            publishOnMagazinListDownloadFinished(_magazinList, true, "Success");
        }

        private void _reloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_reloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            LoadMagazinList();
        }

        private bool directoryAvailable()
        {
            if (_magazineDir == string.Empty)
            {
                _logger.Error("No directory for magazines! Trying to read again...");
                _libraryDrive = _localDriveController.GetLibraryDrive();

                if (_libraryDrive == null)
                {
                    _logger.Error("Found no library drive!");
                    publishOnLibraryServiceErrord("Found no library drive! Please check your attached storages!");
                    return false;
                }
                else
                {
                    _magazineDir = _libraryDrive.Name + "Magazine";
                }
            }

            return true;
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
