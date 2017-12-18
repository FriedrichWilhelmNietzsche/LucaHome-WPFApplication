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
    public partial class MagazinPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "MagazinPage";

        private readonly NavigationService _navigationService;

        private string _magazinSearchKey = string.Empty;

        private const int MAX_GRID_COLUMNS = 6;
        private const int MAX_GRID_ROWS = 6;

        public MagazinPage(NavigationService navigationService)
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
                createUI(LibraryService.Instance.FoundMagazins(_magazinSearchKey));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            LibraryService.Instance.OnMagazinListDownloadFinished += _onMagazinListDownloadFinished;

            if (LibraryService.Instance.MagazinList == null)
            {
                LibraryService.Instance.LoadMagazinList();
                return;
            }

            createUI(LibraryService.Instance.MagazinList);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            LibraryService.Instance.OnMagazinListDownloadFinished -= _onMagazinListDownloadFinished;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            LibraryService.Instance.LoadMagazinList();
        }

        private void _onMagazinListDownloadFinished(IList<MagazinDirDto> magazinList, bool success, string response)
        {
            createUI(LibraryService.Instance.MagazinList);
        }

        private void createUI(IList<MagazinDirDto> magazinList)
        {
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
                        LibraryService.Instance.OpenExplorer(entry.DirName);
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
