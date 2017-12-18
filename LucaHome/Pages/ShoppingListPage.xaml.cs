using Common.Dto;
using Data.Services;
using LucaHome.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

        private readonly NavigationService _navigationService;

        private string _shoppingListSearchKey = string.Empty;
        private IList<ShoppingEntryDto> _shoppingList = new List<ShoppingEntryDto>();

        public ShoppingListPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

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
                ShoppingList = ShoppingListService.Instance.FoundShoppingEntries(_shoppingListSearchKey);
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
            ShoppingListService.Instance.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished;

            if (ShoppingListService.Instance.ShoppingList == null)
            {
                ShoppingListService.Instance.LoadShoppingList();
                return;
            }

            ShoppingList = ShoppingListService.Instance.ShoppingList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ShoppingListService.Instance.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            ShoppingList = ShoppingListService.Instance.ShoppingList;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new ShoppingEntryAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            ShoppingListService.Instance.LoadShoppingList();
        }

        private void ButtonIncreaseEntryAmount_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int shoppingEntryId = (int)senderButton.Tag;
                ShoppingEntryDto shoppingEntry = ShoppingListService.Instance.GetById(shoppingEntryId);
                shoppingEntry.Quantity++;

                ShoppingListService.Instance.UpdateShoppingEntry(shoppingEntry);
            }
        }

        private void ButtonDecreaseEntryAmount_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int shoppingEntryId = (int)senderButton.Tag;
                ShoppingEntryDto shoppingEntry = ShoppingListService.Instance.GetById(shoppingEntryId);
                shoppingEntry.Quantity--;

                ShoppingListService.Instance.UpdateShoppingEntry(shoppingEntry);
            }
        }

        private void ButtonDeleteEntry_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int shoppingEntryId = (int)senderButton.Tag;
                ShoppingEntryDto shoppingEntry = ShoppingListService.Instance.GetById(shoppingEntryId);

                DeleteDialog shoppingEntryDeleteDialog = new DeleteDialog("Delete shopping entry?",
                    string.Format("Entry: {0}\nGroup: {1}\nQuantity: {2}", shoppingEntry.Name, shoppingEntry.Group, shoppingEntry.Quantity));
                shoppingEntryDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                shoppingEntryDeleteDialog.ShowDialog();

                var confirmDelete = shoppingEntryDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    ShoppingListService.Instance.DeleteShoppingEntry(shoppingEntry);
                }
            }
        }
    }
}
