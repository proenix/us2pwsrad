using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Start new game
        }

        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            // TODO: Open options window
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            // TODO: Close game
        }

        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            // TODO: Open help window
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            // TODO: Open about window
        }
    }
}
