using Common.Common;
using Common.Tools;
using System.Windows;

namespace LucaHome.Dialogs
{
    public partial class SetDialog : Window
    {
        public enum Action { NULL, ACTIVATE, DEACTIVATE};

        private const string TAG = "SetDialog";
        private readonly Logger _logger;

        private Action _setAction = Action.NULL;

        public SetDialog(string title, string description)
        {
            _logger = new Logger(TAG, Enables.LOGGING);

            InitializeComponent();

            Title.Text = title;
            Description.Text = description;
        }

        public Action SetAction
        {
            get
            {
                return _setAction;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("ConfirmButton_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _setAction = Action.ACTIVATE;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _logger.Debug(string.Format("CancelButton_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            _setAction = Action.DEACTIVATE;
            Close();
        }
    }
}
