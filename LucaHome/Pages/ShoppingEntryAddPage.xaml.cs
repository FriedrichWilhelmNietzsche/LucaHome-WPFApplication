using Common.Dto;
using Common.Enums;
using Data.Services;
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

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private ShoppingEntryDto _newShoppingEntry;

        public ShoppingEntryAddPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            _newShoppingEntry = new ShoppingEntryDto(ShoppingListService.Instance.ShoppingList.Count, "", ShoppingEntryGroup.OTHER, 1, "");

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

        public string ShoppingEntryUnit
        {
            get
            {
                return _newShoppingEntry.Unit;
            }
            set
            {
                _newShoppingEntry.Unit = value;
                OnPropertyChanged("ShoppingEntryUnit");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ShoppingListService.Instance.OnShoppingEntryAddFinished -= _shoppingEntryAddFinished;
            ShoppingListService.Instance.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
        }

        private void SaveShoppingEntry_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            ShoppingListService.Instance.OnShoppingEntryAddFinished += _shoppingEntryAddFinished;
            ShoppingListService.Instance.AddShoppingEntry(_newShoppingEntry);
        }

        private void _shoppingEntryAddFinished(bool success, string response)
        {
            ShoppingListService.Instance.OnShoppingEntryAddFinished -= _shoppingEntryAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new shopping entry!");
                ShoppingListService.Instance.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished;
                ShoppingListService.Instance.LoadShoppingList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding shopping entry failed!\n{0}", response));
            }
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            ShoppingListService.Instance.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonCountIncrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            ShoppingEntryQuantity++;
        }

        private void ButtonCountDecrease_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            ShoppingEntryQuantity--;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
