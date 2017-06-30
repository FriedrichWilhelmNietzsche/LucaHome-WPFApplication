using Common.Common;
using Common.Tools;
using LucaHome.Pages;
using System.Windows;
using System.Windows.Navigation;

namespace LucaHome
{
    public partial class MainWindow : Window
    {
        private const string TAG = "MainWindow";
        private Logger _logger;

        private NavigationService _navigationService;

        private BootPage _bootPage;

        public MainWindow()
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();

            _navigationService = _mainFrame.NavigationService;
            _bootPage = new BootPage(_navigationService);

            _navigationService.Navigate(_bootPage);
        }
    }
}
