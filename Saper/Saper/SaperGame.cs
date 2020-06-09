using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

                    // TODO remove line below
                    label.Content = mineField[row, col].numberOfNeighbourMines.ToString();
                    label.Foreground = getTextBrushColor(mineField[row, col].numberOfNeighbourMines);
                }
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
