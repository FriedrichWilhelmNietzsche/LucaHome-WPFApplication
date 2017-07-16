using Common.Common;
using Common.Dto;
using Common.Tools;
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
 * https://stackoverflow.com/questions/2796470/wpf-create-a-dialog-prompt
 * https://stackoverflow.com/questions/1545258/changing-the-start-up-location-of-a-wpf-window
 */

namespace LucaHome.Pages
{
    public partial class CoinPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "CoinPage";
        private readonly Logger _logger;

        private readonly CoinService _coinService;
        private readonly NavigationService _navigationService;

        private string _coinSearchKey = string.Empty;
        private IList<CoinDto> _coinList = new List<CoinDto>();

        public CoinPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _coinService = CoinService.Instance;
            _navigationService = navigationService;

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string CoinSearchKey
        {
            get
            {
                return _coinSearchKey;
            }
            set
            {
                _coinSearchKey = value;
                OnPropertyChanged("CoinSearchKey");

                if (_coinSearchKey != string.Empty)
                {
                    CoinList = _coinService.FoundCoins(_coinSearchKey);
                }
                else
                {
                    CoinList = _coinService.CoinList;
                }
            }
        }

        public IList<CoinDto> CoinList
        {
            get
            {
                return _coinList;
            }
            set
            {
                _coinList = value;
                OnPropertyChanged("CoinList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _coinService.OnCoinDownloadFinished += _coinListDownloadFinished;

            if (_coinService.CoinList == null)
            {
                _coinService.LoadCoinList();
                return;
            }

            CoinList = _coinService.CoinList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _coinService.OnCoinDownloadFinished -= _coinListDownloadFinished;
        }

        private void ButtonUpdateCoin_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonUpdateCoin_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int coinId = (int)senderButton.Tag;
                CoinDto updateCoin = _coinService.GetById(coinId);
                _logger.Warning(string.Format("Updating coin {0}!", updateCoin));

                CoinUpdatePage coinUpdatePage = new CoinUpdatePage(_navigationService, updateCoin);
                _navigationService.Navigate(coinUpdatePage);
            }
        }

        private void ButtonDeleteCoin_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonDeleteCoin_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                _logger.Debug(string.Format("Tag is {0}", senderButton.Tag));

                int coinId = (int)senderButton.Tag;
                CoinDto deleteCoin = _coinService.GetById(coinId);
                _logger.Warning(string.Format("Asking for deleting coin {0}!", deleteCoin));

                DeleteDialog coinDeleteDialog = new DeleteDialog("Delete coin?",
                    string.Format("Coin: {0}\nAmount: {1}\nUser: {2}", deleteCoin.Type, deleteCoin.Amount, deleteCoin.User));
                coinDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                coinDeleteDialog.ShowDialog();

                var confirmDelete = coinDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    _coinService.DeleteCoin(deleteCoin);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonAdd_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.Navigate(new CoinAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _coinService.LoadCoinList();
        }

        private void _coinListDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            _logger.Debug(string.Format("_coinListDownloadFinished with model {0} was successful: {1}", coinList, success));
            CoinList = _coinService.CoinList;
        }
    }
}
