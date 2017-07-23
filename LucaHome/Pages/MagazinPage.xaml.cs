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
    public partial class MagazinPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MagazinPage";
        private readonly Logger _logger;

        private readonly LibraryService _libraryService;
        private readonly NavigationService _navigationService;

        private string _magazinSearchKey = string.Empty;

        private const int MAX_GRID_COLUMNS = 6;
        private const int MAX_GRID_ROWS = 6;

        public MagazinPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _libraryService = LibraryService.Instance;
            _navigationService = navigationService;

            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string MagazinSearchKey
        {
            get
            {
                return _magazinSearchKey;
            }
            set
            {
                _magazinSearchKey = value;
                OnPropertyChanged("MagazinSearchKey");

                if (_magazinSearchKey != string.Empty)
                {
                    createUI(_libraryService.FoundMagazins(_magazinSearchKey));
                }
                else
                {
                    createUI(_libraryService.MagazinList);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _libraryService.OnMagazinListDownloadFinished += _onMagazinListDownloadFinished;

            if (_libraryService.MagazinList == null)
            {
                _libraryService.LoadMagazinList();
                return;
            }

            createUI(_libraryService.MagazinList);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _libraryService.OnMagazinListDownloadFinished -= _onMagazinListDownloadFinished;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _libraryService.LoadMagazinList();
        }

        private void _onMagazinListDownloadFinished(IList<MagazinDirDto> magazinList, bool success, string response)
        {
            _logger.Debug(string.Format("_onMagazinListDownloadFinished with object {0} and response {1} was successful {2}", magazinList, response, success));
            createUI(_libraryService.MagazinList);
        }

        private void createUI(IList<MagazinDirDto> magazinList)
        {
            _logger.Debug(string.Format("createUI with list {0}", magazinList));
            MagazinGrid.Children.Clear();

            int row = 0;
            int column = 0;

            foreach (MagazinDirDto entry in magazinList)
            {
                MagazinCard newMagazinCard = new MagazinCard();
                newMagazinCard.Title = entry.DirName;
                newMagazinCard.MagazinIcon = entry.Icon;
                newMagazinCard.ButtonOpenExplorerCommand = new DelegateCommand(
                    () =>
                    {
                        string command = string.Format(@"{0}\{1}", _libraryService.MagazinDir, entry.DirName);
                        Process.Start(command);
                    });
                newMagazinCard.MouseUpCommand = new DelegateCommand(
                    () =>
                    {
                        _navigationService.Navigate(new MagazinListPage(_navigationService, entry));
                    });

                Grid.SetColumn(newMagazinCard, column);
                Grid.SetRow(newMagazinCard, row);

                MagazinGrid.Children.Add(newMagazinCard);

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
