using Common.Common;
using Common.Tools;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LucaHome.Pages
{
    public partial class BootPage : Page
    {
        private const string TAG = "BootPage";
        private Logger _logger;

        private NavigationService _navigationService;

        // TODO Only for testing! Remove later!
        private Timer _exampleBootTimer = new Timer();

        public BootPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);
            _navigationService = navigationService;

            InitializeComponent();

            // TODO Only for testing! Remove later!
            _exampleBootTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _exampleBootTimer.Interval = 1500;
            _exampleBootTimer.Enabled = true;
        }

        // TODO Only for testing! Remove later!
        private void OnTimedEvent(object source, ElapsedEventArgs elapsedEventArgs)
        {
            _exampleBootTimer.Stop();
            Application.Current.Dispatcher.Invoke(new Action(() => { _navigationService.Navigate(new MainPage(_navigationService)); }));
        }
    }
}
