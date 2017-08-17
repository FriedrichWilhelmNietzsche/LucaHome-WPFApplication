using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace Data.Services
{
    public class SpecialicedBookService
    {
        public delegate void SpecialicedBookListDownloadEventHandler(IList<string> bookList, bool success, string response);

        private const string TAG = "SpecialicedBookService";
        private readonly Logger _logger;

        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly LocalDriveController _localDriveController;

        private static SpecialicedBookService _instance = null;
        private static readonly object _padlock = new object();

        private DriveInfo _libraryDrive;
        private string _specialicedBookDir = string.Empty;
        private IList<string> _bookList = new List<string>();

        private Timer _reloadTimer;

        SpecialicedBookService()
        {
            _logger = new Logger(TAG);

            _localDriveController = new LocalDriveController();

            _libraryDrive = _localDriveController.GetLibraryDrive();
            _specialicedBookDir = _libraryDrive.Name + "Books\\Sachbücher";

            _reloadTimer = new Timer(TIMEOUT);
            _reloadTimer.Elapsed += _reloadTimer_Elapsed;
            _reloadTimer.AutoReset = true;
            _reloadTimer.Start();
        }

        public event SpecialicedBookListDownloadEventHandler OnSpecialicedBookListDownloadEventHandler;
        private void publishOnSpecialicedBookListDownloadEventHandler(IList<string> bookList, bool success, string response)
        {
            OnSpecialicedBookListDownloadEventHandler?.Invoke(bookList, success, response);
        }

        public static SpecialicedBookService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new SpecialicedBookService();
                    }

                    return _instance;
                }
            }
        }

        public string SpecialicedBookDir
        {
            get
            {
                return _specialicedBookDir;
            }
        }

        public IList<string> BookList
        {
            get
            {
                return _bookList;
            }
        }

        public List<string> FoundBooks(string searchKey)
        {
            List<string> foundBooks = _bookList
                        .Where(entry =>
                            entry.Contains(searchKey))
                        .Select(book => book)
                        .ToList();

            return foundBooks;
        }

        public void LoadBookList()
        {
            string[] extensionArray = new string[] { ".pdf", ".epub" };

            _bookList = _localDriveController.ReadFileNamesInDir(_specialicedBookDir, extensionArray);

            _bookList = _bookList
                .Select(fileName => fileName.Replace(_specialicedBookDir, "").Replace("\\", ""))
                .OrderBy(fileName => fileName)
                .ToArray();

            publishOnSpecialicedBookListDownloadEventHandler(_bookList, true, "Success");
        }

        private void _reloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_reloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            LoadBookList();
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
