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
        private const int TIMEOUT = 60 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static ScheduleService _instance = null;
        private static readonly object _padlock = new object();

        private IList<ScheduleDto> _scheduleList = new List<ScheduleDto>();

        private Timer _downloadTimer;

        ScheduleService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _scheduleDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event ScheduleDownloadEventHandler OnScheduleDownloadFinished;
        private void publishOnScheduleDownloadFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            OnScheduleDownloadFinished?.Invoke(scheduleList, success, response);
        }

        public event ScheduleDownloadEventHandler OnSetScheduleFinished;
        private void publishOnSetScheduleFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            OnSetScheduleFinished?.Invoke(scheduleList, success, response);
        }

        public event ScheduleAddEventHandler OnAddScheduleFinished;
        private void publishOnAddScheduleFinished(bool success, string response)
        {
            OnAddScheduleFinished?.Invoke(success, response);
        }

        public event ScheduleUpdateEventHandler OnUpdateScheduleFinished;
        private void publishOnUpdateScheduleFinished(bool success, string response)
        {
            OnUpdateScheduleFinished?.Invoke(success, response);
        }

        public event ScheduleDeleteEventHandler OnDeleteScheduleFinished;
        private void publishOnDeleteScheduleFinished(bool success, string response)
        {
            OnDeleteScheduleFinished?.Invoke(success, response);
        }

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
            if (searchKey == string.Empty)
            {
                return _scheduleList;
            }

            List<ScheduleDto> foundSchedules = _scheduleList
                        .Where(schedule =>
                            schedule.Name.Contains(searchKey)
                            || schedule.Socket.ToString().Contains(searchKey)
                            || schedule.WirelessSwitch.ToString().Contains(searchKey)
                            || schedule.Time.ToString().Contains(searchKey)
                            || schedule.Action.ToString().Contains(searchKey)
                            || schedule.IsActive.ToString().Contains(searchKey)
                            || schedule.ActiveString.Contains(searchKey))
                        .Select(schedule => schedule)
                        .ToList();

            return foundSchedules;
        }

        public void LoadScheduleList()
        {
            loadScheduleListAsync();
        }

        public void SetSchedule(ScheduleDto schedule, bool state)
        {
            Logger.Instance.Debug(TAG, string.Format("Set Schedule {0} to {1}", schedule, state));
            setScheduleAsync(schedule.Name, state);
        }

        public void SetSchedule(string scheduleName, bool state)
        {
            Logger.Instance.Debug(TAG, string.Format("Set Schedule {0} to {1}", scheduleName, state));
            ScheduleDto schedule = GetByName(scheduleName);
            setScheduleAsync(schedule.Name, state);
        }

        public void ChangeScheduleState(ScheduleDto schedule)
        {
            Logger.Instance.Debug(TAG, string.Format("Change state for schedule {0}", schedule));
            setScheduleAsync(schedule.Name, !schedule.IsActive);
        }

        public void ChangeScheduleState(string scheduleName)
        {
            Logger.Instance.Debug(TAG, string.Format("Change state for schedule {0}", scheduleName));
            ScheduleDto schedule = GetByName(scheduleName);
            setScheduleAsync(schedule.Name, !schedule.IsActive);
        }

        public void AddSchedule(ScheduleDto newSchedule)
        {
            Logger.Instance.Debug(TAG, string.Format("AddSchedule: add schedule {0}", newSchedule));
            addScheduleAsync(newSchedule);
        }

        public void UpdateSchedule(ScheduleDto updateSchedule)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateSchedule: updating schedule {0}", updateSchedule));
            updateScheduleAsync(updateSchedule);
        }

        public void DeleteSchedule(ScheduleDto deleteSchedule)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteSchedule: deleting schedule {0}", deleteSchedule));
            deleteScheduleAsync(deleteSchedule);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadScheduleListAsync();
        }

        private async Task loadScheduleListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnScheduleDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_SCHEDULES.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Schedule);
        }

        private async Task setScheduleAsync(string scheduleName, bool state)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnScheduleDownloadFinished(null, false, "No user");
                return;
            }

            string action = LucaServerAction.SET_SCHEDULE.Action + scheduleName + (state ? Constants.STATE_ON : Constants.STATE_OFF);

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                action);

            _downloadController.OnDownloadFinished += _setScheduleFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ScheduleSet);
        }

        private async Task addScheduleAsync(ScheduleDto newSchedule)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnAddScheduleFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newSchedule.CommandAdd);

            _downloadController.OnDownloadFinished += _addScheduleFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ScheduleAdd);
        }

        private async Task updateScheduleAsync(ScheduleDto updateSchedule)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnUpdateScheduleFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateSchedule.CommandUpdate);

            _downloadController.OnDownloadFinished += _updateScheduleFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ScheduleUpdate);
        }

        private async Task deleteScheduleAsync(ScheduleDto deleteSchedule)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnDeleteScheduleFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteSchedule.CommandDelete);

            _downloadController.OnDownloadFinished += _deleteScheduleFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ScheduleDelete);
        }

        private void _scheduleDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.Schedule)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnScheduleDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnScheduleDownloadFinished(null, false, response);
                return;
            }

            IList<ScheduleDto> scheduleList = JsonDataToScheduleConverter.Instance.GetList(
                response,
                WirelessSocketService.Instance.WirelessSocketList,
                /* TODO add wirelessSwitchList */
                null);

            if (scheduleList == null)
            {
                Logger.Instance.Error(TAG, "Converted scheduleList is null!");
                publishOnScheduleDownloadFinished(null, false, response);
                return;
            }

            _scheduleList = scheduleList;
            publishOnScheduleDownloadFinished(_scheduleList, true, response);
        }

        private void _setScheduleFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ScheduleSet)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _setScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnSetScheduleFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Setting was not successful!");
                publishOnSetScheduleFinished(null, false, response);
                return;
            }

            publishOnSetScheduleFinished(null, true, response);
            loadScheduleListAsync();
        }

        private void _addScheduleFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ScheduleAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _addScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnAddScheduleFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnAddScheduleFinished(false, response);
                return;
            }

            publishOnAddScheduleFinished(true, response);
            loadScheduleListAsync();
        }

        private void _updateScheduleFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ScheduleUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _updateScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnUpdateScheduleFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Updating was not successful!");
                publishOnUpdateScheduleFinished(false, response);
                return;
            }

            publishOnUpdateScheduleFinished(true, response);
            loadScheduleListAsync();
        }

        private void _deleteScheduleFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ScheduleDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _deleteScheduleFinished;

            if (response.Contains("Error") || response.Contains("ERROR") || response.Contains("0"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnDeleteScheduleFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Deleting was not successful!");
                publishOnDeleteScheduleFinished(false, response);
                return;
            }

            publishOnDeleteScheduleFinished(true, response);
            loadScheduleListAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _scheduleDownloadFinished;
            _downloadController.OnDownloadFinished -= _setScheduleFinished;
            _downloadController.OnDownloadFinished -= _addScheduleFinished;
            _downloadController.OnDownloadFinished -= _updateScheduleFinished;
            _downloadController.OnDownloadFinished -= _deleteScheduleFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
