using Common.Common;
using Common.Tools;
using System.Windows;

namespace LucaHome.Dialogs
{
    public partial class DeleteDialog : Window
    {
        private const string TAG = "DeleteDialog";
        private readonly Logger _logger;

        public DeleteDialog(string title, string description)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();

            Title.Text = title;
            Description.Text = description;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ConfirmButton_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("CancelButton_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            DialogResult = false;
            Close();
        }
    }
}
