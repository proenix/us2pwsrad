using System;
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
        private int cols;
        private int rows;
        private int mines;

        private int field_size;

        private bool firstMoveDone = false;
        private bool gameEnded = false;

        private Grid mineFieldLayout;
        private System.Windows.Controls.Label minesLabel;
        private System.Windows.Controls.Label timerLabel;
        private System.Windows.Controls.Label faceLabel;

        private MineField[,] mineField;

        // Define structure of single mine field.
        private struct MineField
        {
            public System.Windows.Controls.Label field;
            public FieldStatus fieldStatus;
            public bool isMine;
            public int numberOfNeighbourMines;
        }

        private int _numberOfMoves;
        public int NumberOfMoves
        {
            get { return _numberOfMoves; }
            set
            {
                if (value != _numberOfMoves)
                {
                    _numberOfMoves = value;
                }
            }
        }

        private int _numberOfFlagsPlaced;
        public int NumberOfFlagsToBePlaced
        {
            get { return _numberOfFlagsPlaced; }
            set
            {
                if (value != _numberOfFlagsPlaced)
                {
                    _numberOfFlagsPlaced = value;
                    this.minesLabel.Content = value;
                }
            }
        }

        private int _timerCounter;
        public int TimerCounter
        {
            get { return _timerCounter; }
            set
            {
                if (value != _timerCounter)
                {
                    _timerCounter = value;
                    this.timerLabel.Content = value;
                }
            }
        }
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        internal void SetReferences(System.Windows.Controls.Label minesLabel, System.Windows.Controls.Label timerLabel, System.Windows.Controls.Label faceLabel)
        {
            this.minesLabel = minesLabel;
            this.timerLabel = timerLabel;
            this.faceLabel = faceLabel;
        }

        // Holds predefined types of possible in-game field state
        private enum FieldStatus
        {
            open, unopen, flag, questionMark
        }

        public SaperGame(Grid mineField)
        {
            this.rng = new Random();
            this.mineFieldLayout = mineField;
            this.dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            this.dispatcherTimer.Tick += dispatcherTimer_Tick;
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        // Initilaize row and column definitions for game field. Clear old data.
        private void initializeFields(int rows, int cols)
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
            mineFieldLayout.Children.Clear();
        }

        // Set options for game.
        public void setOptions(int rows, int cols, int mines)
        {
            this.rows = rows;
            this.cols = cols;
            this.mines = mines;
        }

        // Start new game with current options.
        public void startGame()
        {
            this.firstMoveDone = false;
            this.gameEnded = false;
            this.NumberOfMoves = 0;
            this.NumberOfFlagsToBePlaced = this.mines;
            this.TimerCounter = 0;
            this.dispatcherTimer.Stop();
            this.field_size = (300 / this.cols > 28) ? ((int)Math.Ceiling((float) (300 / this.cols))) : 28;
            this.faceLabel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/face_unclicked.png")));

            generateField();
        }

        // Generate empty mine field and populate it with mines.
        private void generateField()
        {
            this.mineField = new MineField[rows, cols];

            // Populate field with number of mines
            for (int mine = 0; mine < mines; mine++)
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
            initializeFields(rows, cols);
            mineFieldLayout.Width = cols * (field_size + 2);
            mineFieldLayout.Height = rows * (field_size + 2);
            mineFieldLayout.ShowGridLines = false;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    System.Windows.Controls.Label label = new System.Windows.Controls.Label
                    {
                        Width = field_size,
                        Height = field_size,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Background = Brushes.LightGray
                    };
                    label.MouseUp += new MouseButtonEventHandler(FieldMouseUp);
                    label.MouseDown += new MouseButtonEventHandler(FieldMouseDown);
                    label.MouseEnter += new MouseEventHandler(FieldMouseEnter);
                    label.MouseLeave += new MouseEventHandler(FieldMouseLeave);
                    label.Tag = (row, col);
                    //if (mineField[row,col].isMine == true)
                    //{
                    //    label.Background = Brushes.Crimson;
                    //}
                    Grid.SetColumn(label, col);
                    Grid.SetRow(label, row);
                    mineFieldLayout.Children.Add(label);
                    mineField[row, col].field = label;
                    mineField[row, col].fieldStatus = FieldStatus.unopen;
                    mineField[row, col].numberOfNeighbourMines = countNeighbourMines(row, col);
                } 
            }
        }

        // Process On Mouse Down action. Sends event to FieldMouseEnter to further processing.
        private void FieldMouseDown(object sender, MouseButtonEventArgs e)
        {
            FieldMouseEnter(sender, e);
        }

        // Process On Mouse Over Field Leave action. When LMB of MMB is hold resets affected fields.
        private void FieldMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.gameEnded == true)
                return;

            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            (int, int) pos = ((int, int))label.Tag;
            int row = pos.Item1;
            int col = pos.Item2;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                displayField(row, col);
            } 
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                int startPosRow = (row - 1 < 0) ? row : (row - 1);
                int startPosCol = (col - 1 < 0) ? col : (col - 1);
                int endPosRow = (row + 1 >= rows) ? row : (row + 1);
                int endPosCol = (col + 1 >= cols) ? col : (col + 1);
                for (int rowNumber = startPosRow; rowNumber <= endPosRow; rowNumber++)
                {
                    for (int colNumber = startPosCol; colNumber <= endPosCol; colNumber++)
                    {
                        if (mineField[rowNumber, colNumber].fieldStatus == FieldStatus.unopen)
                        {
                            displayField(rowNumber, colNumber);
                        }
                    }
                }
            }
            this.faceLabel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/face_unclicked.png")));
        }

        // Process On Mouse Over Field Enter action. When LMB of MMB is hold shows affected fields.
        private void FieldMouseEnter(object sender, MouseEventArgs e)
        {
            if (this.gameEnded == true)
                return;

            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            (int, int) pos = ((int, int))label.Tag;
            int row = pos.Item1;
            int col = pos.Item2;
            MineField field = mineField[row, col];

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (field.fieldStatus == FieldStatus.unopen)
                {
                    label.Background = Brushes.White;
                    this.faceLabel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/face_wut.png")));
                }
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                int startPosRow = (row - 1 < 0) ? row : (row - 1);
                int startPosCol = (col - 1 < 0) ? col : (col - 1);
                int endPosRow = (row + 1 >= rows) ? row : (row + 1);
                int endPosCol = (col + 1 >= cols) ? col : (col + 1);
                for (int rowNumber = startPosRow; rowNumber <= endPosRow; rowNumber++)
                {
                    for (int colNumber = startPosCol; colNumber <= endPosCol; colNumber++)
                    {
                        if (mineField[rowNumber, colNumber].fieldStatus == FieldStatus.unopen)
                        {
                            mineField[rowNumber, colNumber].field.Background = Brushes.White;
                        }
                    }
                }
                this.faceLabel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/face_wut.png")));
            }
        }        

        private void FieldMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Game already ended. Do nothing
            if (this.gameEnded == true)
                return;

            System.Windows.Controls.Label label = (System.Windows.Controls.Label) sender;
            (int, int) pos = ((int, int)) label.Tag;
            int row = pos.Item1;
            int col = pos.Item2;

            // Reset face to default
            this.faceLabel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/face_unclicked.png")));

            // Already open do nothing.
            if (e.ChangedButton != MouseButton.Middle && mineField[row, col].fieldStatus == FieldStatus.open)
                return;

            // Check mouse click actions
            if (e.ChangedButton == MouseButton.Left)
            {
                if (mineField[row, col].isMine == true)
                {
                    // Check for first move. First field should always be without mine.
                    if (this.firstMoveDone == false)
                    {
                        generateField();
                        FieldMouseUp(sender, e);
                        return;
                    }
                    else
                    {
                        this.dispatcherTimer.Stop();
                        this.gameEnded = true;
                        this.faceLabel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/face_dead.png")));
                        revealAll();
                        MessageBox.Show(Application.Current.FindResource("_saperGameOverMessage").ToString());
                    }
                }
                else
                {
                    revealNeighbours(row, col);
                }
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                if (mineField[row, col].fieldStatus == FieldStatus.open)
                    revealOnMiddle(row, col);
                else
                {
                    int startPosRow = (row - 1 < 0) ? row : (row - 1);
                    int startPosCol = (col - 1 < 0) ? col : (col - 1);
                    int endPosRow = (row + 1 >= rows) ? row : (row + 1);
                    int endPosCol = (col + 1 >= cols) ? col : (col + 1);
                    for (int rowNumber = startPosRow; rowNumber <= endPosRow; rowNumber++)
                    {
                        for (int colNumber = startPosCol; colNumber <= endPosCol; colNumber++)
                        {
                            if (mineField[rowNumber, colNumber].fieldStatus == FieldStatus.unopen)
                            {
                                displayField(rowNumber, colNumber);
                            }
                        }
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (mineField[row, col].fieldStatus == FieldStatus.unopen)
                {
                    mineField[row, col].fieldStatus = FieldStatus.flag;
                    NumberOfFlagsToBePlaced--;
                    displayField(row, col);
                } 
                else if (mineField[row, col].fieldStatus == FieldStatus.flag)
                {
                    NumberOfFlagsToBePlaced++;
                    mineField[row, col].fieldStatus = FieldStatus.questionMark;
                    displayField(row, col);
                }
                else
                {
                    mineField[row, col].fieldStatus = FieldStatus.unopen;
                    displayField(row, col);
                }
            }
            if (!this.firstMoveDone)
            {
                this.dispatcherTimer.Start();
                this.firstMoveDone = true;
            }
            NumberOfMoves++;

            if (winCheck())
            {
                gameEnded = true;
                this.dispatcherTimer.Stop();
                revealAll(true);
                NumberOfFlagsToBePlaced = 0;
                this.faceLabel.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/face_win.png")));
                MessageBox.Show(Application.Current.FindResource("_saperWinMessage").ToString() + NumberOfMoves.ToString());
            }
        }

        // Reveal neighbour fields if number of set flags is the same as number of neighbour mines.
        /// <summary>
        /// If flag is missplaced opens field anyway!
        /// </summary>
        /// <param name="row">Number of row to open.</param>
        /// <param name="col">Number of column to open.</param>
        /// <returns>Number of fields that were opened.</returns>
        private int revealOnMiddle(int row, int col)
        {
            // Game already ended. Do nothing
            if (this.gameEnded == true)
                return 0;

            // Count number of flags seted in near fields.
            int startPosRow = (row - 1 < 0) ? row : (row - 1);
            int startPosCol = (col - 1 < 0) ? col : (col - 1);
            int endPosRow = (row + 1 >= rows) ? row : (row + 1);
            int endPosCol = (col + 1 >= cols) ? col : (col + 1);
            int flagsCounter = 0;
            int showed = 0;

            // Count number of flags in neighbour fields.
            for (int rowNumber = startPosRow; rowNumber <= endPosRow; rowNumber++)
            {
                for (int colNumber = startPosCol; colNumber <= endPosCol; colNumber++)
                {
                    if (mineField[rowNumber, colNumber].fieldStatus == FieldStatus.flag)
                        flagsCounter++;
                }
            }
            
            // Process opening or displaying background back to normal.
            for (int rowNumber = startPosRow; rowNumber <= endPosRow; rowNumber++)
            {
                for (int colNumber = startPosCol; colNumber <= endPosCol; colNumber++)
                {
                    if (mineField[row, col].numberOfNeighbourMines == flagsCounter 
                        && mineField[rowNumber, colNumber].fieldStatus == FieldStatus.unopen)
                    {
                        FieldMouseUp(mineField[rowNumber, colNumber].field, new MouseButtonEventArgs(Mouse.PrimaryDevice, new TimeSpan(DateTime.Now.Ticks).Milliseconds, MouseButton.Left));
                        showed++;
                    } 
                    else
                    {
                        if (this.gameEnded == false)
                            displayField(rowNumber, colNumber);
                    }
                }
            }
            return showed;
        }

        // Increment Time Counter on each tick.
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.TimerCounter++;
        }

        // Check for win conditions.
        private bool winCheck()
        {
            int counterPlacedFlags = 0;
            int counterCorrectlyPlacedFlags = 0;
            int counterPlacedQuestionMarks = 0;
            int counterFieldsUnOpened = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    switch (mineField[row, col].fieldStatus)
                    {
                        case FieldStatus.unopen:
                            counterFieldsUnOpened++;
                            break;
                        case FieldStatus.flag:
                            counterPlacedFlags++;
                            if (mineField[row, col].isMine)
                                counterCorrectlyPlacedFlags++;
                            break;
                        case FieldStatus.questionMark:
                            counterPlacedQuestionMarks++;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (this.mines == counterFieldsUnOpened && counterPlacedFlags == 0 && counterPlacedQuestionMarks == 0)
                return true;

            if ((counterFieldsUnOpened + counterPlacedFlags + counterPlacedQuestionMarks) == this.mines)
                return true;

            if (counterCorrectlyPlacedFlags == counterPlacedFlags && counterCorrectlyPlacedFlags == this.mines && counterFieldsUnOpened == 0)
                return true;

            return false;
        }

        // Reveal current field and near fileds if empty.
        private void displayField(int row, int col, bool placeFlagOnMine = false)
        {
            // ref MineField mf = ref mineField[row, col];
            if (mineField[row, col].fieldStatus == FieldStatus.open)
            {
                if (mineField[row, col].isMine == true)
                {
                    if (placeFlagOnMine)
                    {
                        mineField[row, col].field.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/flag.png")));
                    } 
                    else
                    {
                        mineField[row, col].field.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/mine_icon_red.png")));
                    }
                }
                else
                {
                    mineField[row, col].field.Content = mineField[row, col].numberOfNeighbourMines.ToString();
                    mineField[row, col].field.Foreground = getTextBrushColor(mineField[row, col].numberOfNeighbourMines);
                    mineField[row, col].field.Background = Brushes.White;
                }
            } 
            else if (mineField[row, col].fieldStatus == FieldStatus.flag)
            {
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

        // Reveal neighbouring fields.
        /// <summary>
        /// Reveals neighbouring fields of provided field if current field has 0 mines in neighbouring fields.
        /// Can also reveal whole board if ignoreFieldStatus param is set to true.
        /// </summary>
        /// <param name="row">Row number of field that should be checked.</param>
        /// <param name="col">Column number of field that should be checked.</param>
        /// <param name="ignoreFieldStatus">Ignore number of mines and reveal everything.</param>
        private void revealNeighbours(int row, int col, bool ignoreFieldStatus = false, bool placeFlagOnMine = false)
        {
            // TODO: Optimize algorithm for revealing neighbour fields.
            // If opened alredy pass
            if (mineField[row, col].fieldStatus == FieldStatus.open)
                return;

            // Set current field as opened and display it.
            mineField[row, col].fieldStatus = FieldStatus.open;
            displayField(row, col, placeFlagOnMine);

            int startPosRow = (row - 1 < 0) ? row : (row - 1);
            int startPosCol = (col - 1 < 0) ? col : (col - 1);
            int endPosRow = (row + 1 >= rows) ? row : (row + 1);
            int endPosCol = (col + 1 >= cols) ? col : (col + 1);

            // Neighbouring mines count
            int neighbourMines= mineField[row, col].numberOfNeighbourMines;
            for (int rowNumber = startPosRow; rowNumber <= endPosRow; rowNumber++)
            {
                for (int colNumber = startPosCol; colNumber <= endPosCol; colNumber++)
                {
                    if (colNumber == col && rowNumber == row)
                    {
                        continue;
                    }
                    if (ignoreFieldStatus)
                    {
                        revealNeighbours(rowNumber, colNumber, ignoreFieldStatus, placeFlagOnMine);
                    }
                    else if (neighbourMines == 0)
                    {
                        revealNeighbours(rowNumber, colNumber, ignoreFieldStatus, placeFlagOnMine);
                    }
                }
            }
        }

        // Reveal all fields on board.
        /// <summary>
        /// If placeFlagOnMine param is set to true display flags where mines were placed.
        /// </summary>
        /// <param name="placeFlagOnMine">If true display flags icon instead of mine.</param>
        private void revealAll(bool placeFlagOnMine = false)
        {
            // If all fields should be revealed.
            if (this.gameEnded)
            {
                for (int nrow = 0; nrow < rows; nrow++)
                {
                    for (int ncol = 0; ncol < cols; ncol++)
                    {
                        revealNeighbours(nrow, ncol, true, placeFlagOnMine);
                    }
                }
            }
        }

        // Get color of number in field.
        /// <summary>
        /// Returns text color depending on number of mines.
        /// </summary>
        /// <param name="numberOfMines">Number of mines in neighbour fields.</param> 
        /// <returns></returns>
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