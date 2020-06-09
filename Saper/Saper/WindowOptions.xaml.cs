using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Saper
{
    /// <summary>
    /// Interaction logic for WindowOptions.xaml
    /// </summary>
    public partial class WindowOptions : Window
    {
        private MainWindow mw;
        private int cols;
        private int rows;
        private int mines;

        public WindowOptions(MainWindow mw, (int, int) size, int mines)
        {
            this.mw = mw;
            this.cols = size.Item1;
            this.rows = size.Item2;
            this.mines = mines;
            InitializeComponent();
            SetLanguageDictionary();
            FillForm();
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

        // Check if input is number.
        private bool isNumberInputed(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            return regex.IsMatch(e.Text);
        }

        // Prepopulate fields with current options.
        private void FillForm() 
        {
            optionsCols.Text = cols.ToString();
            optionsRows.Text = rows.ToString();
            optionsNumberOfMines.Text = mines.ToString();
        }

        private void optionsRows_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = isNumberInputed(e);
        }

        private void optionsCols_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = isNumberInputed(e);
        }

        private void optionsNumberOfMines_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = isNumberInputed(e);
        }

        // Process saving options. Pass data to game object in main window.
        private void optionsSave_Click(object sender, RoutedEventArgs e)
        {
            int nMines = 0;
            int nRows = 0;
            int nCols = 0;
            try
            {
                nMines = int.Parse(optionsNumberOfMines.Text);
                nRows = int.Parse(optionsRows.Text);
                nCols = int.Parse(optionsCols.Text);
            }
            catch (Exception){}

            // Check rows conditions.
            if (nRows <= 2)
            {
                MessageBox.Show(this.FindResource("_optionsRowsTooLowError").ToString());
                return;
            }
            else if (nRows > 100)
            {
                MessageBox.Show(this.FindResource("_optionsRowsTooHighError").ToString());
                return;
            }

            // Check cols conditions.
            if (nCols <= 2)
            {
                MessageBox.Show(this.FindResource("_optionsColsTooLowError").ToString());
                return;
            }
            else if (nCols > 100)
            {
                MessageBox.Show(this.FindResource("_optionsColsTooLowError").ToString());
                return;
            }

            // Check number of mines conditions.
            if (nMines <= 0)
            {
                MessageBox.Show(this.FindResource("_optionsMinesTooLowError").ToString());
                return;
            }
            else if (nMines >= nRows * nCols)
            {
                MessageBox.Show(this.FindResource("_optionsMinesTooHighError").ToString());
                return;
            }

            // Save game object in main window 
            mw.setGameOptions(nCols, nRows, nMines);
            MessageBox.Show(this.FindResource("_optionsSavedMessage").ToString());
        }
    }
}
