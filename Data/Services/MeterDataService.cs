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

namespace Data.Services
{
    public delegate void MeterDownloadEventHandler(IList<MeterDto> meterList, bool success, string response);

    public delegate void MeterDataDownloadEventHandler(IList<MeterDataDto> meterDataList, bool success, string response);
    public delegate void MeterDataAddEventHandler(bool success, string response);
    public delegate void MeterDataUpdateEventHandler(bool success, string response);
    public delegate void MeterDataDeleteEventHandler(bool success, string response);

    public class MeterDataService
    {
        private const string TAG = "MeterDataService";
        private const int TIMEOUT = 12 * 60 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static MeterDataService _instance = null;
        private static readonly object _padlock = new object();

        private MeterDto _activeMeterInView;
        private IList<MeterDto> _meterList = new List<MeterDto>();
        private IList<MeterDataDto> _meterDataList = new List<MeterDataDto>();

        private Timer _downloadTimer;

        MeterDataService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _meterDataDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event MeterDownloadEventHandler OnMeterDownloadFinished;
        private void publishOnMeterDownloadFinished(IList<MeterDto> meterList, bool success, string response)
        {
            OnMeterDownloadFinished?.Invoke(meterList, success, response);
        }

        public event MeterDataDownloadEventHandler OnMeterDataDownloadFinished;
        private void publishOnMeterDataDownloadFinished(IList<MeterDataDto> meterDataList, bool success, string response)
        {
            OnMeterDataDownloadFinished?.Invoke(meterDataList, success, response);
        }

        public event MeterDataAddEventHandler OnMeterDataAddFinished;
        private void publishOnMeterDataAddFinished(bool success, string response)
        {
            OnMeterDataAddFinished?.Invoke(success, response);
        }

        public event MeterDataUpdateEventHandler OnMeterDataUpdateFinished;
        private void publishOnMeterDataUpdateFinished(bool success, string response)
        {
            OnMeterDataUpdateFinished?.Invoke(success, response);
        }

        public event MeterDataDeleteEventHandler OnMeterDataDeleteFinished;
        private void publishOnMeterDataDeleteFinished(bool success, string response)
        {
            OnMeterDataDeleteFinished?.Invoke(success, response);
        }

        public static MeterDataService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MeterDataService();
                    }

