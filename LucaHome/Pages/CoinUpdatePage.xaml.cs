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
    public partial class CoinUpdatePage : Page, INotifyPropertyChanged
    {
        private const string TAG = "CoinUpdatePage";

        private readonly NavigationService _navigationService;

        private readonly Notifier _notifier;

        private CoinDto _updateCoin;

        public CoinUpdatePage(NavigationService navigationService, CoinDto updateCoin)
        {
            _navigationService = navigationService;

            _updateCoin = updateCoin;

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

        public CollectionView TypeList
        {
            get
            {
                return new CollectionView(CoinService.Instance.TypeList);
            }
        }

        public string Type
        {
            get
            {
                return _updateCoin.Type;
            }
            set
            {
                _updateCoin.Type = value;
                OnPropertyChanged("Type");
            }
        }

        public double Amount
        {
            get
            {
                return _updateCoin.Amount;
            }
            set
            {
                _updateCoin.Amount = value;
                OnPropertyChanged("Amount");
            }
        }

        public string User
        {
            get
            {
                return _updateCoin.User;
            }
            set
            {
                _updateCoin.User = value;
                OnPropertyChanged("User");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CoinService.Instance.OnCoinUpdateFinished -= _onCoinUpdateFinished;
            CoinService.Instance.OnCoinDownloadFinished -= _onCoinDownloadFinished;
        }

        private void UpdateCoin_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            CoinService.Instance.OnCoinUpdateFinished += _onCoinUpdateFinished;
            CoinService.Instance.UpdateCoin(_updateCoin);
        }

        private void _onCoinUpdateFinished(bool success, string response)
        {
            CoinService.Instance.OnCoinAddFinished -= _onCoinUpdateFinished;

            if (success)
            {
                _notifier.ShowSuccess("Updated coin!");

                CoinService.Instance.OnCoinDownloadFinished += _onCoinDownloadFinished;
                CoinService.Instance.LoadCoinList();
            }
            else
            {
                _notifier.ShowError(string.Format("Updating coin failed!\n{0}", response));
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
