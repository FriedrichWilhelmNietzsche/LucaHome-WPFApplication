using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Common.Interfaces;

namespace Data.Services
{
    public delegate void BirthdayDownloadEventHandler(IList<BirthdayDto> birthdayList, bool success, string response);
    public delegate void BirthdayAddEventHandler(bool success, string response);
    public delegate void BirthdayUpdateEventHandler(bool success, string response);
    public delegate void BirthdayDeleteEventHandler(bool success, string response);

    public class BirthdayService
    {
        private const string TAG = "BirthdayService";
        private readonly Logger _logger;

        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly IJsonDataConverter<BirthdayDto> _jsonDataToBirthdayConverter;

        private static BirthdayService _instance = null;
        private static readonly object _padlock = new object();

        private IList<BirthdayDto> _birthdayList = new List<BirthdayDto>();

        private Timer _downloadTimer;

        BirthdayService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToBirthdayConverter = new JsonDataToBirthdayConverter();

            _downloadController.OnDownloadFinished += _birthdayDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event BirthdayDownloadEventHandler OnBirthdayDownloadFinished;
        private void publishOnBirthdayDownloadFinished(IList<BirthdayDto> birthdayList, bool success, string response)
        {
            OnBirthdayDownloadFinished?.Invoke(birthdayList, success, response);
        }

        public event BirthdayAddEventHandler OnBirthdayAddFinished;
        private void publishOnBirthdayAddFinished(bool success, string response)
        {
            OnBirthdayAddFinished?.Invoke(success, response);
        }

        public event BirthdayUpdateEventHandler OnBirthdayUpdateFinished;
        private void publishOnBirthdayUpdateFinished(bool success, string response)
        {
            OnBirthdayUpdateFinished?.Invoke(success, response);
        }

        public event BirthdayDeleteEventHandler OnBirthdayDeleteFinished;
        private void publishOnBirthdayDeleteFinished(bool success, string response)
        {
            OnBirthdayDeleteFinished?.Invoke(success, response);
        }

        public static BirthdayService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new BirthdayService();
                    }

                    return _instance;
                }
            }
        }

        public IList<BirthdayDto> BirthdayList
        {
            get
            {
                return _birthdayList;
            }
        }

        public BirthdayDto GetById(int id)
        {
            BirthdayDto foundBirthday = _birthdayList
                        .Where(birthday => birthday.Id == id)
                        .Select(birthday => birthday)
                        .FirstOrDefault();

            return foundBirthday;
        }

        public IList<BirthdayDto> FoundBirthdays(string searchKey)
        {
            List<BirthdayDto> foundBirthdays = _birthdayList
                        .Where(birthday =>
                            birthday.Name.Contains(searchKey)
                            || birthday.Id.ToString().Contains(searchKey)
                            || birthday.Birthday.ToString().Contains(searchKey)
                            || birthday.Age.ToString().Contains(searchKey))
                        .Select(birthday => birthday)
                        .ToList();

            return foundBirthdays;
        }

        public void LoadBirthdayList()
        {
            _logger.Debug("LoadBirthdayList");
            loadBirthdayListAsync();
        }

        public void AddBirthday(BirthdayDto newBirthday)
        {
            _logger.Debug(string.Format("AddBirthday: Adding new birthday {0}", newBirthday));
            addBirthdayAsync(newBirthday);
        }

        public void UpdateBirthday(BirthdayDto updateBirthday)
        {
            _logger.Debug(string.Format("UpdateBirthday: Updating birthday {0}", updateBirthday));
            updateBirthdayAsync(updateBirthday);
        }

        public void DeleteBirthday(BirthdayDto deleteBirthday)
        {
            _logger.Debug(string.Format("DeleteBirthday: Deleting birthday {0}", deleteBirthday));
            deleteBirthdayAsync(deleteBirthday);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadBirthdayListAsync();
        }

        private async Task loadBirthdayListAsync()
        {
            _logger.Debug("loadBirthdayListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnBirthdayDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_BIRTHDAYS.Action;
            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Birthday);
        }

        private async Task addBirthdayAsync(BirthdayDto newBirthday)
        {
            _logger.Debug(string.Format("addBirthdayAsync: Adding new birthday {0}", newBirthday));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnBirthdayAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newBirthday.CommandAdd);

            _downloadController.OnDownloadFinished += _birthdayAddFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.BirthdayAdd);
        }

        private async Task updateBirthdayAsync(BirthdayDto updateBirthday)
        {
            _logger.Debug(string.Format("updateBirthdayAsync: Updating birthday {0}", updateBirthday));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnBirthdayUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateBirthday.CommandUpdate);

            _downloadController.OnDownloadFinished += _birthdayUpdateFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.BirthdayUpdate);
        }

        private async Task deleteBirthdayAsync(BirthdayDto deleteBirthday)
        {
            _logger.Debug(string.Format("deleteBirthdayAsync: Deleting birthday {0}", deleteBirthday));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnBirthdayDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteBirthday.CommandDelete);

            _downloadController.OnDownloadFinished += _birthdayDeleteFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.BirthdayDelete);
        }

        private void _birthdayDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_birthdayDownloadFinished");

            if (downloadType != DownloadType.Birthday)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnBirthdayDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnBirthdayDownloadFinished(null, false, response);
                return;
            }

            IList<BirthdayDto> birthdayList = _jsonDataToBirthdayConverter.GetList(response);
            if (birthdayList == null)
            {
                _logger.Error("Converted birthdayList is null!");

                publishOnBirthdayDownloadFinished(null, false, response);
                return;
            }

            _birthdayList = birthdayList;

            publishOnBirthdayDownloadFinished(_birthdayList, true, response);
        }

        private void _birthdayAddFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_birthdayAddFinished");

            if (downloadType != DownloadType.BirthdayAdd)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _birthdayAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnBirthdayAddFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                publishOnBirthdayAddFinished(false, response);
                return;
            }

            publishOnBirthdayAddFinished(true, response);

            loadBirthdayListAsync();
        }

        private void _birthdayUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_birthdayUpdateinished");

            if (downloadType != DownloadType.BirthdayUpdate)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _birthdayUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnBirthdayUpdateFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Updating was not successful!");

                publishOnBirthdayUpdateFinished(false, response);
                return;
            }

            publishOnBirthdayUpdateFinished(true, response);

            loadBirthdayListAsync();
        }

        private void _birthdayDeleteFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            _logger.Debug("_birthdayDeleteFinished");

            if (downloadType != DownloadType.BirthdayDelete)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _birthdayDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnBirthdayDeleteFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Deleting was not successful!");

                publishOnBirthdayDeleteFinished(false, response);
                return;
            }

            publishOnBirthdayDeleteFinished(true, response);

            loadBirthdayListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _birthdayDownloadFinished;
            _downloadController.OnDownloadFinished -= _birthdayAddFinished;
            _downloadController.OnDownloadFinished -= _birthdayUpdateFinished;
            _downloadController.OnDownloadFinished -= _birthdayDeleteFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
