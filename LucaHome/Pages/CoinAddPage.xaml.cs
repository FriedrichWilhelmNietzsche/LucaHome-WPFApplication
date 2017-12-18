using Common.Dto;
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

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private CoinDto _newCoin;

        public CoinAddPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            _newCoin = new CoinDto(CoinService.Instance.CoinList.Count, "", "", 0, 0, CoinDto.Trend.NULL);

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
                return new CollectionView(CoinService.Instance.TypeList);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CoinService.Instance.OnCoinAddFinished -= _onCoinAddFinished;
            CoinService.Instance.OnCoinDownloadFinished -= _onCoinDownloadFinished;
        }

        private void SaveCoin_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            CoinService.Instance.OnCoinAddFinished += _onCoinAddFinished;
            CoinService.Instance.AddCoin(_newCoin);
        }

        private void _onCoinAddFinished(bool success, string response)
        {
            CoinService.Instance.OnCoinAddFinished -= _onCoinAddFinished;

            if (success)
            {
                _notifier.ShowSuccess("Added new coin!");

                CoinService.Instance.OnCoinDownloadFinished += _onCoinDownloadFinished;
                CoinService.Instance.LoadCoinList();
            }
            else
            {
                _notifier.ShowError(string.Format("Adding coin failed!\n{0}", response));
            }
        }

        private void _onCoinDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            CoinService.Instance.OnCoinDownloadFinished -= _onCoinDownloadFinished;
            _navigationService.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }
    }
}
