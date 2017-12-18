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
    public partial class NovelPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "NovelPage";

        private readonly NavigationService _navigationService;

        private string _novelSearchKey = string.Empty;

        private const int MAX_GRID_COLUMNS = 6;
        private const int MAX_GRID_ROWS = 6;

        public NovelPage(NavigationService navigationService)
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

        public string NovelSearchKey
        {
            get
            {
                return _novelSearchKey;
            }
            set
            {
                _novelSearchKey = value;
                OnPropertyChanged("NovelSearchKey");
                createUI(NovelService.Instance.FoundNovelDtos(_novelSearchKey));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            NovelService.Instance.OnNovelListDownloadFinished += _onNovelListDownloadFinished;

            if (NovelService.Instance.NovelList == null)
            {
                NovelService.Instance.LoadNovelList();
                return;
            }

            createUI(NovelService.Instance.NovelList);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            NovelService.Instance.OnNovelListDownloadFinished -= _onNovelListDownloadFinished;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            NovelService.Instance.LoadNovelList();
        }

        private void _onNovelListDownloadFinished(IList<NovelDto> novelList, bool success, string response)
        {
            createUI(NovelService.Instance.NovelList);
        }

        private void createUI(IList<NovelDto> novelList)
        {
            NovelGrid.Children.Clear();

            int row = 0;
            int column = 0;

            foreach (NovelDto entry in novelList)
            {
                MagazinCard newMagazinCard = new MagazinCard();
                newMagazinCard.Title = entry.Author;
                newMagazinCard.MagazinIcon = entry.Icon;
                newMagazinCard.ButtonOpenExplorerCommand = new DelegateCommand(
                    () =>
                    {
                        NovelService.Instance.OpenExplorer(entry.Author);
                    });
                newMagazinCard.MouseUpCommand = new DelegateCommand(
                    () =>
                    {
                        _navigationService.Navigate(new NovelListPage(_navigationService, entry));
                    });

                Grid.SetColumn(newMagazinCard, column);
                Grid.SetRow(newMagazinCard, row);

                NovelGrid.Children.Add(newMagazinCard);

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
