using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 */

namespace LucaHome.Pages
{
    public partial class ShoppingListPage : Page
    {
        private const string TAG = "ShoppingListPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly ShoppingListService _shoppingListService;

        private readonly ShoppingEntryAddPage _shoppingEntryAddPage;

        public ShoppingListPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _shoppingListService = ShoppingListService.Instance;

            _shoppingEntryAddPage = new ShoppingEntryAddPage(_navigationService);

            InitializeComponent();
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

            setList();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _shoppingListService.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
        }

        private void setList()
        {
            _logger.Debug("setList");

            ShoppingList.Items.Clear();

            foreach (ShoppingEntryDto entry in _shoppingListService.ShoppingList)
            {
                ShoppingList.Items.Add(entry);
            }
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success)
        {
            _logger.Debug(string.Format("_onShoppingListDownloadFinished with model {0} was successful {1}", shoppingList, success));

            setList();
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
    }
}
