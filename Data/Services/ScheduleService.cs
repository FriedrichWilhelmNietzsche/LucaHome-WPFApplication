using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Data.Services
{
    public delegate void ScheduleDownloadEventHandler(IList<ScheduleDto> scheduleList, bool success, string response);
    public delegate void ScheduleAddEventHandler(bool success, string response);
    public delegate void ScheduleUpdateEventHandler(bool success, string response);
    public delegate void ScheduleDeleteEventHandler(bool success, string response);

    public class ScheduleService
    {
        private const string TAG = "ScheduleService";
        private readonly Logger _logger;

        private const int TIMEOUT = 10 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToScheduleConverter _jsonDataToScheduleConverter;
        private readonly WirelessSocketService _wirelessSocketService;

        private static ScheduleService _instance = null;
        private static readonly object _padlock = new object();

        private IList<ScheduleDto> _scheduleList = new List<ScheduleDto>();

        private Timer _downloadTimer;

        ScheduleService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToScheduleConverter = new JsonDataToScheduleConverter();
            _wirelessSocketService = WirelessSocketService.Instance;

            _downloadController.OnDownloadFinished += _scheduleDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event ScheduleDownloadEventHandler OnScheduleDownloadFinished;
        public event ScheduleDownloadEventHandler OnSetScheduleFinished;
        public event ScheduleAddEventHandler OnAddScheduleFinished;
        public event ScheduleUpdateEventHandler OnUpdateScheduleFinished;
        public event ScheduleDeleteEventHandler OnDeleteScheduleFinished;

        public static ScheduleService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new ScheduleService();
                    }

                    return _instance;
                }
            }
        }

        public IList<ScheduleDto> ScheduleList
        {
            get
            {
                return _scheduleList;
            }
        }

        public ScheduleDto GetById(int id)
        {
            ScheduleDto foundSchedule = _scheduleList
                        .Where(schedule => schedule.Id == id)
                        .Select(schedule => schedule)
                        .FirstOrDefault();

            return foundSchedule;
        }

        public ScheduleDto GetByName(string name)
        {
            ScheduleDto foundSchedule = _scheduleList
                        .Where(schedule => schedule.Name.Equals(name))
                        .Select(schedule => schedule)
                        .FirstOrDefault();

            return foundSchedule;
        }

        public IList<ScheduleDto> FoundSchedules(string searchKey)
        {
            List<ScheduleDto> foundSchedules = _scheduleList
                        .Where(schedule =>
                            schedule.Name.Contains(searchKey)
                            || schedule.Socket.ToString().Contains(searchKey)
                            || schedule.Information.Contains(searchKey)
                            || schedule.WeekDay.ToString().Contains(searchKey)
                            || schedule.IsActive.ToString().Contains(searchKey)
                            || schedule.Time.ToString().Contains(searchKey))
                        .Select(schedule => schedule)
                        .ToList();

            return foundSchedules;
        }

        public void LoadScheduleList()
        {
            _logger.Debug("LoadScheduleList");
            loadScheduleListAsync();
        }

        public void SetSchedule(ScheduleDto schedule, bool state)
        {
            _logger.Debug(string.Format("Set Schedule {0} to {1}", schedule, state));
            setScheduleAsync(schedule.Name, state);
        }

        public void SetSchedule(string scheduleName, bool state)
        {
            _logger.Debug(string.Format("Set Schedule {0} to {1}", scheduleName, state));
            ScheduleDto schedule = GetByName(scheduleName);
            setScheduleAsync(schedule.Name, state);
        }

        public void ChangeScheduleState(ScheduleDto schedule)
        {
            _logger.Debug(string.Format("Change state for schedule {0}", schedule));
            setScheduleAsync(schedule.Name, !schedule.IsActive);
        }

        public void ChangeScheduleState(string scheduleName)
        {
            _logger.Debug(string.Format("Change state for schedule {0}", scheduleName));
            ScheduleDto schedule = GetByName(scheduleName);
            setScheduleAsync(schedule.Name, !schedule.IsActive);
        }

        public void AddSchedule(ScheduleDto newSchedule)
        {
            _logger.Debug(string.Format("AddSchedule: add schedule {0}", newSchedule));
            addScheduleAsync(newSchedule);
        }

        public void UpdateSchedule(ScheduleDto updateSchedule)
        {
            _logger.Debug(string.Format("UpdateSchedule: updating schedule {0}", updateSchedule));
            updateScheduleAsync(updateSchedule);
        }

        public void DeleteSchedule(ScheduleDto deleteSchedule)
        {
            _logger.Debug(string.Format("DeleteSchedule: deleting schedule {0}", deleteSchedule));
            deleteScheduleAsync(deleteSchedule);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadScheduleListAsync();
        }

        private async Task loadScheduleListAsync()
        {
            _logger.Debug("loadScheduleListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnScheduleDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SCHEDULES.Action;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.Schedule);
        }

        private async Task setScheduleAsync(string scheduleName, bool state)
        {
            _logger.Debug(string.Format("setScheduleListAsync: socketName {0} state {1}", scheduleName, state));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnScheduleDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.SET_SCHEDULE.Action + scheduleName + (state ? Constants.STATE_ON : Constants.STATE_OFF);

            _downloadController.OnDownloadFinished += _setScheduleFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.ScheduleSet);
        }

        private async Task addScheduleAsync(ScheduleDto newSchedule)
        {
            _logger.Debug(string.Format("addScheduleAsync: add new schedule {0}", newSchedule));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnAddScheduleFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newSchedule.CommandAdd);

            _downloadController.OnDownloadFinished += _addScheduleFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.ScheduleAdd);
        }

        private async Task updateScheduleAsync(ScheduleDto updateSchedule)
        {
            _logger.Debug(string.Format("updateScheduleAsync: updating schedule {0}", updateSchedule));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnUpdateScheduleFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateSchedule.CommandUpdate);

            _downloadController.OnDownloadFinished += _updateScheduleFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.ScheduleUpdate);
        }

        private async Task deleteScheduleAsync(ScheduleDto deleteSchedule)
        {
            _logger.Debug(string.Format("deleteScheduleAsync: delete schedule {0}", deleteSchedule));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnDeleteScheduleFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteSchedule.CommandDelete);

            _downloadController.OnDownloadFinished += _deleteScheduleFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.ScheduleDelete);
        }

        private void _scheduleDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_scheduleDownloadFinished");

            if (downloadType != DownloadType.Schedule)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnScheduleDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnScheduleDownloadFinished(null, false, response);
                return;
            }

            IList<ScheduleDto> scheduleList = _jsonDataToScheduleConverter.GetList(response, _wirelessSocketService.WirelessSocketList);
            if (scheduleList == null)
            {
                _logger.Error("Converted scheduleList is null!");

                OnScheduleDownloadFinished(null, false, response);
                return;
            }

            _scheduleList = scheduleList;

            OnScheduleDownloadFinished(_scheduleList, true, response);
        }

        private void _setScheduleFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_setScheduleFinished");

            if (downloadType != DownloadType.ScheduleSet)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _setScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnSetScheduleFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Setting was not successful!");

                OnSetScheduleFinished(null, false, response);
                return;
            }

            OnSetScheduleFinished(null, true, response);

            loadScheduleListAsync();
        }

        private void _addScheduleFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_addScheduleFinished");

            if (downloadType != DownloadType.ScheduleAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _addScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnAddScheduleFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                OnAddScheduleFinished(false, response);
                return;
            }

            OnAddScheduleFinished(true, response);

            loadScheduleListAsync();
        }

        private void _updateScheduleFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_updateScheduleFinished");

            if (downloadType != DownloadType.ScheduleUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _updateScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnUpdateScheduleFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Updating was not successful!");

                OnUpdateScheduleFinished(false, response);
                return;
            }

            OnUpdateScheduleFinished(true, response);

            loadScheduleListAsync();
        }

        private void _deleteScheduleFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_deleteScheduleFinished");

            if (downloadType != DownloadType.ScheduleDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _deleteScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                _logger.Error(response);

                OnDeleteScheduleFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Deleting was not successful!");

                OnDeleteScheduleFinished(false, response);
                return;
            }

            OnDeleteScheduleFinished(true, response);

            loadScheduleListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _scheduleDownloadFinished;
            _downloadController.OnDownloadFinished -= _setScheduleFinished;
            _downloadController.OnDownloadFinished -= _addScheduleFinished;
            _downloadController.OnDownloadFinished -= _updateScheduleFinished;
            _downloadController.OnDownloadFinished -= _deleteScheduleFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
