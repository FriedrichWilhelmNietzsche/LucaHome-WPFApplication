using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using System.Collections.Generic;
using System.ComponentModel;

namespace LucaHome.Pages
{
    public partial class MenuUpdatePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MenuUpdatePage";
        private readonly Logger _logger;

        private readonly MenuService _menuService;
        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private MenuDto _updateMenu;

        public MenuUpdatePage(NavigationService navigationService, MenuDto updateMenu)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _menuService = MenuService.Instance;
            _navigationService = navigationService;

            _updateMenu = updateMenu;

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

        public string MenuPageTitle
        {
            get
            {
                return string.Format("Update menu - {0}.{1}.{2}", _updateMenu.Date.Day, _updateMenu.Date.Month, _updateMenu.Date.Year); ;
            }
            set
            {
                _logger.Debug("MenuPageTitle cannot be overriden!");
                OnPropertyChanged("MenuPageTitle");
            }
        }

        public string Menu
        {
            get
            {
                return _updateMenu.Title;
            }
            set
            {
                _updateMenu.Title = value;
                OnPropertyChanged("Menu");
            }
        }

        public string Description
        {
            get
            {
                return _updateMenu.Description;
            }
            set
            {
                _updateMenu.Description = value;
                OnPropertyChanged("Description");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));

            _menuService.OnMenuDownloadFinished -= _onMenuDownloadFinished;
            _menuService.OnMenuUpdateFinished -= _onMenuUpdateFinished;
        }

        private void UpdateMenu_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("UpdateMenu_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _menuService.OnMenuUpdateFinished += _onMenuUpdateFinished;
            _menuService.UpdateMenu(_updateMenu);
        }

        private void _onMenuDownloadFinished(IList<MenuDto> menuList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMenuDownloadFinished with model {0} was successful {1}", menuList, success));

            _menuService.OnMenuDownloadFinished -= _onMenuDownloadFinished;
            _navigationService.GoBack();
        }

        private void _onMenuUpdateFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_onMenuUpdateFinished was successful {0}", success));

            _menuService.OnMenuUpdateFinished -= _onMenuUpdateFinished;

            if (success)
            {
                _notifier.ShowSuccess("Updated menu!");

                _menuService.OnMenuDownloadFinished += _onMenuDownloadFinished;
                _menuService.LoadMenuList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating menu failed!\n{0}", response));
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }
    }
}
