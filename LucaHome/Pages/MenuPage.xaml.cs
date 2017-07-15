using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 */

namespace LucaHome.Pages
{
    public partial class MenuPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MenuPage";
        private readonly Logger _logger;

        private readonly MenuService _menuService;
        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private IList<MenuDto> _menuList = new List<MenuDto>();

        public MenuPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _menuService = MenuService.Instance;
            _navigationService = navigationService;

            InitializeComponent();
            DataContext = this;

            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 15,
                    offsetY: 15);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(2),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(2));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            });

            _notifier.ClearMessages();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IList<MenuDto> MenuList
        {
            get
            {
                return _menuList;
            }
            set
            {
                _menuList = value;
                OnPropertyChanged("MenuList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _menuService.OnMenuDownloadFinished += _onMenuListDownloadFinished;

            if (_menuService.MenuList == null)
            {
                _menuService.LoadMenuList();
                return;
            }

            MenuList = _menuService.MenuList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _menuService.OnMenuDownloadFinished -= _onMenuListDownloadFinished;
        }

        private void _onMenuListDownloadFinished(IList<MenuDto> menuList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMenuListDownloadFinished with model {0} was successful {1}", menuList, success));
            MenuList = _menuService.MenuList;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Received click of sender {0} with arguments {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int menuId = (int)senderButton.Tag;
                MenuDto updateMenu = _menuService.GetById(menuId);

                if (updateMenu != null)
                {
                    _navigationService.Navigate(new MenuUpdatePage(_navigationService, updateMenu));
                }
                else
                {
                    _logger.Error(string.Format("Found no menu with menuId {0}!", menuId));
                    _notifier.ShowError("Error while trying to update menu! MenuIdNotFoundError!");
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }


        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _menuService.LoadMenuList();
        }
    }
}
