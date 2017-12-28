using Common.Dto;
using Common.Tools;
using Data.Services;
using OpenWeather.Models;
using OpenWeather.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class BootPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "BootPage";

        private const int LISTENER_INDEX = 0;
        private const int ACTION_INDEX = 1;

        private int _downloadCount = 0;
        private readonly Dictionary<int, Action[]> _downloadActions = new Dictionary<int, Action[]>();
        private IList<string> _objectFinishedList = new List<string>();

        private readonly NavigationService _navigationService;

        public BootPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();

            _downloadActions.Add(0, new Action[] { () => BirthdayService.Instance.OnBirthdayDownloadFinished += _birthdayDownloadFinished, BirthdayService.Instance.LoadBirthdayList });
            _downloadActions.Add(1, new Action[] { () => CoinService.Instance.OnCoinConversionDownloadFinished += _onCoinConversionDownloadFinished, CoinService.Instance.LoadCoinConversionList });
            _downloadActions.Add(2, new Action[] { () => CoinService.Instance.OnCoinDownloadFinished += _onCoinDownloadFinished, CoinService.Instance.LoadCoinList });
            _downloadActions.Add(3, new Action[] { () => LibraryService.Instance.OnMagazinListDownloadFinished += _onMagazinListDownloadFinished, LibraryService.Instance.LoadMagazinList });
            _downloadActions.Add(4, new Action[] { () => MenuService.Instance.OnListedMenuDownloadFinished += _onListedMenuDownloadFinished, MenuService.Instance.LoadListedMenuList });
            _downloadActions.Add(5, new Action[] { () => MenuService.Instance.OnMenuDownloadFinished += _onMenuDownloadFinished, MenuService.Instance.LoadMenuList });
            _downloadActions.Add(6, new Action[] { () => MovieService.Instance.OnMovieDownloadFinished += _movieDownloadFinished, MovieService.Instance.LoadMovieList });
            _downloadActions.Add(7, new Action[] { () => NovelService.Instance.OnNovelListDownloadFinished += _onNovelListDownloadFinished, NovelService.Instance.LoadNovelList });
            _downloadActions.Add(8, new Action[] { () => SecurityService.Instance.OnSecurityDownloadFinished += _onSecurityDownloadFinished, SecurityService.Instance.LoadSecurity });
            _downloadActions.Add(9, new Action[] { () => SeriesService.Instance.OnSeriesListDownloadFinished += _onSeriesListDownloadFinished, SeriesService.Instance.LoadSeriesList });
            _downloadActions.Add(10, new Action[] { () => ShoppingListService.Instance.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished, ShoppingListService.Instance.LoadShoppingList });
            _downloadActions.Add(11, new Action[] { () => SpecialicedBookService.Instance.OnSpecialicedBookListDownloadEventHandler += _onSpecialicedBookListDownloadFinished, SpecialicedBookService.Instance.LoadBookList });
            _downloadActions.Add(12, new Action[] { () => OpenWeatherService.Instance.OnCurrentWeatherDownloadFinished += _currentWeatherDownloadFinished, OpenWeatherService.Instance.LoadCurrentWeather });
            _downloadActions.Add(13, new Action[] { () => OpenWeatherService.Instance.OnForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished, OpenWeatherService.Instance.LoadForecastModel });
            _downloadActions.Add(14, new Action[] { () => TemperatureService.Instance.OnTemperatureDownloadFinished += _temperatureDownloadFinished, TemperatureService.Instance.LoadTemperatureList });
            _downloadActions.Add(15, new Action[] { () => WirelessSocketService.Instance.OnWirelessSocketDownloadFinished += _wirelessSocketDownloadFinished, WirelessSocketService.Instance.LoadWirelessSocketList });
            _downloadActions.Add(16, new Action[] { () => WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished += _wirelessSwitchDownloadFinished, WirelessSwitchService.Instance.LoadWirelessSwitchList });
            _downloadActions.Add(17, new Action[] { () => ScheduleService.Instance.OnScheduleDownloadFinished += _scheduleDownloadFinished, ScheduleService.Instance.LoadScheduleList });
            _downloadActions.Add(18, new Action[] { () => MeterDataService.Instance.OnMeterDownloadFinished += _meterDownloadFinished, MeterDataService.Instance.LoadMeterDataList });
            _downloadActions.Add(19, new Action[] { () => MoneyMeterDataService.Instance.OnMoneyMeterDataDownloadFinished += _moneyMeterDataDownloadFinished, MoneyMeterDataService.Instance.LoadMoneyMeterDataList });
            _downloadActions.Add(20, new Action[] { () => MapContentService.Instance.OnMapContentDownloadFinished += _mapContentDownloadFinished, MapContentService.Instance.LoadMapContentList });

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ProgressText
        {
            get
            {
                return String.Format("Loading LucaHome WPF Application ... {0} %", (_downloadCount * 100) / _downloadActions.Count);
            }
            set
            {
                OnPropertyChanged("ProgressText");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            performDownload();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _downloadCount = 0;
        }

        private void _birthdayDownloadFinished(IList<BirthdayDto> birthdayList, bool success, string response)
        {
            BirthdayService.Instance.OnBirthdayDownloadFinished -= _birthdayDownloadFinished;
            checkDownloadCount("_birthdayDownloadFinished");
        }

        private void _onCoinConversionDownloadFinished(IList<KeyValuePair<string, double>> coinConversionList, bool success, string response)
        {
            CoinService.Instance.OnCoinConversionDownloadFinished -= _onCoinConversionDownloadFinished;
            checkDownloadCount("_onCoinConversionDownloadFinished");
        }

        private void _onCoinDownloadFinished(IList<CoinDto> coinList, bool success, string response)
        {
            CoinService.Instance.OnCoinDownloadFinished -= _onCoinDownloadFinished;
            checkDownloadCount("_onCoinDownloadFinished");
        }

        private void _onMagazinListDownloadFinished(IList<MagazinDirDto> magazinList, bool success, string response)
        {
            LibraryService.Instance.OnMagazinListDownloadFinished -= _onMagazinListDownloadFinished;
            checkDownloadCount("_onMagazinListDownloadFinished");
        }

        private void _onListedMenuDownloadFinished(IList<ListedMenuDto> listedMenuList, bool success, string response)
        {
            MenuService.Instance.OnListedMenuDownloadFinished -= _onListedMenuDownloadFinished;
            checkDownloadCount("_onListedMenuDownloadFinished");
        }

        private void _onMenuDownloadFinished(IList<MenuDto> menuList, bool success, string response)
        {
            MenuService.Instance.OnMenuDownloadFinished -= _onMenuDownloadFinished;
            checkDownloadCount("_onMenuDownloadFinished");
        }

        private void _movieDownloadFinished(IList<MovieDto> movieList, bool success, string response)
        {
            MovieService.Instance.OnMovieDownloadFinished -= _movieDownloadFinished;
            checkDownloadCount("_movieDownloadFinished");
        }

        private void _onNovelListDownloadFinished(IList<NovelDto> novelList, bool success, string response)
        {
            NovelService.Instance.OnNovelListDownloadFinished -= _onNovelListDownloadFinished;
            checkDownloadCount("_onNovelListDownloadFinished");
        }

        private void _onSecurityDownloadFinished(SecurityDto security, bool success, string response)
        {
            SecurityService.Instance.OnSecurityDownloadFinished -= _onSecurityDownloadFinished;
            checkDownloadCount("_onSecurityDownloadFinished");
        }

        private void _onSeriesListDownloadFinished(IList<SeriesDto> seriesList, bool success, string response)
        {
            SeriesService.Instance.OnSeriesListDownloadFinished -= _onSeriesListDownloadFinished;
            checkDownloadCount("_onSeriesListDownloadFinished");
        }

        private void _onShoppingListDownloadFinished(IList<ShoppingEntryDto> shoppingList, bool success, string response)
        {
            ShoppingListService.Instance.OnShoppingListDownloadFinished -= _onShoppingListDownloadFinished;
            checkDownloadCount("_onShoppingListDownloadFinished");
        }

        private void _onSpecialicedBookListDownloadFinished(IList<string> bookList, bool success, string response)
        {
            SpecialicedBookService.Instance.OnSpecialicedBookListDownloadEventHandler -= _onSpecialicedBookListDownloadFinished;
            checkDownloadCount("_onSpecialicedBookListDownloadFinished");
        }

        private void _currentWeatherDownloadFinished(WeatherModel currentWeather, bool success)
        {
            OpenWeatherService.Instance.OnCurrentWeatherDownloadFinished -= _currentWeatherDownloadFinished;
            checkDownloadCount("_currentWeatherDownloadFinished");
        }

        private void _forecastWeatherDownloadFinished(ForecastModel forecastWeather, bool success)
        {
            OpenWeatherService.Instance.OnForecastWeatherDownloadFinished -= _forecastWeatherDownloadFinished;
            checkDownloadCount("_forecastWeatherDownloadFinished");
        }

        private void _temperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success, string response)
        {
            TemperatureService.Instance.OnTemperatureDownloadFinished -= _temperatureDownloadFinished;
            checkDownloadCount("_temperatureDownloadFinished");
        }

        private void _wirelessSocketDownloadFinished(IList<WirelessSocketDto> wirelessSocketList, bool success, string response)
        {
            WirelessSocketService.Instance.OnWirelessSocketDownloadFinished -= _wirelessSocketDownloadFinished;
            checkDownloadCount("_wirelessSocketDownloadFinished");
        }

        private void _wirelessSwitchDownloadFinished(IList<WirelessSwitchDto> wirelessSwitchList, bool success, string response)
        {
            WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished -= _wirelessSwitchDownloadFinished;
            checkDownloadCount("_wirelessSwitchDownloadFinished");
        }

        private void _scheduleDownloadFinished(IList<ScheduleDto> scheduleList, bool success, string response)
        {
            ScheduleService.Instance.OnScheduleDownloadFinished -= _scheduleDownloadFinished;
            checkDownloadCount("_scheduleDownloadFinished");
        }

        private void _meterDownloadFinished(IList<MeterDto> meterList, bool success, string response)
        {
            MeterDataService.Instance.OnMeterDownloadFinished -= _meterDownloadFinished;
            checkDownloadCount("_meterDownloadFinished");
        }

        private void _moneyMeterDataDownloadFinished(IList<MoneyMeterDataDto> moneyMeterDataList, bool success, string response)
        {
            MoneyMeterDataService.Instance.OnMoneyMeterDataDownloadFinished -= _moneyMeterDataDownloadFinished;
            checkDownloadCount("_moneyMeterDataDownloadFinished");
        }

        private void _mapContentDownloadFinished(IList<MapContentDto> mapContentList, bool success, string response)
        {
            MapContentService.Instance.OnMapContentDownloadFinished -= _mapContentDownloadFinished;
            checkDownloadCount("_mapContentDownloadFinished");
        }

        private void checkDownloadCount(string objectFinished)
        {
            _downloadCount++;
            Logger.Instance.Debug(TAG, string.Format("checkDownloadCount: Download {0}/{1}", _downloadCount, _downloadActions.Count));

            _objectFinishedList.Add(objectFinished);
            Logger.Instance.Debug(TAG, string.Format("_objectFinishedList: {0}", String.Join(", ", _objectFinishedList)));

            OnPropertyChanged("ProgressText");

            if (_downloadCount >= _downloadActions.Count)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { _navigationService.Navigate(new MainPage(_navigationService)); }));
            }
            else
            {
                performDownload();
            }
        }

        private void performDownload()
        {
            Action currentRegisterAction = _downloadActions[_downloadCount][LISTENER_INDEX];
            if (currentRegisterAction != null)
            {
                currentRegisterAction.Invoke();

                Action currentDownloadAction = _downloadActions[_downloadCount][ACTION_INDEX];
                if (currentDownloadAction != null)
                {
                    currentDownloadAction.Invoke();
                    return;
                }
            }

            Logger.Instance.Error(TAG, "Failed to register or to perform an action!");
        }
    }
}
