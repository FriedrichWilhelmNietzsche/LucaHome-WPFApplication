using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using LucaHome.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 */

namespace LucaHome.Pages
{
    public partial class ShoppingListPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "ShoppingListPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly ShoppingListService _shoppingListService;

        private string _shoppingListSearchKey = string.Empty;
        private IList<ShoppingEntryDto> _shoppingList = new List<ShoppingEntryDto>();

        private readonly ShoppingEntryAddPage _shoppingEntryAddPage;

        public ShoppingListPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _shoppingListService = ShoppingListService.Instance;

            _shoppingEntryAddPage = new ShoppingEntryAddPage(_navigationService);

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ShoppingListSearchKey
        {
            get
            {
                return string.Empty;
            }
            set
            {
                _shoppingListSearchKey = value;
                OnPropertyChanged("ShoppingListSearchKey");

                if (_shoppingListSearchKey != string.Empty)
                {
                    ShoppingList = _shoppingListService.FoundShoppingEntries(_shoppingListSearchKey);
                }
                else
                {
                    ShoppingList = _shoppingListService.ShoppingList;
                }
            }
        }

        public IList<ShoppingEntryDto> ShoppingList
        {
            get
            {
                return _shoppingList;
            }
            set
            {
                _shoppingList = value;
                OnPropertyChanged("ShoppingList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _shoppingListService.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished;

            if (_shoppingListService.ShoppingList == null)
            {
                _shoppingListService.LoadShoppingList();
                return;
            }

            ShoppingList = _shoppingListService.ShoppingList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _shoppingListService.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            _logger.Debug(string.Format("_onShoppingListDownloadFinished with model {0} was successful {1}", shoppingList, success));
            ShoppingList = _shoppingListService.ShoppingList;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonAdd_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.Navigate(_shoppingEntryAddPage);
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _shoppingListService.LoadShoppingList();
        }

        private void ButtonIncreaseEntryAmount_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonIncreaseEntryAmount_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int shoppingEntryId = (int)senderButton.Tag;
                ShoppingEntryDto shoppingEntry = _shoppingListService.GetById(shoppingEntryId);
                _logger.Warning(string.Format("Increasing amount of entry {0}!", shoppingEntry));
                shoppingEntry.IncreaseQuantity();

                _shoppingListService.UpdateShoppingEntry(shoppingEntry);
            }
        }

        private void ButtonDecreaseEntryAmount_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonDecreaseEntryAmount_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int shoppingEntryId = (int)senderButton.Tag;
                ShoppingEntryDto shoppingEntry = _shoppingListService.GetById(shoppingEntryId);
                _logger.Warning(string.Format("Decreasing amount of entry {0}!", shoppingEntry));
                shoppingEntry.DecreaseQuantity();

                _shoppingListService.UpdateShoppingEntry(shoppingEntry);
            }
        }

        private void ButtonDeleteEntry_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonDeleteEntry_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int shoppingEntryId = (int)senderButton.Tag;
                ShoppingEntryDto shoppingEntry = _shoppingListService.GetById(shoppingEntryId);
                _logger.Warning(string.Format("Asking for deleting shopping entry {0}!", shoppingEntry));

                DeleteDialog shoppingEntryDeleteDialog = new DeleteDialog("Delete shopping entry?",
                    string.Format("Entry: {0}\nGroup: {1}\nQuantity: {2}", shoppingEntry.Name, shoppingEntry.Group, shoppingEntry.Quantity));
                shoppingEntryDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                shoppingEntryDeleteDialog.ShowDialog();

                var confirmDelete = shoppingEntryDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    _shoppingListService.DeleteShoppingEntry(shoppingEntry);
                }
            }
        }
    }
}
