using System.Windows;

namespace LucaHome.Dialogs
{
    public enum Action { NULL, CONFIRM, CANCEL };

    public partial class SetDialog : Window
    {
        private const string TAG = "SetDialog";

        private Action _setAction = Action.NULL;

        public SetDialog(string title, string description, string confirmButtonText, string cancelButtonText)
        {
            InitializeComponent();

            Title.Text = title;
            Description.Text = description;

            ConfirmButton.Content = confirmButtonText;
            CancelButton.Content = cancelButtonText;
        }

        public SetDialog(string title, string description) : this(title, description, "Confirm", "Cancel")
        { }

        public Action SetAction
        {
            get
            {
                return _setAction;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _setAction = Action.CONFIRM;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _setAction = Action.CANCEL;
            Close();
        }
    }
}
