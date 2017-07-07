using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Services
{
    public delegate void BirthdayDownloadEventHandler(IList<BirthdayDto> birthdayList, bool success);

    public class BirthdayService
    {
        private const string TAG = "BirthdayService";
        private Logger _logger;

        private AppSettingsService _appSettingsService;
        private DownloadController _downloadController;
        private JsonDataToBirthdayConverter _jsonDataToBirthdayConverter;

        private static BirthdayService _instance = null;
        private static readonly object _padlock = new object();

        private IList<BirthdayDto> _birthdayList = new List<BirthdayDto>();

        BirthdayService()
        {
            _logger = new Logger(TAG);

            _appSettingsService = AppSettingsService.Instance;
            _downloadController = new DownloadController();
            _jsonDataToBirthdayConverter = new JsonDataToBirthdayConverter();
        }

        public event BirthdayDownloadEventHandler OnBirthdayDownloadFinished;

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

        public void LoadBirthdayList()
        {
            _logger.Debug("LoadBirthdayList");
            loadBirthdayListAsync();
        }

        private async Task loadBirthdayListAsync()
        {
            _logger.Debug("loadBirthdayListAsync");

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnBirthdayDownloadFinished(null, false);
                return;
            }

            string requestUrl = "http://" + _appSettingsService.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_BIRTHDAYS.Action;

            _downloadController.OnDownloadFinished += _birthdayDownloadFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.Birthday);
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

            if (response.Contains("Error"))
            {
                _logger.Error(response);

                OnBirthdayDownloadFinished(null, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnBirthdayDownloadFinished(null, false);
                return;
            }

            IList<BirthdayDto> birthdayList = _jsonDataToBirthdayConverter.GetList(response);
            if (birthdayList == null)
            {
                _logger.Error("Converted birthdayList is null!");

                OnBirthdayDownloadFinished(null, false);
                return;
            }

            _birthdayList = birthdayList;

            OnBirthdayDownloadFinished(_birthdayList, true);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.Dispose();
        }
    }
}
