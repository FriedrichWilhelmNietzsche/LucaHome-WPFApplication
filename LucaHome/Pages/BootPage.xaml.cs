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

        private int _downloadCount = 0;
        private readonly Dictionary<int, Action> _downloadActions = new Dictionary<int, Action>();
        private readonly Dictionary<int, Action> _registerActions = new Dictionary<int, Action>();
        private IList<string> _objectFinishedList = new List<string>();

        private readonly NavigationService _navigationService;

        public BootPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();

            _downloadActions.Add(0, BirthdayService.Instance.LoadBirthdayList);
            _downloadActions.Add(1, CoinService.Instance.LoadCoinConversionList);
            _downloadActions.Add(2, CoinService.Instance.LoadCoinList);
            _downloadActions.Add(3, LibraryService.Instance.LoadMagazinList);
            _downloadActions.Add(4, MenuService.Instance.LoadListedMenuList);
            _downloadActions.Add(5, MenuService.Instance.LoadMenuList);
            _downloadActions.Add(6, MovieService.Instance.LoadMovieList);
            _downloadActions.Add(7, NovelService.Instance.LoadNovelList);
            _downloadActions.Add(8, SecurityService.Instance.LoadSecurity);
            _downloadActions.Add(9, SeriesService.Instance.LoadSeriesList);
            _downloadActions.Add(10, ShoppingListService.Instance.LoadShoppingList);
            _downloadActions.Add(11, SpecialicedBookService.Instance.LoadBookList);
            _downloadActions.Add(12, OpenWeatherService.Instance.LoadCurrentWeather);
            _downloadActions.Add(13, OpenWeatherService.Instance.LoadForecastModel);
            _downloadActions.Add(14, TemperatureService.Instance.LoadTemperatureList);
            _downloadActions.Add(15, WirelessSocketService.Instance.LoadWirelessSocketList);
            _downloadActions.Add(16, WirelessSwitchService.Instance.LoadWirelessSwitchList);
            _downloadActions.Add(17, ScheduleService.Instance.LoadScheduleList);
            _downloadActions.Add(18, MapContentService.Instance.LoadMapContentList);
            _downloadActions.Add(19, MeterDataService.Instance.LoadMeterDataList);

            _registerActions.Add(0, () => BirthdayService.Instance.OnBirthdayDownloadFinished += _birthdayDownloadFinished);
            _registerActions.Add(1, () => CoinService.Instance.OnCoinConversionDownloadFinished += _onCoinConversionDownloadFinished);
            _registerActions.Add(2, () => CoinService.Instance.OnCoinDownloadFinished += _onCoinDownloadFinished);
            _registerActions.Add(3, () => LibraryService.Instance.OnMagazinListDownloadFinished += _onMagazinListDownloadFinished);
            _registerActions.Add(4, () => MenuService.Instance.OnListedMenuDownloadFinished += _onListedMenuDownloadFinished);
            _registerActions.Add(5, () => MenuService.Instance.OnMenuDownloadFinished += _onMenuDownloadFinished);
            _registerActions.Add(6, () => MovieService.Instance.OnMovieDownloadFinished += _movieDownloadFinished);
            _registerActions.Add(7, () => NovelService.Instance.OnNovelListDownloadFinished += _onNovelListDownloadFinished);
            _registerActions.Add(8, () => SecurityService.Instance.OnSecurityDownloadFinished += _onSecurityDownloadFinished);
            _registerActions.Add(9, () => SeriesService.Instance.OnSeriesListDownloadFinished += _onSeriesListDownloadFinished);
            _registerActions.Add(10, () => ShoppingListService.Instance.OnShoppingListDownloadFinished += _onShoppingListDownloadFinished);
            _registerActions.Add(11, () => SpecialicedBookService.Instance.OnSpecialicedBookListDownloadEventHandler += _onSpecialicedBookListDownloadFinished);
            _registerActions.Add(12, () => OpenWeatherService.Instance.OnCurrentWeatherDownloadFinished += _currentWeatherDownloadFinished);
            _registerActions.Add(13, () => OpenWeatherService.Instance.OnForecastWeatherDownloadFinished += _forecastWeatherDownloadFinished);
            _registerActions.Add(14, () => TemperatureService.Instance.OnTemperatureDownloadFinished += _temperatureDownloadFinished);
            _registerActions.Add(15, () => WirelessSocketService.Instance.OnWirelessSocketDownloadFinished += _wirelessSocketDownloadFinished);
            _registerActions.Add(16, () => WirelessSwitchService.Instance.OnWirelessSwitchDownloadFinished += _wirelessSwitchDownloadFinished);
            _registerActions.Add(17, () => ScheduleService.Instance.OnScheduleDownloadFinished += _scheduleDownloadFinished);
            _registerActions.Add(18, () => MapContentService.Instance.OnMapContentDownloadFinished += _mapContentDownloadFinished);
            _registerActions.Add(19, () => MeterDataService.Instance.OnMeterDataDownloadFinished += _meterDataDownloadFinished);

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

        private void _mapContentDownloadFinished(IList<MapContentDto> mapContentList, bool success, string response)
        {
            MapContentService.Instance.OnMapContentDownloadFinished -= _mapContentDownloadFinished;
            checkDownloadCount("_mapContentDownloadFinished");
        }

        private void _meterDataDownloadFinished(IList<MeterDataDto> meterDataList, bool success, string response)
        {
            MeterDataService.Instance.OnMeterDataDownloadFinished -= _meterDataDownloadFinished;
            checkDownloadCount("_meterDataDownloadFinished");
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
            Action currentRegisterAction = _registerActions[_downloadCount];
            if (currentRegisterAction != null)
            {
                currentRegisterAction.Invoke();

                Action currentDownloadAction = _downloadActions[_downloadCount];
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
