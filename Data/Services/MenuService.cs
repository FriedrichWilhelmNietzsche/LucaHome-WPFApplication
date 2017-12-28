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
    public delegate void ListedMenuDownloadEventHandler(IList<ListedMenuDto> listedMenuList, bool success, string response);
    public delegate void ListedMenuAddEventHandler(bool success, string response);
    public delegate void ListedMenuUpdateEventHandler(bool success, string response);
    public delegate void ListedMenuDeleteEventHandler(bool success, string response);

    public delegate void MenuDownloadEventHandler(IList<MenuDto> menuList, bool success, string response);
    public delegate void MenuUpdateEventHandler(bool success, string response);
    public delegate void MenuClearEventHandler(bool success, string response);

    public class MenuService
    {
        private const string TAG = "MenuService";
        private const int TIMEOUT = 6 * 60 * 60 * 1000;

        private readonly DownloadController _downloadController;

        private static MenuService _instance = null;
        private static readonly object _padlock = new object();

        private IList<ListedMenuDto> _listedMenuList = new List<ListedMenuDto>();
        private IList<MenuDto> _menuList = new List<MenuDto>();

        private Timer _downloadTimer;

        MenuService()
        {
            _downloadController = new DownloadController();
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

        public event ListedMenuAddEventHandler OnListedMenuAddFinished;
        private void publishOnListedMenuAddFinished(bool success, string response)
        {
            OnListedMenuAddFinished?.Invoke(success, response);
        }

        public event ListedMenuUpdateEventHandler OnListedMenuUpdateFinished;
        private void publishOnListedMenuUpdateFinished(bool success, string response)
        {
            OnListedMenuUpdateFinished?.Invoke(success, response);
        }

        public event ListedMenuDeleteEventHandler OnListedMenuDeleteFinished;
        private void publishOnListedMenuDeleteFinished(bool success, string response)
        {
            OnListedMenuDeleteFinished?.Invoke(success, response);
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
            loadListedMenuListAsync();
        }

        public void AddListedMenu(ListedMenuDto addListedMenu)
        {
            Logger.Instance.Debug(TAG, string.Format("AddListedMenu: {0}", addListedMenu));
            addListedMenuAsync(addListedMenu);
        }

        public void UpdateListedMenu(ListedMenuDto updateListedMenu)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateListedMenu: {0}", updateListedMenu));
            updateListedMenuAsync(updateListedMenu);
        }

        public void DeleteListedMenu(ListedMenuDto deleteListedMenu)
        {
            Logger.Instance.Debug(TAG, string.Format("DeleteListedMenu: {0}", deleteListedMenu));
            deleteListedMenuAsync(deleteListedMenu);
        }

        public void LoadMenuList()
        {
            loadMenuListAsync();
        }

        public void UpdateMenu(MenuDto updateMenu)
        {
            Logger.Instance.Debug(TAG, string.Format("UpdateMenu: {0}", updateMenu));
            updateMenuAsync(updateMenu);
        }

        public void ClearMenu(MenuDto clearMenu)
        {
            Logger.Instance.Debug(TAG, string.Format("ClearMenu: {0}", clearMenu));
            clearMenuAsync(clearMenu);
        }

        private void _downloadTimer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            loadListedMenuListAsync();
            loadMenuListAsync();
        }

        private async Task loadListedMenuListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnListedMenuDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_LISTEDMENU.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ListedMenu);
        }

        private async Task addListedMenuAsync(ListedMenuDto addListedMenu)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMenuUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                addListedMenu.CommandAdd);

            _downloadController.OnDownloadFinished += _listedMenuAddFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ListedMenuAdd);
        }

        private async Task updateListedMenuAsync(ListedMenuDto updateListedMenu)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMenuUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateListedMenu.CommandUpdate);

            _downloadController.OnDownloadFinished += _listedMenuUpdateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ListedMenuUpdate);
        }

        private async Task deleteListedMenuAsync(ListedMenuDto deleteListedMenu)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMenuUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                deleteListedMenu.CommandDelete);

            _downloadController.OnDownloadFinished += _listedMenuDeleteFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.ListedMenuDelete);
        }

        private async Task loadMenuListAsync()
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMenuDownloadFinished(null, false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                LucaServerAction.GET_MENU.Action);

            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.Menu);
        }

        private async Task updateMenuAsync(MenuDto updateMenu)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMenuUpdateFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                updateMenu.CommandUpdate);

            _downloadController.OnDownloadFinished += _menuUpdateFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MenuUpdate);
        }

        private async Task clearMenuAsync(MenuDto clearMenu)
        {
            UserDto user = SettingsController.Instance.User;
            if (user == null)
            {
                publishOnMenuClearFinished(false, "No user");
                return;
            }

            string requestUrl = string.Format("http://{0}{1}{2}&password={3}&action={4}",
                SettingsController.Instance.ServerIpAddress, Constants.ACTION_PATH,
                user.Name, user.Passphrase,
                clearMenu.CommandClear);

            _downloadController.OnDownloadFinished += _menuClearFinished;
            _downloadController.SendCommandToWebsite(requestUrl, DownloadType.MenuClear);
        }

        private void _listedMenuDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ListedMenu)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);

                publishOnListedMenuDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnListedMenuDownloadFinished(null, false, response);
                return;
            }

            IList<ListedMenuDto> listedMenuList = JsonDataToListedMenuConverter.Instance.GetList(response);
            if (listedMenuList == null)
            {
                Logger.Instance.Error(TAG, "Converted listedMenuList is null!");
                publishOnListedMenuDownloadFinished(null, false, response);
                return;
            }

            _listedMenuList = listedMenuList;
            publishOnListedMenuDownloadFinished(_listedMenuList, true, response);
        }

        private void _listedMenuAddFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ListedMenuAdd)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _listedMenuAddFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnListedMenuAddFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Update was not successful!");
                publishOnListedMenuAddFinished(false, response);
                return;
            }

            publishOnListedMenuAddFinished(true, response);
            loadListedMenuListAsync();
        }

        private void _listedMenuUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ListedMenuUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _listedMenuUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnListedMenuUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Update was not successful!");
                publishOnListedMenuUpdateFinished(false, response);
                return;
            }

            publishOnListedMenuUpdateFinished(true, response);
            loadListedMenuListAsync();
        }

        private void _listedMenuDeleteFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.ListedMenuDelete)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _listedMenuDeleteFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnListedMenuDeleteFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Update was not successful!");
                publishOnListedMenuDeleteFinished(false, response);
                return;
            }

            publishOnListedMenuDeleteFinished(true, response);
            loadListedMenuListAsync();
        }

        private void _menuDownloadFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.Menu)
            {
                return;
            }

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMenuDownloadFinished(null, false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Download was not successful!");
                publishOnMenuDownloadFinished(null, false, response);
                return;
            }

            IList<MenuDto> menuList = JsonDataToMenuConverter.Instance.GetList(response);
            if (menuList == null)
            {
                Logger.Instance.Error(TAG, "Converted menuList is null!");
                publishOnMenuDownloadFinished(null, false, response);
                return;
            }

            _menuList = menuList;
            publishOnMenuDownloadFinished(_menuList, true, response);
        }

        private void _menuUpdateFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MenuUpdate)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _menuUpdateFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMenuUpdateFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Update was not successful!");
                publishOnMenuUpdateFinished(false, response);
                return;
            }

            publishOnMenuUpdateFinished(true, response);
            loadMenuListAsync();
        }

        private void _menuClearFinished(string response, bool success, DownloadType downloadType, object additional)
        {
            if (downloadType != DownloadType.MenuClear)
            {
                return;
            }

            _downloadController.OnDownloadFinished -= _menuClearFinished;

            if (response.Contains("Error") || response.Contains("ERROR"))
            {
                Logger.Instance.Error(TAG, response);
                publishOnMenuClearFinished(false, response);
                return;
            }

            if (!success)
            {
                Logger.Instance.Error(TAG, "Update was not successful!");
                publishOnMenuClearFinished(false, response);
                return;
            }

            publishOnMenuClearFinished(true, response);
            loadMenuListAsync();
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");

            _downloadController.OnDownloadFinished -= _listedMenuDownloadFinished;
            _downloadController.OnDownloadFinished -= _listedMenuAddFinished;
            _downloadController.OnDownloadFinished -= _listedMenuUpdateFinished;
            _downloadController.OnDownloadFinished -= _listedMenuDeleteFinished;

            _downloadController.OnDownloadFinished -= _menuDownloadFinished;
            _downloadController.OnDownloadFinished -= _menuUpdateFinished;
            _downloadController.OnDownloadFinished -= _menuClearFinished;

            _downloadController.Dispose();

            _downloadTimer.Elapsed -= _downloadTimer_Elapsed;
            _downloadTimer.AutoReset = false;
            _downloadTimer.Stop();
        }
    }
}
