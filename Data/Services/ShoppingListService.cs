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
    public delegate void ShoppingListServiceDownloadEventHandler(IList<ShoppingEntryDto> shoppingList, bool success);
    public delegate void ShoppingEntryAddEventHandler(bool success);

    public class ShoppingListService
    {
        private const string TAG = "ShoppingListService";
        private readonly Logger _logger;

        private readonly AppSettingsService _appSettingsService;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToShoppingConverter _jsonDataToShoppingConverter;

        private static ShoppingListService _instance = null;
        private static readonly object _padlock = new object();

        private IList<ShoppingEntryDto> _shoppingList = new List<ShoppingEntryDto>();

        ShoppingListService()
        {
            _logger = new Logger(TAG);

            _appSettingsService = AppSettingsService.Instance;
            _downloadController = new DownloadController();
            _jsonDataToShoppingConverter = new JsonDataToShoppingConverter();
        }

        public event ShoppingListServiceDownloadEventHandler OnShoppingListDownloadFinished;
        public event ShoppingEntryAddEventHandler OnShoppingEntryAddFinished;

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

        private async Task loadShoppingListAsync()
        {
            _logger.Debug("loadShoppingListAsync");

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnShoppingListDownloadFinished(null, false);
                return;
            }

            string requestUrl = "http://" + _appSettingsService.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_SHOPPING_LIST.Action;
            _logger.Debug(string.Format("RequestUrl {0}", requestUrl));

            _downloadController.OnDownloadFinished += _shoppingListDownloadFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.ShoppingList);
        }

        private async Task addShoppingEntryAsync(ShoppingEntryDto newShoppingEntry)
        {
            _logger.Debug(string.Format("addShoppingEntryAsync: Adding new shoppingEntry {0}", newShoppingEntry));

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnShoppingEntryAddFinished(false);
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _appSettingsService.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                newShoppingEntry.CommandAdd);

            _downloadController.OnDownloadFinished += _shoppingEntryAddFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.ShoppingListAdd);
        }

        private void _shoppingListDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_shoppingListDownloadFinished");

            if (downloadType != DownloadType.ShoppingList)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _shoppingListDownloadFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnShoppingListDownloadFinished(null, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnShoppingListDownloadFinished(null, false);
                return;
            }

            IList<ShoppingEntryDto> shoppingList = _jsonDataToShoppingConverter.GetList(response);
            if (shoppingList == null)
            {
                _logger.Error("Converted shoppingList is null!");

                OnShoppingListDownloadFinished(null, false);
                return;
            }

            _shoppingList = shoppingList;

            OnShoppingListDownloadFinished(_shoppingList, true);
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

                OnShoppingEntryAddFinished(false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Adding was not successful!");

                OnShoppingEntryAddFinished(false);
                return;
            }

            OnShoppingEntryAddFinished(true);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _shoppingListDownloadFinished;
            _downloadController.OnDownloadFinished -= _shoppingEntryAddFinished;

            _downloadController.Dispose();
        }
    }
}
