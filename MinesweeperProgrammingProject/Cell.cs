using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesweeperProgrammingProject
{
    class Cell
    {
        public bool HasBomb { get; set; }
        public bool HasFlag { get; set; }
        public bool Cleared { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int Adj { get; set; }
        public Button Button { get; set; }

        public Cell(bool hasBomb, bool hasFlag, bool cleared, int row, int col, int adj, Button button)
        {
            this.HasBomb = hasBomb;
            this.HasFlag = hasFlag;
            this.Cleared = cleared;
            this.Row = row;
            this.Col = col;
            this.Adj = adj;
            this.Button = button;
        }

        internal void Clear()
        {
            if (!this.HasFlag)
            {
                this.Cleared = true;
                this.Button.BackColor = Color.Azure;
                if (this.Adj != 0)
                {
                    this.Button.Text = $"{this.Adj}";
                }
            }
        }

        internal void Flag()
        {
            if (this.HasFlag)
            {
                this.HasFlag = false;
                this.Button.BackColor = Color.White;
                this.Button.Text = "";
            }
            else
            {
                this.HasFlag = true;
                this.Button.BackColor = Color.Yellow;
                this.Button.Text = "\uD83D\uDEA9";
            }
        }
    }
}
