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
    public delegate void ListedMenuDownloadEventHandler(IList<ListedMenuDto> listedMenuList, bool success, string response);
    public delegate void MenuDownloadEventHandler(IList<MenuDto> menuList, bool success, string response);
    public delegate void MenuUpdateEventHandler(bool success, string response);
    public delegate void MenuClearEventHandler(bool success, string response);

    public class MenuService
    {
        private const string TAG = "MenuService";
        private readonly Logger _logger;

        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly SettingsController _settingsController;
        private readonly DownloadController _downloadController;
        private readonly IJsonDataConverter<ListedMenuDto> _jsonDataToListedMenuConverter;
        private readonly IJsonDataConverter<MenuDto> _jsonDataToMenuConverter;

        private static MenuService _instance = null;
        private static readonly object _padlock = new object();

        private IList<ListedMenuDto> _listedMenuList = new List<ListedMenuDto>();
        private IList<MenuDto> _menuList = new List<MenuDto>();

        private Timer _downloadTimer;

        MenuService()
        {
            _logger = new Logger(TAG);

            _settingsController = SettingsController.Instance;
            _downloadController = new DownloadController();
            _jsonDataToListedMenuConverter = new JsonDataToListedMenuConverter();
            _jsonDataToMenuConverter = new JsonDataToMenuConverter();

            _downloadController.OnDownloadFinished += _listedMenuDownloadFinished;
            _downloadController.OnDownloadFinished += _menuDownloadFinished;

            _downloadTimer = new Timer(TIMEOUT);
            _downloadTimer.Elapsed += _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = true;
            _downloadTimer.Start();
        }

        public event ListedMenuDownloadEventHandler OnListedMenuDownloadFinished;
        private void publishOnListedMenuDownloadFinished(IList<ListedMenuDto> listedMenuList, bool success, string response)
        {
            OnListedMenuDownloadFinished?.Invoke(listedMenuList, success, response);
        }

        public event MenuDownloadEventHandler OnMenuDownloadFinished;
        private void publishOnMenuDownloadFinished(IList<MenuDto> menuList, bool success, string response)
        {
            OnMenuDownloadFinished?.Invoke(menuList, success, response);
        }

        public event MenuUpdateEventHandler OnMenuUpdateFinished;
        private void publishOnMenuUpdateFinished(bool success, string response)
        {
            OnMenuUpdateFinished?.Invoke(success, response);
        }

        public event MenuClearEventHandler OnMenuClearFinished;
        private void publishOnMenuClearFinished(bool success, string response)
        {
            OnMenuClearFinished?.Invoke(success, response);
        }

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

        public IList<ListedMenuDto> ListedMenuList
        {
            get
            {
                return _listedMenuList;
            }
        }

        public IList<MenuDto> MenuList
        {
            get
            {
                return _menuList.OrderBy(menu => menu.Date).ToList();
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

        public void LoadListedMenuList()
        {
            _logger.Debug("LoadListedMenuList");
            loadListedMenuListAsync();
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

        public void ClearMenu(MenuDto clearMenu)
        {
            _logger.Debug(string.Format("ClearMenu: {0}", clearMenu));
            clearMenuAsync(clearMenu);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Debug(string.Format("_downloadTimer_Elapsed with sender {0} and elapsedEventArgs {1}", sender, elapsedEventArgs));
            loadListedMenuListAsync();
            loadMenuListAsync();
        }

        private async Task loadListedMenuListAsync()
        {
            _logger.Debug("loadListedMenuListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnListedMenuDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_LISTED_MENU.Action;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ListedMenu);
        }

        private async Task loadMenuListAsync()
        {
            _logger.Debug("loadMenuListAsync");

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnMenuDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = "http://" + _settingsController.ServerIpAddress + Constants.ACTION_PATH + user.Name + "&password=" + user.Passphrase + "&action=" + LucaServerAction.GET_MENU.Action;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Menu);
        }

        private async Task updateMenuAsync(MenuDto updateMenu)
        {
            _logger.Debug(string.Format("updateMenuAsync: {0}", updateMenu));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnMenuUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateMenu.CommandUpdate);

            _downloadController.OnDownloadFinished += _menuUpdateFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MenuUpdate);
        }

        private async Task clearMenuAsync(MenuDto clearMenu)
        {
            _logger.Debug(string.Format("clearMenuAsync: {0}", clearMenu));

            UserDto user = _settingsController.User;
            if (user == null)
            {
                publishOnMenuClearFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                _settingsController.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                clearMenu.CommandClear);

            _downloadController.OnDownloadFinished += _menuClearFinished;

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MenuClear);
        }

        private void _listedMenuDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_listedMenuDownloadFinished");

            if (downloadType != DownloadType.ListedMenu)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnListedMenuDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnListedMenuDownloadFinished(null, false, response);
                return;
            }

            IList<ListedMenuDto> listedMenuList = _jsonDataToListedMenuConverter.GetList(response);
            if (listedMenuList == null)
            {
                _logger.Error("Converted listedMenuList is null!");

                publishOnListedMenuDownloadFinished(null, false, response);
                return;
            }

            _listedMenuList = listedMenuList;

            publishOnListedMenuDownloadFinished(_listedMenuList, true, response);
        }

        private void _menuDownloadFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_menuDownloadFinished");

            if (downloadType != DownloadType.Menu)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnMenuDownloadFinished(null, false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Download was not successful!");

                publishOnMenuDownloadFinished(null, false, response);
                return;
            }

            IList<MenuDto> menuList = _jsonDataToMenuConverter.GetList(response);
            if (menuList == null)
            {
                _logger.Error("Converted menuList is null!");

                publishOnMenuDownloadFinished(null, false, response);
                return;
            }

            _menuList = menuList;

            publishOnMenuDownloadFinished(_menuList, true, response);
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

                publishOnMenuUpdateFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Update was not successful!");

                publishOnMenuUpdateFinished(false, response);
                return;
            }

            publishOnMenuUpdateFinished(true, response);

            loadMenuListAsync();
        }

        private void _menuClearFinished(string response, bool success, DownloadType downloadType)
        {
            _logger.Debug("_menuUpdateFinished");

            if (downloadType != DownloadType.MenuClear)
            {
                _logger.Debug(string.Format("Received download finished with downloadType {0}", downloadType));
                return;
            }

            _downloadController.OnDownloadFinished -= _menuClearFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                _logger.Error(response);

                publishOnMenuClearFinished(false, response);
                return;
            }

            _logger.Debug(string.Format("response: {0}", response));

            if (!success)
            {
                _logger.Error("Update was not successful!");

                publishOnMenuClearFinished(false, response);
                return;
            }

            publishOnMenuClearFinished(true, response);

            loadMenuListAsync();
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");

            _downloadController.OnDownloadFinished -= _menuDownloadFinished;
            _downloadController.OnDownloadFinished -= _menuUpdateFinished;
            _downloadController.OnDownloadFinished -= _menuClearFinished;

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();

            _downloadController.Dispose();
        }
    }
}
