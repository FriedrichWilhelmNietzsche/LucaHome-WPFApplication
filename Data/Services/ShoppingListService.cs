using Common.Common;
using Common.Converter;
using Common.Dto;
using Common.Enums;
using Common.Interfaces;
using Common.Tools;
using Data.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Data.Services
{
    public delegate void ShoppingListServiceDownloadEventHandler(IList<ShoppingEntryDto> shoppingList, bool success, string response);
    public delegate void ShoppingEntryAddEventHandler(bool success, string response);
    public delegate void ShoppingEntryUpdateEventHandler(bool success, string response);
    public delegate void ShoppingEntryDeleteEventHandler(bool success, string response);

    public class ShoppingListService
    {
        private const string TAG = "ShoppingListService";
        private readonly Logger _logger;

        private const int TIMEOUT = 5 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly IJsonDataConverter<ShoppingEntryDto> _jsonDataToShoppingConverter;

        private static ShoppingListService _instance = null;
        private static readonly object _padlock = new object();

        private IList<ShoppingEntryDto> _shoppingList = new List<ShoppingEntryDto>();

        private Timer _downloadTimer;

        ShoppingListService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToShoppingConverter = new JsonDataToShoppingConverter();

            _downloadController.OnDownloadFinished += _shoppingListDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event ShoppingListServiceDownloadEventHandler OnShoppingListDownloadFinished;
        public event ShoppingEntryAddEventHandler OnShoppingEntryAddFinished;
        public event ShoppingEntryUpdateEventHandler OnShoppingEntryUpdateFinished;
        public event ShoppingEntryDeleteEventHandler OnShoppingEntryDeleteFinished;

        public static ShoppingListService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new ShoppingListService();
                    }

                    return _instance;
                }
            }
        }

        public IList<ShoppingEntryDto> ShoppingList
        {
            get
            {
                return _shoppingList;
            }
        }

        public ShoppingEntryDto GetById(int id)
        {
            ShoppingEntryDto foundEntry = _shoppingList
                        .Where(entry => entry.Id == id)
                        .Select(entry => entry)
                        .FirstOrDefault();

            return foundEntry;
        }

        public IList<ShoppingEntryDto> FoundShoppingEntries(string searchKey)
        {
            List<ShoppingEntryDto> foundShoppingEntries = _shoppingList
                        .Where(shoppingEntry =>
                            shoppingEntry.Name.Contains(searchKey)
                            || shoppingEntry.Group.ToString().Contains(searchKey)
                            || shoppingEntry.Quantity.ToString().Contains(searchKey))
                        .Select(shoppingEntry => shoppingEntry)
                        .ToList();

            return foundShoppingEntries;
        }

        public void LoadShoppingList()
        {
            _logger.Debug("LoadShoppingList");
            loadShoppingListAsync();
        }

        public void AddShoppingEntry(ShoppingEntryDto newShoppingEntry)
        {
            _logger.Debug(string.Format("AddShoppingEntry: Adding new ShoppingEntry {0}", newShoppingEntry));
            addShoppingEntryAsync(newShoppingEntry);
        }

        public void UpdateShoppingEntry(ShoppingEntryDto updateShoppingEntry)
        {
            _logger.Debug(string.Format("UpdateShoppingEntry: Updating ShoppingEntry {0}", updateShoppingEntry));
            updateShoppingEntryAsync(updateShoppingEntry);
        }

        public void DeleteShoppingEntry(ShoppingEntryDto deleteShoppingEntry)
        {
            _logger.Debug(string.Format("DeleteShoppingEntry: Deleting ShoppingEntry {0}", deleteShoppingEntry));
            deleteShoppingEntryAsync(deleteShoppingEntry);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadShoppingListAsync();
        }

        private async Task loadShoppingListAsync()
        {
            _logger.Debug("loadShoppingListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnShoppingListDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SHOPPING_LIST.Action;
            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingList);
        }

        private async Task addShoppingEntryAsync(ShoppingEntryDto newShoppingEntry)
        {
            _logger.Debug(string.Format("addShoppingEntryAsync: Adding new shoppingEntry {0}", newShoppingEntry));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnShoppingEntryAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newShoppingEntry.CommandAdd);

            _downloadController.OnDownloadFinished += _shoppingEntryAddFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingListAdd);
        }

        private async Task updateShoppingEntryAsync(ShoppingEntryDto updateShoppingEntry)
        {
            _logger.Debug(string.Format("updateShoppingEntryAsync: Updating shoppingEntry {0}", updateShoppingEntry));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnShoppingEntryUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateShoppingEntry.CommandUpdate);

            _downloadController.OnDownloadFinished += _shoppingEntryUpdateFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingListUpdate);
        }

        private async Task deleteShoppingEntryAsync(ShoppingEntryDto deleteShoppingEntry)
        {
            _logger.Debug(string.Format("deleteShoppingEntryAsync: Deleting shoppingEntry {0}", deleteShoppingEntry));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                OnShoppingEntryDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteShoppingEntry.CommandDelete);

            _downloadController.OnDownloadFinished += _shoppingEntryDeleteFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingListDelete);
        }

        private void _shoppingListDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_shoppingListDownloadFinished");

            if (downloadType != DownloadType.ShoppingList)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnShoppingListDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnShoppingListDownloadFinished(null, false, response);
                return;
            }

            IList<ShoppingEntryDto> shoppingList = _jsonDataToShoppingConverter.GetList(response);
            if (shoppingList == null)
            {
                _logger.Error("Converted shoppingList is null!");

                OnShoppingListDownloadFinished(null, false, response);
                return;
            }

            _shoppingList = shoppingList;

            OnShoppingListDownloadFinished(_shoppingList, true, response);
        }

        private void _shoppingEntryAddFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_shoppingEntryAddFinished");

            if (downloadType != DownloadType.ShoppingListAdd)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _shoppingEntryAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnShoppingEntryAddFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                OnShoppingEntryAddFinished(false, response);
                return;
            }

            OnShoppingEntryAddFinished(true, response);

            loadShoppingListAsync();
        }

        private void _shoppingEntryUpdateFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_shoppingEntryUpdateFinished");

            if (downloadType != DownloadType.ShoppingListUpdate)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _shoppingEntryUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnShoppingEntryUpdateFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                OnShoppingEntryUpdateFinished(false, response);
                return;
            }

            OnShoppingEntryUpdateFinished(true, response);

            loadShoppingListAsync();
        }

        private void _shoppingEntryDeleteFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_shoppingEntryDeleteFinished");

            if (downloadType != DownloadType.ShoppingListDelete)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _shoppingEntryDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnShoppingEntryDeleteFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Deleting was not successful!");

                OnShoppingEntryDeleteFinished(false, response);
                return;
            }

            OnShoppingEntryDeleteFinished(true, response);

            loadShoppingListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _shoppingListDownloadFinished;
            _downloadController.OnDownloadFinished -= _shoppingEntryAddFinished;
            _downloadController.OnDownloadFinished -= _shoppingEntryUpdateFinished;
            _downloadController.OnDownloadFinished -= _shoppingEntryDeleteFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
