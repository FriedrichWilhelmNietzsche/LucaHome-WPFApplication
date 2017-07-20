using Common.Common;
using Common.Dto;
using Common.Tools;
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
    public partial class CoinAddPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "CoinAddPage";
        private readonly Logger _logger;

        private readonly CoinService _coinService;
        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private CoinDto _newCoin;

        public CoinAddPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _coinService = CoinService.Instance;
            _navigationService = navigationService;

            _newCoin = new CoinDto(_coinService.CoinList.Count, "", "", 0, 0);

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

        public string CoinAmount
        {
            get
            {
                return _newCoin.Amount.ToString();
            }
            set
            {
                string amountString = value;
                double amount = 0;
                bool parseAmountSuccess = double.TryParse(amountString, out amount);
                if (parseAmountSuccess)
                {
                    _newCoin.Amount = amount;
                    OnPropertyChanged("CoinAmount");
                }
            }
        }

        public string CoinUser
        {
            get
            {
                return _newCoin.User;
            }
            set
            {
                _newCoin.User = value;
                OnPropertyChanged("CoinUser");
            }
        }

        public CollectionView TypeList
        {
            get
            {
                IList<string> typeList = new List<string>();
                typeList.Add("BTC");
                typeList.Add("DASH");
                typeList.Add("ETC");
                typeList.Add("ETH");
                typeList.Add("LTC");
                typeList.Add("XMR");
                typeList.Add("ZEC");
                return new CollectionView(typeList);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _coinService.OnCoinAddFinished -= _onCoinAddFinished;
            _coinService.OnCoinDownloadFinished -= _onCoinDownloadFinished;
        }

        private void SaveCoin_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("SaveCoin_Click with sender {0} and routedEventArgs: {1}", sender, routedEventArgs));
            _coinService.OnCoinAddFinished += _onCoinAddFinished;
            _logger.Debug(string.Format("Trying to add new coin {0}", _newCoin));
            _coinService.AddCoin(_newCoin);
        }

        private void _onCoinAddFinished(bool success, string response)
        {
            _logger.Debug(string.Format("_onCoinAddFinished was successful {0}", success));
            _coinService.OnCoinAddFinished -= _onCoinAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new coin!");

                _coinService.OnCoinDownloadFinished += _onCoinDownloadFinished;
                _coinService.LoadCoinList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding coin failed!\n{0}", response));
            }
        }

        private void _onCoinDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            _logger.Debug(string.Format("_onCoinDownloadFinished with model {0} was successful {1}", coinList, success));
            _coinService.OnCoinDownloadFinished -= _onCoinDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }
    }
}
