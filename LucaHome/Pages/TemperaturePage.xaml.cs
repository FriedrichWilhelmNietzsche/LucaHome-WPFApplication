using Common.Dto;
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

        private readonly NavigationService _navigationService;

        public TemperaturePage(NavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            TemperatureService.Instance.OnTemperatureDownloadFinished += _temperatureDownloadFinished;

            if (TemperatureService.Instance.TemperatureList == null)
            {
                TemperatureService.Instance.LoadTemperatureList();
                return;
            }

            setList();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            TemperatureService.Instance.OnTemperatureDownloadFinished -= _temperatureDownloadFinished;
        }

        private void setList()
        {
            TemperatureList.Items.Clear();

            foreach (TemperatureDto entry in TemperatureService.Instance.TemperatureList)
            {
                TemperatureList.Items.Add(entry);
            }

            Wallpaper.ImageWallpaperSource = TemperatureService.Instance.Wallpaper;
        }

        private void _temperatureDownloadFinished(IList<TemperatureDto> temperatureList, bool success, string response)
        {
            setList();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            TemperatureService.Instance.LoadTemperatureList();
        }
    }
}
