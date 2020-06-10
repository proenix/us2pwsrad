﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
        private SaperGame game;

        public MainWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();

            this.game = new SaperGame(mineField);
            this.game.setOptions(10, 10, 20);
            this.game.SetReferences(labelMoves, labelTimer);
            this.game.startGame();
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

        // Start new game with current options.
        private void MenuItem_Click_NewGame(object sender, RoutedEventArgs e)
        {
            this.game.startGame();
        }

        // Open options window.
        private void MenuItem_Click_Options(object sender, RoutedEventArgs e)
        {
            WindowOptions windowOptions = new WindowOptions(this, game.getSize(), game.getMines());
            windowOptions.Show();
        }

        // Exit application from main menu.
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Open Help window.
        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            WindowHelp windowHelp = new WindowHelp();
            windowHelp.Show();
        }

        // Open About window.
        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            WindowAbout windowAbout = new WindowAbout();
            windowAbout.Show();
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

        // Pass saving options to game object.
        internal void setGameOptions(int cols, int rows, int mines)
        {
            this.game.setOptions(cols, rows, mines);
        }
    }
}
