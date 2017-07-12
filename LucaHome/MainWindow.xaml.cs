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

        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;
        private readonly TemperatureService _temperatureService;

        private readonly BootPage _bootPage;

        public MainWindow()
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();

            _navigationService = _mainFrame.NavigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _temperatureService = TemperatureService.Instance;
            _openWeatherService.City = _temperatureService.OpenWeatherCity;

            _bootPage = new BootPage(_navigationService);

            _navigationService.Navigate(_bootPage);
        }
    }
}
