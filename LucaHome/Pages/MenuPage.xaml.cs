using Common.Dto;
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

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private IList<MenuDto> _menuList = new List<MenuDto>();

        public MenuPage(NavigationService navigationService)
        {
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
            MenuService.Instance.OnMenuDownloadFinished += _onMenuListDownloadFinished;

            if (MenuService.Instance.MenuList == null)
            {
                MenuService.Instance.LoadMenuList();
                return;
            }

            MenuList = MenuService.Instance.MenuList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuService.Instance.OnMenuDownloadFinished -= _onMenuListDownloadFinished;
        }

        private void _onMenuListDownloadFinished(IList<MenuDto> menuList, bool success, string response)
        {
            MenuList = MenuService.Instance.MenuList;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int menuId = (int)senderButton.Tag;
                MenuDto updateMenu = MenuService.Instance.GetById(menuId);

                if (updateMenu != null)
                {
                    _navigationService.Navigate(new MenuUpdatePage(_navigationService, updateMenu));
                }
                else
                {
                    _notifier.ShowError("Error while trying to update menu! MenuIdNotFoundError!");
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }


        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuService.Instance.LoadMenuList();
        }
    }
}
