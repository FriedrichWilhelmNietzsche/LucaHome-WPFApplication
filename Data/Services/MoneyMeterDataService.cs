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
using System;

namespace Data.Services
{
    public delegate void MoneyDateDownloadEventHandler(IList<MoneyDateDto> moneyDateList, bool success, string response);
    public delegate void MoneyMeterDownloadEventHandler(IList<MoneyMeterDto> moneyMeterList, bool success, string response);

    public delegate void MoneyMeterDataDownloadEventHandler(IList<MoneyMeterDataDto> moneyMeterDataList, bool success, string response);
    public delegate void MoneyMeterDataAddEventHandler(bool success, string response);
    public delegate void MoneyMeterDataUpdateEventHandler(bool success, string response);
    public delegate void MoneyMeterDataDeleteEventHandler(bool success, string response);

    public class MoneyMeterDataService
    {
        private const string TAG = "MoneyMeterDataService";
        private const int TIMEOUT = 12 * 60 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static MoneyMeterDataService _instance = null;
        private static readonly object _padlock = new object();

        private IList<MoneyDateDto> _moneyDateList = new List<MoneyDateDto>();
        private IList<MoneyMeterDto> _moneyMeterList = new List<MoneyMeterDto>();
        private IList<MoneyMeterDataDto> _moneyMeterDataList = new List<MoneyMeterDataDto>();

        private Timer _downloadTimer;

        MoneyMeterDataService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _moneyMeterDataDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event MoneyDateDownloadEventHandler OnMoneyDateDownloadFinished;
        private void publishOnMoneyDateDownloadFinished(IList<MoneyDateDto> moneyDateList, bool success, string response)
        {
            OnMoneyDateDownloadFinished?.Invoke(moneyDateList, success, response);
        }

        public event MoneyMeterDownloadEventHandler OnMoneyMeterDownloadFinished;
        private void publishOnMoneyMeterDownloadFinished(IList<MoneyMeterDto> moneyMeterList, bool success, string response)
        {
            OnMoneyMeterDownloadFinished?.Invoke(moneyMeterList, success, response);
        }

        public event MoneyMeterDataDownloadEventHandler OnMoneyMeterDataDownloadFinished;
        private void publishOnMoneyMeterDataDownloadFinished(IList<MoneyMeterDataDto> moneyMeterDataList, bool success, string response)
        {
            OnMoneyMeterDataDownloadFinished?.Invoke(moneyMeterDataList, success, response);
        }

        public event MoneyMeterDataAddEventHandler OnMoneyMeterDataAddFinished;
        private void publishOnMoneyMeterDataAddFinished(bool success, string response)
        {
            OnMoneyMeterDataAddFinished?.Invoke(success, response);
        }

        public event MoneyMeterDataUpdateEventHandler OnMoneyMeterDataUpdateFinished;
        private void publishOnMoneyMeterDataUpdateFinished(bool success, string response)
        {
            OnMoneyMeterDataUpdateFinished?.Invoke(success, response);
        }

        public event MoneyMeterDataDeleteEventHandler OnMoneyMeterDataDeleteFinished;
        private void publishOnMoneyMeterDataDeleteFinished(bool success, string response)
        {
            OnMoneyMeterDataDeleteFinished?.Invoke(success, response);
        }

        public static MoneyMeterDataService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MoneyMeterDataService();
                    }

