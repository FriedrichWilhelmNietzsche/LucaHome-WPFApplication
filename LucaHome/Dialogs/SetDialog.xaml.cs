using System.Windows;

namespace LucaHome.Dialogs
{
    public enum DialogAction { NULL, CONFIRM, CANCEL };

    public partial class SetDialog : Window
    {
        private const string TAG = "SetDialog";

        private DialogAction _setDialogAction = DialogAction.NULL;

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

        public DialogAction SetDialogAction
        {
            get
            {
                return _setDialogAction;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _setDialogAction = DialogAction.CONFIRM;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            _setDialogAction = DialogAction.CANCEL;
            Close();
        }
    }
}
