using Common.Dto;
using Data.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LucaHome.UserControls;
using Microsoft.Practices.Prism.Commands;

namespace LucaHome.Pages
{
    public partial class SeriesPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "SeriesPage";

        private readonly NavigationService _navigationService;

        private string _seriesSearchKey = string.Empty;

        private const int MAX_GRID_COLUMNS = 6;
        private const int MAX_GRID_ROWS = 6;

        public SeriesPage(NavigationService navigationService)
        {
            _navigationService = navigationService;

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
                createUI(SeriesService.Instance.FoundSeries(_seriesSearchKey));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SeriesService.Instance.OnSeriesListDownloadFinished += _onSeriesListDownloadFinished;

            if (SeriesService.Instance.SeriesList == null)
            {
                SeriesService.Instance.LoadSeriesList();
                return;
            }

            createUI(SeriesService.Instance.SeriesList);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SeriesService.Instance.OnSeriesListDownloadFinished -= _onSeriesListDownloadFinished;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            SeriesService.Instance.LoadSeriesList();
        }

        private void _onSeriesListDownloadFinished(IList<SeriesDto> seriesList, bool success, string response)
        {
            createUI(SeriesService.Instance.SeriesList);
        }

        private void createUI(IList<SeriesDto> seriesList)
        {
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
                        SeriesService.Instance.OpenExplorer(entry.SeriesName);
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
                if (column >= MAX_GRID_COLUMNS)
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
