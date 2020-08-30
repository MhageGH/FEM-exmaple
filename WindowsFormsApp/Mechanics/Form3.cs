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
    public partial class Form3 : Form
    {
        public double[,] matrix;

        public Form3()
        {
            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int fontSize = 8;
            int xPitch = 60;
            int yPitch = 30;
            if (matrix != null)
            {
                pictureBox1.Size = new Size(70 + 60 * matrix.GetLength(1), 70 + 30 * matrix.GetLength(0));
                for (int i = 0; i < matrix.GetLength(0); ++i) for (int j = 0; j < matrix.GetLength(1); ++j)
                        e.Graphics.DrawString(matrix[i, j].ToString("F0"), new Font("メイリオ", fontSize), Brushes.Black,                        
                            new RectangleF(50 + xPitch * i, 50 + yPitch * j, 50, 20), new StringFormat() { Alignment = StringAlignment.Center });
                var points = new Point[] {
                    new Point(50 - 20 + 10, 50 - 10),
                    new Point(50 - 20, 50 - 10),
                    new Point(50 - 20, 50 - 10 + yPitch*matrix.GetLength(0) + 5),
                    new Point(50 - 20 + 10, 50 - 10 + yPitch * matrix.GetLength(0) + 5)
                };
                e.Graphics.DrawLines(Pens.Black, points);
                points = points.Select(x => new Point(x.X + xPitch * matrix.GetLength(1) + 30, x.Y)).ToArray();
                points[0].X -= 20;
                points[3].X -= 20;
                e.Graphics.DrawLines(Pens.Black, points);
            }
        }
    }
}
