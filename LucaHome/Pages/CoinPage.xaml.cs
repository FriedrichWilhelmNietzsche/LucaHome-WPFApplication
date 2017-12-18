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
 * https://stackoverflow.com/questions/2796470/wpf-create-a-dialog-prompt
 * https://stackoverflow.com/questions/1545258/changing-the-start-up-location-of-a-wpf-window
 */

namespace LucaHome.Pages
{
    public partial class CoinPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "CoinPage";

        private readonly NavigationService _navigationService;

        private string _coinSearchKey = string.Empty;
        private IList<CoinDto> _coinList = new List<CoinDto>();

        public CoinPage(NavigationService navigationService)
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
                CoinList = CoinService.Instance.FoundCoins(_coinSearchKey);
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
                OnPropertyChanged("AllCoinValue");
            }
        }

        public string AllCoinValue
        {
            get
            {
                return CoinService.Instance.AllCoinsValue;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CoinService.Instance.OnCoinDownloadFinished += _coinListDownloadFinished;

            if (CoinService.Instance.CoinList == null)
            {
                CoinService.Instance.LoadCoinList();
                return;
            }

            CoinList = CoinService.Instance.CoinList;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CoinService.Instance.OnCoinDownloadFinished -= _coinListDownloadFinished;
        }

        private void ButtonUpdateCoin_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int coinId = (int)senderButton.Tag;
                CoinDto updateCoin = CoinService.Instance.GetById(coinId);

                CoinUpdatePage coinUpdatePage = new CoinUpdatePage(_navigationService, updateCoin);
                _navigationService.Navigate(coinUpdatePage);
            }
        }

        private void ButtonDeleteCoin_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;

                int coinId = (int)senderButton.Tag;
                CoinDto deleteCoin = CoinService.Instance.GetById(coinId);

                DeleteDialog coinDeleteDialog = new DeleteDialog("Delete coin?",
                    string.Format("Coin: {0}\nAmount: {1}\nUser: {2}", deleteCoin.Type, deleteCoin.Amount, deleteCoin.User));
                coinDeleteDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                coinDeleteDialog.ShowDialog();

                var confirmDelete = coinDeleteDialog.DialogResult;
                if (confirmDelete == true)
                {
                    CoinService.Instance.DeleteCoin(deleteCoin);
                }
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new CoinAddPage(_navigationService));
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            CoinService.Instance.LoadCoinConversionList();
            CoinService.Instance.LoadCoinList();
        }

        private void _coinListDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            CoinList = CoinService.Instance.CoinList;
        }
    }
}
