using Common.Common;
using Common.Dto;
using Common.Tools;
using Data.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LucaHome.UserControls;
using Microsoft.Practices.Prism.Commands;
using System.Diagnostics;

namespace LucaHome.Pages
{
    public partial class SeriesPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "SeriesPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly SeriesService _seriesService;

        private string _seriesSearchKey = string.Empty;

        private const int MAX_GRID_COLUMNS = 6;
        private const int MAX_GRID_ROWS = 6;

        public SeriesPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _seriesService = SeriesService.Instance;

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SeriesSearchKey
        {
            get
            {
                return _seriesSearchKey;
            }
            set
            {
                _seriesSearchKey = value;
                OnPropertyChanged("SeriesSearchKey");

                if (_seriesSearchKey != string.Empty)
                {
                    createUI(_seriesService.FoundSeries(_seriesSearchKey));
                }
                else
                {
                    createUI(_seriesService.SeriesList);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _seriesService.OnSeriesListDownloadFinished += _onSeriesListDownloadFinished;

            if (_seriesService.SeriesList == null)
            {
                _seriesService.LoadSeriesList();
                return;
            }

            createUI(_seriesService.SeriesList);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _seriesService.OnSeriesListDownloadFinished -= _onSeriesListDownloadFinished;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _seriesService.LoadSeriesList();
        }

        private void _onSeriesListDownloadFinished(IList<SeriesDto> seriesList, bool success, string response)
        {
            _logger.Debug(string.Format("_onSeriesListDownloadFinished with object {0} and response {1} was successful {2}", seriesList, response, success));
            createUI(_seriesService.SeriesList);
        }

        private void createUI(IList<SeriesDto> seriesList)
        {
            _logger.Debug(string.Format("createUI with list {0}", seriesList));
            SeriesGrid.Children.Clear();

            int row = 0;
            int column = 0;

            foreach (SeriesDto entry in seriesList)
            {
                SeriesCard newSeriesCard = new SeriesCard();
                newSeriesCard.SeriesName = entry.SeriesName;
                newSeriesCard.SeriesIcon = entry.Icon;
                newSeriesCard.ButtonOpenExplorerCommand = new DelegateCommand(
                    () =>
                    {
                        string command = string.Format(@"{0}\{1}", _seriesService.SeriesDir, entry.SeriesName);
                        Process.Start(command);
                    });
                newSeriesCard.MouseUpCommand = new DelegateCommand(
                    () =>
                    {
                        _navigationService.Navigate(new SeriesListPage(_navigationService, entry));
                    });

                Grid.SetColumn(newSeriesCard, column);
                Grid.SetRow(newSeriesCard, row);

                SeriesGrid.Children.Add(newSeriesCard);

                column++;
                if (column > MAX_GRID_COLUMNS)
                {
                    column = 0;
                    row++;
                }
                if (row > MAX_GRID_ROWS)
                {
                    // TODO currently break, add new row later
                    break;
                }
            }
        }
    }
}
