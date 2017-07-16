using Common.Common;
using Common.Dto;
using Common.Enums;
using Common.Tools;
using Data.Services;
using LucaHome.Rules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LucaHome.Pages
{
    public partial class ShoppingEntryAddPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "ShoppingEntryAddPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly ShoppingListService _shoppingListService;

        private readonly Notifier _notifier;

        private ShoppingEntryDto _newShoppingEntry;

        public ShoppingEntryAddPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _shoppingListService = ShoppingListService.Instance;

            _newShoppingEntry = new ShoppingEntryDto(_shoppingListService.ShoppingList.Count, "", ShoppingEntryGroup.OTHER, 1);

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

        public CollectionView ShoppingTypeList
        {
            get
            {
                IList<string> shoppingTypeList = new List<string>();
                foreach (ShoppingEntryGroup entry in ShoppingEntryGroup.Values)
                {
                    shoppingTypeList.Add(entry.Description);
                }
                return new CollectionView(shoppingTypeList);
            }
        }

        public string ShoppingEntryName
        {
            get
            {
                return _newShoppingEntry.Name;
            }
            set
            {
                _newShoppingEntry.Name = value;
                OnPropertyChanged("ShoppingEntryName");
            }
        }

        public string ShoppingEntryType
        {
            get
            {
                return _newShoppingEntry.Group.Description;
            }
            set
            {
                string description = value;
                ShoppingEntryGroup shoppingEntryGroup = ShoppingEntryGroup.GetByDescription(description);
                _newShoppingEntry.Group = shoppingEntryGroup;
                OnPropertyChanged("ShoppingEntryType");
            }
        }

        public int ShoppingEntryQuantity
        {
            get
            {
                return _newShoppingEntry.Quantity;
            }
            set
            {
                _newShoppingEntry.Quantity = value;
                OnPropertyChanged("ShoppingEntryQuantity");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _shoppingListService.OnShoppingEntryAddFinished -= _shoppingEntryAddFinished;
            _shoppingListService.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
        }

        private void SaveShoppingEntry_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("SaveShoppingEntry_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _shoppingListService.OnShoppingEntryAddFinished += _shoppingEntryAddFinished;
            _shoppingListService.AddShoppingEntry(_newShoppingEntry);
        }

        private void _shoppingEntryAddFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_ShoppingEntryAddFinished was successful {0}", success));
            _shoppingListService.OnShoppingEntryAddFinished -= _shoppingEntryAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new shopping entry!");

                _shoppingListService.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished;
                _shoppingListService.LoadShoppingList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding shopping entry failed!\n{0}", response));
            }
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            _logger.Debug(string.Format("_onShoppingListDownloadFinished with model {0} was successful {1}", shoppingList, success));
            _shoppingListService.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonCountIncrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonCountIncrease_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            ShoppingEntryQuantity++;
        }

        private void ButtonCountDecrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonCountDecrease_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            ShoppingEntryQuantity--;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }
    }
}
