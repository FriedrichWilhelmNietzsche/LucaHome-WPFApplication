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
    public partial class BirthdayPage : Page
    {
        private const string TAG = "BirthdayPage";
        private readonly Logger _logger;

        private readonly NavigationService _navigationService;
        private readonly BirthdayService _birthdayService;

        public BirthdayPage(NavigationService navigationService)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            _navigationService = navigationService;
            _birthdayService = BirthdayService.Instance;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Loaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _birthdayService.OnBirthdayDownloadFinished += _birthdayListDownloadFinished;

            if (_birthdayService.BirthdayList == null)
            {
                _birthdayService.LoadBirthdayList();
                return;
            }

            setList();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("Page_Unloaded with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _birthdayService.OnBirthdayDownloadFinished -= _birthdayListDownloadFinished;
        }

        private void setList()
        {
            _logger.Debug("setList");

            BirthdayList.Items.Clear();

            foreach (BirthdayDto entry in _birthdayService.BirthdayList)
            {
                BirthdayList.Items.Add(entry);
            }
        }

        private void _birthdayListDownloadFinished(IList<BirthdayDto> birthdayList, bool success)
        {
            _logger.Debug(string.Format("_birthdayListDownloadFinished with model {0} was successful: {1}", birthdayList, success));
            setList();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonBack_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _navigationService.GoBack();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonAdd_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _logger.Warning("Not yet implemented...");
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ButtonReload_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));

            _birthdayService.LoadBirthdayList();
        }
    }
}
