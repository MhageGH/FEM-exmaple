using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Point> nodes = new List<Point>();
        List<int> selectedNodes = new List<int>();
        List<int[]> triangles = new List<int[]>();

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked) nodes.Add(new Point(e.X, e.Y));
            else if (radioButton2.Checked)
            {
                for (int i = 0; i < nodes.Count; ++i)
                {
                    var x = e.X - nodes[i].X;
                    var y = e.Y - nodes[i].Y;
                    var r2 = x * x + y * y;
                    if (r2 < 100)
                    {
                        selectedNodes.Add(i);
                        break;
                    }
                }
                if (selectedNodes.Count == 3)
                {
                    var tri = new int[3];
                    for (int i = 0; i < 3; ++i) tri[i] = selectedNodes[i];
                    triangles.Add(tri);
                    selectedNodes.Clear();
                }
            }
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var t in triangles)
            {
                var ps = new Point[3];
                for (int i = 0; i < ps.Length; ++i) ps[i] = nodes[t[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            foreach (var n in nodes) e.Graphics.FillEllipse(Brushes.Blue, n.X - 5, n.Y - 5, 10, 10);
            foreach (var i in selectedNodes) e.Graphics.FillEllipse(Brushes.Red, nodes[i].X - 5, nodes[i].Y - 5, 10, 10);
        }
    }
}
