using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace LucaHome.UserControls
{
    /// <summary>
    /// Interaction logic for MenuAndToolBars.xaml
    /// </summary>
    public partial class MenuAndToolBars : UserControl
    {
        public MenuAndToolBars()
        {
            InitializeComponent();
        }

        private void TwitterButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitter.com/James_Willock");
        }
    }
}
