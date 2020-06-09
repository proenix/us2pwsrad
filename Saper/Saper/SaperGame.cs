using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Saper
{
    class SaperGame
    {
        private Random rng;
        private int cols = 10;
        private int rows = 10;
        private int mines = 16;

        const int FIELD_WIDTH = 30;

        private Grid mineFieldLayout;
        private MineField[,] mineField;

        // Define structore of single mine field.
        private struct MineField
        {
            public System.Windows.Controls.Label field;
            public FieldStatus fieldStatus;
            public bool isMine;
            public int numberOfNeighbourMines;
        }

        // Holds predefined types of possible in-game field state
        private enum FieldStatus
        {
            open, unopen, flag, questionMark
        }

        // Initialize class object for game.
        public SaperGame(Grid mineFieldLayout)
        {
            this.rng = new Random();
            this.mineFieldLayout = mineFieldLayout;
        }

        // Initilaize row and column definitions for game field.
        private void initializeFields(int cols, int rows)
        {
            mineFieldLayout.RowDefinitions.Clear();
            for (int row = 0; row < rows; row++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                mineFieldLayout.RowDefinitions.Add(rowDefinition);
            }
            mineFieldLayout.ColumnDefinitions.Clear();
            for (int col = 0; col < cols; col++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                mineFieldLayout.ColumnDefinitions.Add(columnDefinition);
            }
        }

        public int getMines()
        {
            return this.mines;
        }

        public (int, int) getSize()
        {
            return (this.rows, this.cols);
        }

        // Set options for game.
        public void setOptions(int cols, int rows, int mines)
        {
            this.cols = cols;
            this.rows = rows;
            this.mines = mines;
        }

        // Generate empty mine field and populate it with mines.
        public void generateField()
        {
            this.mineField = new MineField[rows, cols];

            // Populate field with number of mines
            for (int mine = 0; mine <= mines; mine++)
            {
                int row = rng.Next(0, rows);
                int col = rng.Next(0, cols);
                if (mineField[row, col].isMine == true)
                {
                    mine--;
                }
                else
                {
                    mineField[row, col].isMine = true;
                }
            }

            // Initialize field
            initializeFields(cols, rows);
            mineFieldLayout.Width = cols * FIELD_WIDTH;
            mineFieldLayout.Height = rows * FIELD_WIDTH;
            mineFieldLayout.ShowGridLines = false;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    System.Windows.Controls.Label label = new System.Windows.Controls.Label
                    {
                        Width = FIELD_WIDTH,
                        Height = FIELD_WIDTH,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        //label.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/mine_icon.png")));
                        //label.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/flag.png")));
                        Background = Brushes.LightGray
                    };
                    label.MouseUp += new MouseButtonEventHandler(FieldMouseUp);
                    label.Tag = (row, col);
                    if (mineField[row,col].isMine == true)
                    {
                        label.Background = Brushes.Crimson;
                    }
                    Grid.SetColumn(label, col);
                    Grid.SetRow(label, row);
                    mineFieldLayout.Children.Add(label);
                    mineField[row, col].field = label;
                    mineField[row, col].fieldStatus = FieldStatus.unopen;
                    mineField[row, col].numberOfNeighbourMines = countNeighbourMines(row, col);
                } 
            }
        }

        private void FieldMouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label) sender;
            (int, int) pos = ((int, int)) label.Tag;
            int row = pos.Item1;
            int col = pos.Item2;

            MineField mf = mineField[row, col];
            // Already open do nothing.
            if (mineField[row, col].fieldStatus == FieldStatus.open)
                return;

            // Check mouse click actions
            if (e.ChangedButton == MouseButton.Left)
            {
                if (mineField[row, col].isMine == true)
                {
                    MessageBox.Show("GAME OVER.");
                    mineField[row, col].fieldStatus = FieldStatus.open;
                    revealField(row, col);

                    // TODO: Finish game.
                } 
                else
                {
                    mineField[row, col].fieldStatus = FieldStatus.open;
                    revealField(row, col);
                }
            } 
            else if (e.ChangedButton == MouseButton.Middle)
            {
                // TODO
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (mineField[row, col].fieldStatus == FieldStatus.unopen)
                {
                    mineField[row, col].fieldStatus = FieldStatus.flag;
                    revealField(row, col);
                } 
                else if (mineField[row, col].fieldStatus == FieldStatus.flag)
                {
                    mineField[row, col].fieldStatus = FieldStatus.questionMark;
                    revealField(row, col);
                }
                else
                {
                    mineField[row, col].fieldStatus = FieldStatus.unopen;
                    revealField(row, col);
                }
            }
            winCheck();
            // MessageBox.Show(label.Tag.ToString());
        }

        // Check for win conditions.
        private void winCheck()
        {
            return;
        }

        // Reveal current field and near fileds if empty.
        private void revealField(int row, int col)
        {
            // ref MineField mf = ref mineField[row, col];
            MessageBox.Show("D");
            if (mineField[row, col].fieldStatus == FieldStatus.open)
            {
                if (mineField[row, col].isMine == true)
                {
                    mineField[row, col].field.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/mine_icon_red.png")));
                }
                else
                {
                    mineField[row, col].field.Content = mineField[row, col].numberOfNeighbourMines.ToString();
                    mineField[row, col].field.Foreground = getTextBrushColor(mineField[row, col].numberOfNeighbourMines);
                    mineField[row, col].field.Background = Brushes.White;
                }

                // TODO: implement revealing near fields
            } 
            else if (mineField[row, col].fieldStatus == FieldStatus.flag)
            {
                mineField[row, col].field.Background = Brushes.LightGray;
                mineField[row, col].field.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/flag.png")));
            }
            else if (mineField[row, col].fieldStatus == FieldStatus.questionMark)
            {
                mineField[row, col].field.Content = "?";
                mineField[row, col].field.Background = Brushes.LightGray;
                mineField[row, col].field.Foreground = Brushes.DarkCyan;
            } 
            else
            {
                mineField[row, col].field.Background = Brushes.LightGray;
                mineField[row, col].field.Content = "";
            }
        }

        // Count mines in neighbour fields.
        private int countNeighbourMines(int row, int col)
        {
            if (mineField[row, col].isMine == true)
                return 9;

            int startPosRow = (row - 1 < 0) ? row : (row - 1);
            int startPosCol = (col - 1 < 0) ? col : (col - 1);
            int endPosRow = (row + 1 >= rows) ? row : (row + 1);
            int endPosCol = (col + 1 >= cols) ? col : (col + 1);

            int mineCount = 0;
            for (int rowNumber = startPosRow; rowNumber <= endPosRow; rowNumber++)
            {
                for (int colNumber = startPosCol; colNumber <= endPosCol; colNumber++) 
                {
                    if (mineField[rowNumber, colNumber].isMine == true)
                    {
                        mineCount++;
                    }
                }
            }
            return mineCount;
        }

        private Brush getTextBrushColor(int numberOfMines)
        {
            switch(numberOfMines)
            {
                case 0:
                    return Brushes.White;
                case 1:
                    return Brushes.Blue;
                case 2:
                    return Brushes.Green;
                case 3:   
                    return Brushes.Red;
                case 4:
                    return Brushes.DarkBlue;
                default:
                    return Brushes.Maroon;
            }
        }
    }
}
