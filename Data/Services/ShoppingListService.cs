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
    public delegate void ShoppingListServiceDownloadEventHandler(IList<ShoppingEntryDto> shoppingList, bool success, string response);
    public delegate void ShoppingEntryAddEventHandler(bool success, string response);
    public delegate void ShoppingEntryUpdateEventHandler(bool success, string response);
    public delegate void ShoppingEntryDeleteEventHandler(bool success, string response);

    public class ShoppingListService
    {
        private const string TAG = "ShoppingListService";
        private const int TIMEOUT = 3 * 60 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static ShoppingListService _instance = null;
        private static readonly object _padlock = new object();

        private IList<ShoppingEntryDto> _shoppingList = new List<ShoppingEntryDto>();

        private Timer _downloadTimer;

        ShoppingListService()
        {
            _downloadController = new DownloadController();
            _downloadController.OnDownloadFinished += _shoppingListDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event ShoppingListServiceDownloadEventHandler OnShoppingListDownloadFinished;
        private void publishOnShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            OnShoppingListDownloadFinished?.Invoke(shoppingList, success, response);
        }

        public event ShoppingEntryAddEventHandler OnShoppingEntryAddFinished;
        private void publishOnShoppingEntryAddFinished(bool success, string response)
        {
            OnShoppingEntryAddFinished?.Invoke(success, response);
        }

        public event ShoppingEntryUpdateEventHandler OnShoppingEntryUpdateFinished;
        private void publishOnShoppingEntryUpdateFinished(bool success, string response)
        {
            OnShoppingEntryUpdateFinished?.Invoke(success, response);
        }

        public event ShoppingEntryDeleteEventHandler OnShoppingEntryDeleteFinished;
        private void publishOnShoppingEntryDeleteFinished(bool success, string response)
        {
            OnShoppingEntryDeleteFinished?.Invoke(success, response);
        }

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
                return _shoppingList.OrderBy(shoppingEntry => shoppingEntry.Group).ToList();
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
            if (searchKey == string.Empty)
            {
                return _shoppingList;
            }

            List<ShoppingEntryDto> foundShoppingEntryList = _shoppingList
                        .Where(shoppingEntry => shoppingEntry.ToString().Contains(searchKey))
                        .Select(shoppingEntry => shoppingEntry)
                        .OrderBy(shoppingEntry => shoppingEntry.Group)
                        .ToList();
            return foundShoppingEntryList;
        }

        public void LoadShoppingList()
        {
            loadShoppingListAsync();
        }

        public void AddShoppingEntry(ShoppingEntryDto newShoppingEntry)
        {
            Logger.Instance.Debug(TAG, string.Format("AddShoppingEntry: Adding new ShoppingEntry {0}", newShoppingEntry));
            addShoppingEntryAsync(newShoppingEntry);
        }

        public void UpdateShoppingEntry(ShoppingEntryDto updateShoppingEntry)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateShoppingEntry: Updating ShoppingEntry {0}", updateShoppingEntry));
            updateShoppingEntryAsync(updateShoppingEntry);
        }

        public void DeleteShoppingEntry(ShoppingEntryDto deleteShoppingEntry)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteShoppingEntry: Deleting ShoppingEntry {0}", deleteShoppingEntry));
            deleteShoppingEntryAsync(deleteShoppingEntry);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadShoppingListAsync();
        }

        private async Task loadShoppingListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnShoppingListDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_SHOPPING_LIST.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingList);
        }

        private async Task addShoppingEntryAsync(ShoppingEntryDto newShoppingEntry)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnShoppingEntryAddFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newShoppingEntry.CommandAdd);

            _downloadController.OnDownloadFinished += _shoppingEntryAddFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingListAdd);
        }

        private async Task updateShoppingEntryAsync(ShoppingEntryDto updateShoppingEntry)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnShoppingEntryUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateShoppingEntry.CommandUpdate);

            _downloadController.OnDownloadFinished += _shoppingEntryUpdateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingListUpdate);
        }

        private async Task deleteShoppingEntryAsync(ShoppingEntryDto deleteShoppingEntry)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnShoppingEntryDeleteFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteShoppingEntry.CommandDelete);

            _downloadController.OnDownloadFinished += _shoppingEntryDeleteFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ShoppingListDelete);
        }

        private void _shoppingListDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ShoppingList)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnShoppingListDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnShoppingListDownloadFinished(null, false, response);
                return;
            }

            IList<ShoppingEntryDto> shoppingList = JsonDataToShoppingConverter.Instance.GetList(response);
            if (shoppingList == null)
            {
                Logger.Instance.Error(TAG, "Converted shoppingList is null!");
                publishOnShoppingListDownloadFinished(null, false, response);
                return;
            }

            _shoppingList = shoppingList.OrderBy(shoppingEntry => shoppingEntry.Group).ToList();
            publishOnShoppingListDownloadFinished(_shoppingList, true, response);
        }

        private void _shoppingEntryAddFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ShoppingListAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _shoppingEntryAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnShoppingEntryAddFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnShoppingEntryAddFinished(false, response);
                return;
            }

            publishOnShoppingEntryAddFinished(true, response);
            loadShoppingListAsync();
        }

        private void _shoppingEntryUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ShoppingListUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _shoppingEntryUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnShoppingEntryUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Adding was not successful!");
                publishOnShoppingEntryUpdateFinished(false, response);
                return;
            }

            publishOnShoppingEntryUpdateFinished(true, response);
            loadShoppingListAsync();
        }

        private void _shoppingEntryDeleteFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ShoppingListDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _shoppingEntryDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnShoppingEntryDeleteFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Deleting was not successful!");
                publishOnShoppingEntryDeleteFinished(false, response);
                return;
            }

            publishOnShoppingEntryDeleteFinished(true, response);
            loadShoppingListAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _shoppingListDownloadFinished;
            _downloadController.OnDownloadFinished -= _shoppingEntryAddFinished;
            _downloadController.OnDownloadFinished -= _shoppingEntryUpdateFinished;
            _downloadController.OnDownloadFinished -= _shoppingEntryDeleteFinished;
            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
