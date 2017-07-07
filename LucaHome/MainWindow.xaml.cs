using Common.Common;
using Common.Tools;
using Data.Services;
using LucaHome.Pages;
using OpenWeather.Service;
using System.Windows;
using System.Windows.Navigation;

namespace LucaHome
{
    public partial class MainWindow : Window
    {
        private const string TAG = "MainWindow";
        private Logger _logger;

        private AppSettingsService _appSettingsService;
        private NavigationService _navigationService;
        private OpenWeatherService _openWeatherService;

        private BootPage _bootPage;

        public MainWindow()
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();
            
            _navigationService = _mainFrame.NavigationService;

            _appSettingsService = AppSettingsService.Instance;
            _openWeatherService = OpenWeatherService.Instance;
            _openWeatherService.City = _appSettingsService.OpenWeatherCity;

            _bootPage = new BootPage(_navigationService);

            _navigationService.Navigate(_bootPage);
        }
    }
}
