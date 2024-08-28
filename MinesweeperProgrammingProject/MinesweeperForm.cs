using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesweeperProgrammingProject
{
    public partial class MinesweeperForm : Form
    {
        //////////////////////////////////////////
        // class constants
        private int ROWS;
        private int COLS;
        private int BOMBS;
        private const int BUTTON_SIZE = 25;
        private const string BOMB = "\uD83D\uDCA3";
        private int leftToClear;
        private int bombsPlaced;
        Random rng = new Random();
        Cell[,] cells;

        //////////////////////////////////////////
        // fields and properties
        private int Rows { get; set; }
        private int Cols { get; set; }

        //////////////////////////////////////////
        // constructor
        public MinesweeperForm()
        {
            InitializeComponent();
            bool validInput = false;
            do
            {
                Form prompt = new Form()
                {
                    Width = 310,
                    Height = 140,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = "Size Settings",
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label textLabel = new Label() { Left = 10, Top = 10, Text = "Eneter custom board size in the format \"[width], [height]\"", Width = 290 };
                TextBox textBox = new TextBox() { Left = 10, Top = 30, Width = 200 };
                Button okButton = new Button()
                {
                    Text = "OK",
                    Left = 10,
                    Width = 50,
                    Top = 60,
                    DialogResult = DialogResult.OK
                };
                okButton.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(okButton);
                prompt.Controls.Add(textLabel);
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;
                prompt.AcceptButton = okButton;
                prompt.ShowDialog();

                try
                {
                    string[] promptVals = textBox.Text.Trim().Split(',');
                    ROWS = int.Parse(promptVals[1]);
                    COLS = int.Parse(promptVals[0]);
                    validInput = true;
                }
                catch (Exception e)
                {
                    if (textBox.Text.Trim() == "")
                    {
                        ROWS = 10;
                        COLS = 10;
                        validInput = true;
                        break;
                    }
                    Console.WriteLine(e.Message);
                    MessageBox.Show("Invalid input");
                }
            } while (!validInput);
            this.Rows = ROWS;
            this.Cols = COLS;
            BOMBS = (ROWS * COLS) / 5;
            cells = new Cell[ROWS, COLS];
        }

        //////////////////////////////////////////
        // event handlers
        private void MinesweeperForm_Load(object sender, EventArgs e)
        {
            // resize the form
            this.Width = BUTTON_SIZE * this.Cols + 16;
            int titleHeight = this.Height - this.ClientRectangle.Height;
            this.Height = BUTTON_SIZE * this.Rows + 14 + BUTTON_SIZE;

            // create the buttons on the form
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    // create a new button control
                    Button b = new Button();
                    // set the button width and height
                    b.Width = BUTTON_SIZE;
                    b.Height = BUTTON_SIZE;
                    // position the button on the form
                    b.Top = i * BUTTON_SIZE;
                    b.Left = j * BUTTON_SIZE;
                    // no text
                    b.Text = String.Empty;
                    // set the button style
                    b.FlatStyle = FlatStyle.Popup;
                    // add a MouseDown event handler
                    b.MouseDown += new MouseEventHandler(MinesweeperForm_MouseDown);
                    // give the button a name in "row_col" format 
                    b.Name = i + "_" + j;
                    b.BackColor = Color.DarkGray;
                    // add the button control to the form
                    this.Controls.Add(b);
                    bool placeBomb = false;
                    if (BOMBS > 0)
                    {
                        placeBomb = rng.Next(10) < 2;
                    }

                    // do other stuff here?
                    cells[i, j] = new Cell(placeBomb, false, false, i, j, 0, b);
                    if (placeBomb)
                    {
                        bombsPlaced++;
                        BOMBS--;
                    }
                }
            }
            leftToClear = (ROWS * COLS) - bombsPlaced;
            // set up the board
            for (int r = 0; r < cells.GetLength(0); r++)
            {
                for (int c = 0; c < cells.GetLength(1); c++)
                {
                    if (cells[r, c].HasBomb)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                try
                                {
                                    if (!(i == 0 && j == 0))
                                    {
                                        cells[r + i, c + j].Adj++;
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                {
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MinesweeperForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                Button b = (Button)sender;
                // extract the row and column from the button name
                int index = b.Name.IndexOf("_");
                int i = int.Parse(b.Name.Substring(0, index));
                int j = int.Parse(b.Name.Substring(index + 1));

                // handle mousebuttons left and right differently
                if (e.Button == MouseButtons.Left)
                {
                    // dig the position to reveal the contents
                    if (!cells[i, j].HasFlag)
                    {
                        this.DigCell(i, j);
                    }
                }
                else
                {
                    // flag the position as a possible mine
                    this.FlagCell(i, j);
                }
            }
        }

        private void FlagCell(int i, int j)
        {
            cells[i, j].Flag();
        }

        private void DigCell(int r, int c)
        {
            Cell cell = cells[r, c];
            if (cell.Cleared)
            {
                return;
            }
            else if (cell.HasBomb)
            {
                this.GameOver(r, c);
            }
            else if (cell.Adj > 0)
            {
                cell.Clear();
                leftToClear--;
                if (leftToClear == 0)
                {
                    this.WinGame();
                }
                return;
            }
            else
            {
                cell.Clear();
                leftToClear--;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        try
                        {
                            if (!cells[r + i, c + j].HasBomb)
                            {
                                this.DigCell(r + i, c + j);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                        }
                    }
                }
                return;
            }
        }

        private void WinGame()
        {
            MessageBox.Show("You won!");
            this.Close();
        }

        private void GameOver(int r, int c)
        {
            cells[r, c].Button.Text = BOMB;
            MessageBox.Show("You lost");
            this.Close();
        }

        //////////////////////////////////////////
        // instance methods

    }
}
