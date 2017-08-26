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

namespace LucaHome.Pages
{
    public partial class NovelPage : Page, INotifyPropertyChanged
    {
        private const string TAG = "NovelPage";
        private readonly Logger _logger;
        
        private readonly NavigationService _navigationService;
        private readonly NovelService _novelService;

        private string _novelSearchKey = string.Empty;

        private const int MAX_GRID_COLUMNS = 6;
        private const int MAX_GRID_ROWS = 6;

        public NovelPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _novelService = NovelService.Instance;

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

                if (_novelSearchKey != string.Empty)
                {
                    createUI(_novelService.FoundNovelDtos(_novelSearchKey));
                }
                else
                {
                    createUI(_novelService.NovelList);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _novelService.OnNovelListDownloadFinished += _onNovelListDownloadFinished;

            if (_novelService.NovelList == null)
            {
                _novelService.LoadNovelList();
                return;
            }

            createUI(_novelService.NovelList);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _novelService.OnNovelListDownloadFinished -= _onNovelListDownloadFinished;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _navigationService.GoBack();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _novelService.LoadNovelList();
        }

        private void _onNovelListDownloadFinished(IList<NovelDto> novelList, bool success, string response)
        {
            _logger.Debug(string.Format("_onNovelListDownloadFinished with object {0} and response {1} was successful {2}", novelList, response, success));
            createUI(_novelService.NovelList);
        }

        private void createUI(IList<NovelDto> novelList)
        {
            _logger.Debug(string.Format("createUI with list {0}", novelList));
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
                        _novelService.OpenExplorer(entry.Author);
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
