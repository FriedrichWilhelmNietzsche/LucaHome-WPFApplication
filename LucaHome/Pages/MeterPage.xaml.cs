using Common.Dto;
using Common.Enums;
using Data.Services;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

/*
 * Really helpful link
 * https://www.dotnetperls.com/listview-wpf
 * https://stackoverflow.com/questions/2796470/wpf-create-a-dialog-prompt
 * https://stackoverflow.com/questions/1545258/changing-the-start-up-location-of-a-wpf-window
 * https://lvcharts.net/
 */

namespace LucaHome.Pages
{
    public partial class MeterPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MeterPage";

        private readonly NavigationService _navigationService;

        private string _meterDataSearchKey;
        private IList<MeterDataDto> _activeMeterDataList = new List<MeterDataDto>();

        public MeterPage(NavigationService navigationService)
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

        public CollectionView MeterList
        {
            get
            {
                IList<string> meterList = new List<string>();
                foreach (MeterDto entry in MeterDataService.Instance.MeterList)
                {
                    meterList.Add(entry.MeterId);
                }
                return new CollectionView(meterList);
            }
        }

        public string ActiveMeter
        {
            get
            {
                return MeterDataService.Instance.ActiveMeterInView.MeterId;
            }
            set
            {
                string meterId = value;
                MeterDto activeMeter = MeterDataService.Instance.GetByMeterId(meterId);
                MeterDataService.Instance.ActiveMeterInView = activeMeter;
                OnPropertyChanged("ActiveMeter");
                updateView(false);
            }
        }

        public string MeterDataSearchKey
        {
            get
            {
                return _meterDataSearchKey;
            }
            set
            {
                _meterDataSearchKey = value;
                IList<MeterDataDto> foundMeterDataList = MeterDataService.Instance.FoundMeterDataDto(_meterDataSearchKey);
                ActiveMeterDataList = foundMeterDataList;
                OnPropertyChanged("MeterDataSearchKey");
            }
        }

        public IList<MeterDataDto> ActiveMeterDataList
        {
            get
            {
                return _activeMeterDataList;
            }
            set
            {
                _activeMeterDataList = value;
                OnPropertyChanged("ActiveMeterDataList");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            updateView(true);
            MeterDataService.Instance.OnMeterDownloadFinished += _onMeterDownloadFinished;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MeterDataService.Instance.OnMeterDownloadFinished -= _onMeterDownloadFinished;
        }

        private void _onMeterDownloadFinished(IList<MeterDto> meterList, bool success, string response)
        {
            updateView(true);
        }

        private void ButtonUpdateEntry_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                int meterDataId = (int)senderButton.Tag;
                MeterDataDto meterData = MeterDataService.Instance.GetById(meterDataId);
                _navigationService.Navigate(new MeterEditPage(_navigationService, meterData, EditAction.UPDATE));
            }
        }

        private void ButtonDeleteEntry_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                int meterDataId = (int)senderButton.Tag;
                MeterDataDto meterData = MeterDataService.Instance.GetById(meterDataId);
                MeterDataService.Instance.DeleteMeterData(meterData);
            }
        }

        private void ButtonAddMeter_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.Navigate(new MeterEditPage(_navigationService,
                new MeterDataDto(
                    MeterDataService.Instance.MeterDataList.Count,
                    "",
                    MeterDataService.Instance.GetHighestMeterDataTypeId(),
                    new DateTime(),
                    "",
                    "",
                    0,
                    ""), EditAction.ADD));
        }

        private void ButtonAddMeterData_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            MeterDto activeMeter = MeterDataService.Instance.ActiveMeterInView;
            _navigationService.Navigate(new MeterEditPage(_navigationService,
                new MeterDataDto(
                    MeterDataService.Instance.MeterDataList.Count,
                    activeMeter.Type,
                    activeMeter.TypeId,
                    new DateTime(),
                    activeMeter.MeterId,
                    activeMeter.Area,
                    0,
                    ""), EditAction.ADD));
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            MeterDataService.Instance.LoadMeterDataList();
        }

        private void updateView(bool setActiveMeter)
        {
            MeterDataSearchKey = "";
            if (setActiveMeter)
            {
                ActiveMeter = MeterDataService.Instance.ActiveMeterInView.MeterId;
            }
            ActiveMeterDataList = MeterDataService.Instance.ActiveMeterInView.MeterDataList;

            MeterGraphCard.GraphMeterId = ActiveMeter;
            MeterGraphCard.GraphMeterType = MeterDataService.Instance.ActiveMeterInView.Type;
            MeterGraphCard.GraphMeterDetails = string.Format(
                "TypeId: {0}\nArea: {1}",
                MeterDataService.Instance.ActiveMeterInView.TypeId,
                MeterDataService.Instance.ActiveMeterInView.Area);

            List<string> labelList = new List<string>();
            ChartValues<double> chartValues = new ChartValues<double>();
            foreach (MeterDataDto meterData in ActiveMeterDataList)
            {
                labelList.Add(meterData.SaveDate.ToShortDateString());
                chartValues.Add(meterData.Value);
            }
            MeterGraphCard.GraphLabels = labelList.ToArray();

            MeterGraphCard.GraphSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = ActiveMeter,
                    Values = chartValues
                }
            };
        }
    }
}
