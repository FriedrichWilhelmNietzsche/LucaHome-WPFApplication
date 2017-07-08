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
        private readonly Logger _logger;

        private readonly AppSettingsService _appSettingsService;
        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;

        private readonly BootPage _bootPage;

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
