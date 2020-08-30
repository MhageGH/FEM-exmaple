using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mechanics
{
    public partial class Form2 : Form
    {
        public double[,] matrix;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (matrix != null)
            {
                for (int i = 0; i < matrix.GetLength(0); ++i) for (int j = 0; j < matrix.GetLength(1); ++j)
                        e.Graphics.DrawString(matrix[i, j].ToString("F0"), new Font("メイリオ", 8), Brushes.Black,
                            new RectangleF(50 + 60 * i, 180 + 30 * j, 50, 20), new StringFormat() { Alignment = StringAlignment.Center });
                var points = new Point[] { 
                    new Point(50 - 20 + 10, 180 - 10),
                    new Point(50 - 20, 180 - 10), 
                    new Point(50 - 20, 180 - 10 + 30*matrix.GetLength(0) + 5), 
                    new Point(50 - 20 + 10, 180 - 10 + 30 * matrix.GetLength(0) + 5)
                };
                e.Graphics.DrawLines(Pens.Black, points);
                points = points.Select(x => new Point(x.X + 60 * matrix.GetLength(1) + 30, x.Y)).ToArray();
                points[0].X -= 20;
                points[3].X -= 20;
                e.Graphics.DrawLines(Pens.Black, points);
            }
        }
    }
}
