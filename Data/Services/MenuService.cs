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
    public delegate void MenuDownloadEventHandler(IList<MenuDto> menuList, bool success);
    public delegate void MenuUpdateEventHandler(bool success);

    public class MenuService
    {
        private const string TAG = "MenuService";
        private readonly Logger _logger;

        private readonly AppSettingsService _appSettingsService;
        private readonly DownloadController _downloadController;
        private readonly JsonDataToMenuConverter _jsonDataToMenuConverter;

        private static MenuService _instance = null;
        private static readonly object _padlock = new object();

        private IList<MenuDto> _menuList = new List<MenuDto>();

        MenuService()
        {
            _logger = new Logger(TAG);

            _appSettingsService = AppSettingsService.Instance;
            _downloadController = new DownloadController();
            _jsonDataToMenuConverter = new JsonDataToMenuConverter();
        }

        public event MenuDownloadEventHandler OnMenuDownloadFinished;
        public event MenuUpdateEventHandler OnMenuUpdateFinished;

        public static MenuService Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MenuService();
                    }

                    return _instance;
                }
            }
        }

        public IList<MenuDto> MenuList
        {
            get
            {
                IList<MenuDto> sortedMenuList = _menuList.OrderBy(menu => menu.Date).ToList();
                return sortedMenuList;
            }
        }

        public MenuDto GetById(int id)
        {
            MenuDto foundMenu = _menuList
                        .Where(menu => menu.Id == id)
                        .Select(menu => menu)
                        .FirstOrDefault();

            return foundMenu;
        }

        public void LoadMenuList()
        {
            _logger.Debug("LoadMenuList");
            loadMenuListAsync();
        }

        public void UpdateMenu(MenuDto updateMenu)
        {
            _logger.Debug(string.Format("UpdateMenu: {0}", updateMenu));
            updateMenuAsync(updateMenu);
        }

        private async Task loadMenuListAsync()
        {
            _logger.Debug("loadMenuListAsync");

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnMenuDownloadFinished(null, false);
                return;
            }

            string requestUrl = "http://" + _appSettingsService.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_MENU.Action;

            _downloadController.OnDownloadFinished += _menuDownloadFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.Menu);
        }

        private async Task updateMenuAsync(MenuDto updateMenu)
        {
            _logger.Debug(string.Format("updateMenuAsync: {0}", updateMenu));

            UserDto user = _appSettingsService.User;
            if (user == null)
            {
                OnMenuUpdateFinished(false);
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _appSettingsService.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateMenu.CommandUpdate);

            _downloadController.OnDownloadFinished += _menuUpdateFinished;

            await _downloadController.SendCommandToWebsiteAsync(requestUrl, DownloadType.MenuUpdate);
        }

        private void _menuDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_menuDownloadFinished");

            if (downloadType != DownloadType.Menu)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _menuDownloadFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnMenuDownloadFinished(null, false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                OnMenuDownloadFinished(null, false);
                return;
            }

            IList<MenuDto> menuList = _jsonDataToMenuConverter.GetList(response);
            if (menuList == null)
            {
                _logger.Error("Converted menuList is null!");

                OnMenuDownloadFinished(null, false);
                return;
            }

            _menuList = menuList;

            OnMenuDownloadFinished(_menuList, true);
        }

        private void _menuUpdateFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_menuUpdateFinished");

            if (downloadType != DownloadType.MenuUpdate)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _menuUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                OnMenuUpdateFinished(false);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Update was not successful!");

                OnMenuUpdateFinished(false);
                return;
            }

            OnMenuUpdateFinished(true);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _menuDownloadFinished;
            _downloadController.OnDownloadFinished -= _menuUpdateFinished;

            _downloadController.Dispose();
        }
    }
}