                    return _instance;
                }
            }
        }

        public IList<MoneyDateDto> MoneyDateList
        {
            get
            {
                return _moneyDateList;
            }
        }

        public MoneyDateDto GetByDate(DateTime saveDate)
        {
            MoneyDateDto foundMoneyDate = _moneyDateList
                        .Where(moneyDate => moneyDate.SaveDate == saveDate)
                        .Select(moneyDate => moneyDate)
                        .FirstOrDefault();
            return foundMoneyDate;
        }

        public IList<MoneyDateDto> FoundMoneyDateDto(string searchKey)
        {
            if (searchKey == string.Empty)
            {
                return _moneyDateList;
            }

            List<MoneyDateDto> foundMoneyDateList = _moneyDateList
                        .Where(moneyMeter => moneyMeter.ToString().Contains(searchKey))
                        .Select(moneyMeter => moneyMeter)
                        .OrderBy(moneyMeter => moneyMeter.SaveDate)
                        .ToList();
            return foundMoneyDateList;
        }

        public IList<MoneyMeterDto> MoneyMeterList
        {
            get
            {
                return _moneyMeterList;
            }
        }

        public MoneyMeterDto GetByTypeId(int typeId)
        {
            MoneyMeterDto foundMoneyMeter = _moneyMeterList
                        .Where(moneyMeter => moneyMeter.TypeId == typeId)
                        .Select(moneyMeter => moneyMeter)
                        .FirstOrDefault();
            return foundMoneyMeter;
        }

        public IList<MoneyMeterDto> FoundMoneyMeterDto(string searchKey)
        {
            if (searchKey == string.Empty)
            {
                return _moneyMeterList;
            }

            List<MoneyMeterDto> foundMoneyMeterList = _moneyMeterList
                        .Where(moneyMeter => moneyMeter.ToString().Contains(searchKey))
                        .Select(moneyMeter => moneyMeter)
                        .OrderBy(moneyMeter => moneyMeter.TypeId)
                        .ToList();
            return foundMoneyMeterList;
        }

        public IList<MoneyMeterDataDto> MoneyMeterDataList
        {
            get
            {
                return _moneyMeterDataList;
            }
        }

        public MoneyMeterDataDto GetById(int id)
        {
            MoneyMeterDataDto foundMoneyMeterData = _moneyMeterDataList
                        .Where(moneyMeterData => moneyMeterData.Id == id)
                        .Select(moneyMeterData => moneyMeterData)
                        .FirstOrDefault();
            return foundMoneyMeterData;
        }

        public IList<MoneyMeterDataDto> FoundMoneyMeterDataDto(string searchKey)
        {
            if (searchKey == string.Empty)
            {
                return _moneyMeterDataList;
            }

            List<MoneyMeterDataDto> foundMoneyMeterDataList = _moneyMeterDataList
                        .Where(moneyMeterData => moneyMeterData.ToString().Contains(searchKey))
                        .Select(moneyMeterData => moneyMeterData)
                        .OrderBy(moneyMeterData => moneyMeterData.Id)
                        .ToList();
            return foundMoneyMeterDataList;
        }

        public void LoadMoneyMeterDataList()
        {
            loadMoneyMeterDataListAsync();
        }

        public void AddMoneyMeterData(MoneyMeterDataDto newMoneyMeterData)
        {
            Logger.Instance.Debug(TAG, string.Format("AddMoneyMeterData: Adding new moneyMeterData {0}", newMoneyMeterData));
            addMoneyMeterDataAsync(newMoneyMeterData);
        }

        public void UpdateMoneyMeterData(MoneyMeterDataDto updateMoneyMeterData)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateMoneyMeterData: Updating moneyMeterData {0}", updateMoneyMeterData));
            updateMoneyMeterDataAsync(updateMoneyMeterData);
        }

        public void DeleteMoneyMeterData(MoneyMeterDataDto deleteMoneyMeterData)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteMoneyMeterData: Deleting moneyMeterData {0}", deleteMoneyMeterData));
            deleteMoneyMeterDataAsync(deleteMoneyMeterData);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Logger.Instance.Debug(TAG, string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadMoneyMeterDataListAsync();
        }

        private async Task loadMoneyMeterDataListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMoneyMeterDataDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_MONEY_METER_DATA_USER.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MoneyMeterData);
        }

        private async Task addMoneyMeterDataAsync(MoneyMeterDataDto newMoneyMeterData)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMoneyMeterDataAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newMoneyMeterData.CommandAdd);

            _downloadController.OnDownloadFinished += _moneyMeterDataAddFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MoneyMeterDataAdd);
        }

        private async Task updateMoneyMeterDataAsync(MoneyMeterDataDto updateMoneyMeterData)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMoneyMeterDataUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateMoneyMeterData.CommandUpdate);

            _downloadController.OnDownloadFinished += _moneyMeterDataUpdateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MoneyMeterDataUpdate);
        }

        private async Task deleteMoneyMeterDataAsync(MoneyMeterDataDto deleteMoneyMeterData)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMoneyMeterDataDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteMoneyMeterData.CommandDelete);

            _downloadController.OnDownloadFinished += _moneyMeterDataDeleteFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MoneyMeterDataDelete);
        }

        private void _moneyMeterDataDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MoneyMeterData)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMoneyMeterDataDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnMoneyMeterDataDownloadFinished(null, false, response);
                return;
            }

            IList<MoneyMeterDataDto> moneyMeterDataList = JsonDataToMoneyMeterDataConverter.Instance.GetList(response);
            if (moneyMeterDataList == null)
            {
                Logger.Instance.Error(TAG, "Converted moneyMeterDataList is null!");
                publishOnMoneyMeterDataDownloadFinished(null, false, response);
                return;
            }

            _moneyMeterDataList = moneyMeterDataList;
            publishOnMoneyMeterDataDownloadFinished(_moneyMeterDataList, true, response);
            createMoneyMeterList();
            createMoneyDateList();
        }

        private void _moneyMeterDataAddFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MoneyMeterDataAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _moneyMeterDataAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMoneyMeterDataAddFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnMoneyMeterDataAddFinished(false, response);
                return;
            }

            publishOnMoneyMeterDataAddFinished(true, response);
            loadMoneyMeterDataListAsync();
        }

        private void _moneyMeterDataUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MoneyMeterDataUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _moneyMeterDataUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMoneyMeterDataUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Updating was not successful!");
                publishOnMoneyMeterDataUpdateFinished(false, response);
                return;
            }

            publishOnMoneyMeterDataUpdateFinished(true, response);
            loadMoneyMeterDataListAsync();
        }

        private void _moneyMeterDataDeleteFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MoneyMeterDataDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _moneyMeterDataDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMoneyMeterDataDeleteFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Deleting was not successful!");
                publishOnMoneyMeterDataDeleteFinished(false, response);
                return;
            }

            publishOnMoneyMeterDataDeleteFinished(true, response);
            loadMoneyMeterDataListAsync();
        }

        private void createMoneyMeterList()
        {
            List<MoneyMeterDto> moneyMeterList = new List<MoneyMeterDto>();

            foreach (MoneyMeterDataDto moneyMeterData in _moneyMeterDataList)
            {
                int typeId = moneyMeterData.TypeId;
                if (moneyMeterList.Any(moneyMeterDto => moneyMeterDto.TypeId == typeId))
                {
                    int moneyMeterIndexInList = moneyMeterList.FindIndex(moneyMeterDto => moneyMeterDto.TypeId == typeId);
                    moneyMeterList[moneyMeterIndexInList].MoneyMeterDataList.Add(moneyMeterData);
                }
                else
                {
                    MoneyMeterDto moneyMeterDto = new MoneyMeterDto(moneyMeterData.TypeId, moneyMeterData.Bank, moneyMeterData.Plan, moneyMeterData.User, new List<MoneyMeterDataDto> { moneyMeterData });
                    moneyMeterList.Add(moneyMeterDto);
                }
            }

            _moneyMeterList = moneyMeterList;
            publishOnMoneyMeterDownloadFinished(_moneyMeterList, true, "");
        }

        private void createMoneyDateList()
        {
            List<MoneyDateDto> moneyDateList = new List<MoneyDateDto>();

            foreach (MoneyMeterDataDto moneyMeterData in _moneyMeterDataList)
            {
                DateTime saveDate = moneyMeterData.SaveDate;
                if (moneyDateList.Any(moneyMeterDto => moneyMeterDto.SaveDate == saveDate))
                {
                    int moneyDateIndexInList = moneyDateList.FindIndex(moneyDateDto => moneyDateDto.SaveDate == saveDate);
                    moneyDateList[moneyDateIndexInList].MoneyMeterDataList.Add(moneyMeterData);
                }
                else
                {
                    MoneyDateDto moneyDateDto = new MoneyDateDto(moneyMeterData.SaveDate, moneyMeterData.User, new List<MoneyMeterDataDto> { moneyMeterData });
                    moneyDateList.Add(moneyDateDto);
                }
            }

            _moneyDateList = moneyDateList;
            publishOnMoneyDateDownloadFinished(_moneyDateList, true, "");
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _moneyMeterDataDownloadFinished;
            _downloadController.OnDownloadFinished -= _moneyMeterDataAddFinished;
            _downloadController.OnDownloadFinished -= _moneyMeterDataUpdateFinished;
            _downloadController.OnDownloadFinished -= _moneyMeterDataDeleteFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
