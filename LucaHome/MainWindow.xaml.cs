using Common.Common;
using Common.Tools;
using Data.Controller;
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

        private readonly AppSettingsController _appSettingsController;
        private readonly NavigationService _navigationService;
        private readonly OpenWeatherService _openWeatherService;

        private readonly BootPage _bootPage;

        public MainWindow()
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();

            _appSettingsController = AppSettingsController.Instance;
            _navigationService = _mainFrame.NavigationService;
            _openWeatherService = OpenWeatherService.Instance;
            _openWeatherService.City = _appSettingsController.OpenWeatherCity;

            _bootPage = new BootPage(_navigationService);

            _navigationService.Navigate(_bootPage);
        }
    }
}
