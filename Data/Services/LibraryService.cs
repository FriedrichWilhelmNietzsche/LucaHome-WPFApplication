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
    public class LibraryService
    {
        public delegate void MagazinListDownloadEventHandler(IList<MagazinDirDto> magazinList, bool success, string response);

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
            _magazineDir = _libraryDrive.Name + "Magazine";

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

        public void LoadMagazinList()
        {
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

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _reloadTimer.Elapsed -= _reloadTimer_Elapsed;
            _reloadTimer.AutoReset = false;
            _reloadTimer.Stop();
        }
    }
}
