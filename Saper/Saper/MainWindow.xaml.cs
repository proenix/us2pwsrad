using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
            SetLanguageDictionary();
        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "pl-PL":
                    dict.Source = new Uri("..\\Resources\\StringResources.pl-PL.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Start new game
        }

        // Open options window.
        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            // TODO: Open options window
        }

        // Exit application from main menu.
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Open Help window.
        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            // TODO: Open help window
            WindowHelp windowHelp = new WindowHelp();
            windowHelp.Show();
        }

        // Open About window.
        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            // TODO: Open about window
        }

        // Intercept Window Closing mechanism. 
        /// <summary>
        /// Ask user if Application should be closed. If user answer is No, cancel app closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var response = MessageBox.Show("Do you really want to exit?", "Exiting...",
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (response == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
