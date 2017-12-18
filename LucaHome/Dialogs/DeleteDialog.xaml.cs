using Common.Tools;
using System.Windows;

namespace LucaHome.Dialogs
{
    public partial class DeleteDialog : Window
    {
        private const string TAG = "DeleteDialog";

        public DeleteDialog(string title, string description)
        {
            InitializeComponent();

            Title.Text = title;
            Description.Text = description;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            Logger.Instance.Debug(TAG, string.Format("ConfirmButton_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            Logger.Instance.Debug(TAG, string.Format("CancelButton_Click with sender {0} and routedEventArgs {1}", sender, routedEventArgs));
            DialogResult = false;
            Close();
        }
    }
}
