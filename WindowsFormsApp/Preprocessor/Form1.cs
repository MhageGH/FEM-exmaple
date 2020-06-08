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
        List<int> selectedNodeIndex = new List<int>();
        List<int[]> elements = new List<int[]>();

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
                        selectedNodeIndex.Add(i);
                        break;
                    }
                }
                if (selectedNodeIndex.Count == 3)
                {
                    var ele = new int[3];
                    for (int i = 0; i < 3; ++i) ele[i] = selectedNodeIndex[i];
                    elements.Add(ele);
                    selectedNodeIndex.Clear();
                }
            }
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var ele in elements)
            {
                var ps = new Point[3];
                for (int i = 0; i < ps.Length; ++i) ps[i] = nodes[ele[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            foreach (var node in nodes) e.Graphics.FillEllipse(Brushes.Blue, node.X - 5, node.Y - 5, 10, 10);
            foreach (var i in selectedNodeIndex) e.Graphics.FillEllipse(Brushes.Red, nodes[i].X - 5, nodes[i].Y - 5, 10, 10);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.FileName = "FEM.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var sw = new System.IO.StreamWriter(sfd.FileName))
                {
                    sw.WriteLine("nodes");
                    foreach (var node in nodes) sw.WriteLine(node.X.ToString() + ", " + node.Y.ToString());
                    sw.WriteLine();
                    sw.WriteLine("elements");
                    foreach (var ele in elements) sw.WriteLine(ele[0].ToString() + ", " + ele[1].ToString() + ", " + ele[2].ToString());
                    sw.WriteLine();
                }
            }
        }
    }
}