                    return _instance;
                }
            }
        }

        public MeterDto ActiveMeterInView
        {
            get
            {
                return _activeMeterInView;
            }
            set
            {
                _activeMeterInView = value;
            }
        }

        public IList<MeterDto> MeterList
        {
            get
            {
                return _meterList;
            }
        }

        public MeterDto GetByTypeId(int typeId)
        {
            MeterDto foundMeter = _meterList
                        .Where(meter => meter.TypeId == typeId)
                        .Select(meter => meter)
                        .FirstOrDefault();
            return foundMeter;
        }

        public MeterDto GetByMeterId(string meterId)
        {
            MeterDto foundMeter = _meterList
                        .Where(meter => meter.MeterId == meterId)
                        .Select(meter => meter)
                        .FirstOrDefault();
            return foundMeter;
        }

        public IList<MeterDto> FoundMeterDto(string searchKey)
        {
            if (searchKey == string.Empty)
            {
                return _meterList;
            }

            List<MeterDto> foundMeterList = _meterList
                        .Where(meter => meter.ToString().Contains(searchKey))
                        .Select(meter => meter)
                        .OrderBy(meter => meter.TypeId)
                        .ToList();
            return foundMeterList;
        }

        public IList<MeterDataDto> MeterDataList
        {
            get
            {
                return _meterDataList;
            }
        }

        public MeterDataDto GetById(int id)
        {
            MeterDataDto foundMeterData = _meterDataList
                        .Where(meterData => meterData.Id == id)
                        .Select(meterData => meterData)
                        .FirstOrDefault();
            return foundMeterData;
        }

        public IList<MeterDataDto> FoundMeterDataDto(string searchKey)
        {
            if (_activeMeterInView == null)
            {
                return new List<MeterDataDto>();
            }

            if (searchKey == string.Empty)
            {
                return _activeMeterInView.MeterDataList;
            }

            List<MeterDataDto> foundMeterDataList = _activeMeterInView.MeterDataList
                        .Where(meterData => meterData.ToString().Contains(searchKey))
                        .Select(meterData => meterData)
                        .OrderBy(meterData => meterData.Id)
                        .ToList();
            return foundMeterDataList;
        }

        public int GetHighestMeterDataTypeId()
        {
            int highestTypeId = 0;
            foreach (MeterDataDto meterData in _meterDataList)
            {
                if (meterData.TypeId > highestTypeId)
                {
                    highestTypeId = meterData.TypeId;
                }
            }
            return highestTypeId;
        }

        public void LoadMeterDataList()
        {
            loadMeterDataListAsync();
        }

        public void AddMeterData(MeterDataDto newMeterData)
        {
            Logger.Instance.Debug(TAG, string.Format("AddMeterData: Adding new meterData {0}", newMeterData));
            addMeterDataAsync(newMeterData);
        }

        public void UpdateMeterData(MeterDataDto updateMeterData)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateMeterData: Updating meterData {0}", updateMeterData));
            updateMeterDataAsync(updateMeterData);
        }

        public void DeleteMeterData(MeterDataDto deleteMeterData)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteMeterData: Deleting meterData {0}", deleteMeterData));
            deleteMeterDataAsync(deleteMeterData);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Logger.Instance.Debug(TAG, string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadMeterDataListAsync();
        }

        private async Task loadMeterDataListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMeterDataDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_METER_DATA.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MeterData);
        }

        private async Task addMeterDataAsync(MeterDataDto newMeterData)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMeterDataAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newMeterData.CommandAdd);

            _downloadController.OnDownloadFinished += _meterDataAddFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MeterDataAdd);
        }

        private async Task updateMeterDataAsync(MeterDataDto updateMeterData)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMeterDataUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateMeterData.CommandUpdate);

            _downloadController.OnDownloadFinished += _meterDataUpdateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MeterDataUpdate);
        }

        private async Task deleteMeterDataAsync(MeterDataDto deleteMeterData)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMeterDataDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteMeterData.CommandDelete);

            _downloadController.OnDownloadFinished += _meterDataDeleteFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MeterDataDelete);
        }

        private void _meterDataDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MeterData)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMeterDataDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnMeterDataDownloadFinished(null, false, response);
                return;
            }

            IList<MeterDataDto> meterDataList = JsonDataToMeterDataConverter.Instance.GetList(response);
            if (meterDataList == null)
            {
                Logger.Instance.Error(TAG, "Converted meterDataList is null!");
                publishOnMeterDataDownloadFinished(null, false, response);
                return;
            }

            _meterDataList = meterDataList;
            publishOnMeterDataDownloadFinished(_meterDataList, true, response);
            createMeterList();
        }

        private void _meterDataAddFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MeterDataAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _meterDataAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMeterDataAddFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnMeterDataAddFinished(false, response);
                return;
            }

            publishOnMeterDataAddFinished(true, response);
            loadMeterDataListAsync();
        }

        private void _meterDataUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MeterDataUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _meterDataUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMeterDataUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Updating was not successful!");
                publishOnMeterDataUpdateFinished(false, response);
                return;
            }

            publishOnMeterDataUpdateFinished(true, response);
            loadMeterDataListAsync();
        }

        private void _meterDataDeleteFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MeterDataDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _meterDataDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMeterDataDeleteFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Deleting was not successful!");
                publishOnMeterDataDeleteFinished(false, response);
                return;
            }

            publishOnMeterDataDeleteFinished(true, response);
            loadMeterDataListAsync();
        }

        private void createMeterList()
        {
            List<MeterDto> meterList = new List<MeterDto>();

            foreach (MeterDataDto meterData in _meterDataList)
            {
                int typeId = meterData.TypeId;
                if (meterList.Any(meterDto => meterDto.TypeId == typeId))
                {
                    int meterIndexInList = meterList.FindIndex(meterDto => meterDto.TypeId == typeId);
                    meterList[meterIndexInList].MeterDataList.Add(meterData);
                }
                else
                {
                    MeterDto meterDto = new MeterDto(meterData.TypeId, meterData.Type, meterData.MeterId, meterData.Area, new List<MeterDataDto> { meterData });
                    meterList.Add(meterDto);
                }
            }

            _meterList = meterList;
            _activeMeterInView = _meterList.Count > 0 ? _meterList[0] : null;
            publishOnMeterDownloadFinished(_meterList, true, "");
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _meterDataDownloadFinished;
            _downloadController.OnDownloadFinished -= _meterDataAddFinished;
            _downloadController.OnDownloadFinished -= _meterDataUpdateFinished;
            _downloadController.OnDownloadFinished -= _meterDataDeleteFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
