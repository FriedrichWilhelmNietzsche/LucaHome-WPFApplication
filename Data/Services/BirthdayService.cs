using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToBirthdayConverter _jsonDataToBirthdayConverter;

        private static BirthdayService _instance = null;
        private static readonly object _padlock = new object();

        private IList<BirthdayDto> _birthdayList = new List<BirthdayDto>();

        BirthdayService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToBirthdayConverter = new JsonDataToBirthdayConverter();
        }

        public event BirthdayDownloadEventHandler OnBirthdayDownloadFinished;
        public event BirthdayAddEventHandler OnBirthdayAddFinished;
        public event BirthdayUpdateEventHandler OnBirthdayUpdateFinished;
        public event BirthdayDeleteEventHandler OnBirthdayDeleteFinished;

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

        private async Task loadBirthdayListAsync()
        {
            _logger.Debug("loadBirthdayListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnBirthdayDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_BIRTHDAYS.Action;
            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            _downloadController.OnDownloadFinished += _birthdayDownloadFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.Birthday);
        }

        private async Task addBirthdayAsync(BirthdayDto newBirthday)
        {
            _logger.Debug(string.Format("addBirthdayAsync: Adding new birthday {0}", newBirthday));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnBirthdayAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newBirthday.CommandAdd);

            _downloadController.OnDownloadFinished += _birthdayAddFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.BirthdayAdd);
        }

        private async Task updateBirthdayAsync(BirthdayDto updateBirthday)
        {
            _logger.Debug(string.Format("updateBirthdayAsync: Updating birthday {0}", updateBirthday));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnBirthdayUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateBirthday.CommandUpdate);

            _downloadController.OnDownloadFinished += _birthdayUpdateFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.BirthdayUpdate);
        }

        private async Task deleteBirthdayAsync(BirthdayDto deleteBirthday)
        {
            _logger.Debug(string.Format("deleteBirthdayAsync: Deleting birthday {0}", deleteBirthday));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnBirthdayDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteBirthday.CommandDelete);

            _downloadController.OnDownloadFinished += _birthdayDeleteFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.BirthdayDelete);
        }

        private void _birthdayDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_birthdayDownloadFinished");

            if (downloadType != DownloadType.Birthday)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _birthdayDownloadFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnBirthdayDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnBirthdayDownloadFinished(null, false, response);
                return;
            }

            IList<BirthdayDto> birthdayList = _jsonDataToBirthdayConverter.GetList(response);
            if (birthdayList == null)
            {
                _logger.Error("Converted birthdayList is null!");

                OnBirthdayDownloadFinished(null, false, response);
                return;
            }

            _birthdayList = birthdayList;

            OnBirthdayDownloadFinished(_birthdayList, true, response);
        }

        private void _birthdayAddFinished(string response, bool success, DownloadType downloadType)
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

                OnBirthdayAddFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                OnBirthdayAddFinished(false, response);
                return;
            }

            OnBirthdayAddFinished(true, response);
        }

        private void _birthdayUpdateFinished(string response, bool success, DownloadType downloadType)
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

                OnBirthdayUpdateFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Updating was not successful!");

                OnBirthdayUpdateFinished(false, response);
                return;
            }

            OnBirthdayUpdateFinished(true, response);
        }

        private void _birthdayDeleteFinished(string response, bool success, DownloadType downloadType)
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

                OnBirthdayDeleteFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Deleting was not successful!");

                OnBirthdayDeleteFinished(false, response);
                return;
            }

            OnBirthdayDeleteFinished(true, response);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _birthdayDownloadFinished;
            _downloadController.OnDownloadFinished -= _birthdayAddFinished;
            _downloadController.OnDownloadFinished -= _birthdayUpdateFinished;
            _downloadController.OnDownloadFinished -= _birthdayDeleteFinished;

            _downloadController.Dispose();
        }
    }
}
