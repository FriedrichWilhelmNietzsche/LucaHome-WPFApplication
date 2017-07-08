using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 */

namespace LucaHome.Pages
{
    public partial class TemperaturePage : Page
    {
        private const string TAG = "TemperaturePage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly TemperatureService _temperatureService;

        public TemperaturePage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _temperatureService = TemperatureService.Instance;

            InitializeComponent();
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _temperatureService.OnTemperatureDownloadFinished += _temperatureDownloadFinished;

            if (_temperatureService.TemperatureList == null)
            {
                _temperatureService.LoadTemperatureList();
                return;
            }

            setList();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _temperatureService.OnTemperatureDownloadFinished -= _temperatureDownloadFinished;
        }

        private void setList()
        {
            _logger.Debug("setList");

            TemperatureList.Items.Clear();

            foreach (TemperatureDto entry in _temperatureService.TemperatureList)
            {
                TemperatureList.Items.Add(entry);
            }

            Wallpaper.ImageWallpaperSource = _temperatureService.Wallpaper;
        }

        private void _temperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success)
        {
            _logger.Debug(string.Format("_temperatureDownloadFinished with model {0} was successful: {1}", temperatureList, success));
            setList();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _temperatureService.LoadTemperatureList();
        }
    }
}
