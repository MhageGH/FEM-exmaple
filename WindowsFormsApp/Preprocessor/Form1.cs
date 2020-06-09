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
        List<int> fixXNodeIndex = new List<int>();
        List<int> fixYNodeIndex = new List<int>();
        List<(int index, int value)> forceXNodeIndexWithValue = new List<(int, int)>();
        List<(int index, int value)> forceYNodeIndexWithValue = new List<(int, int)>();

        double SignedArea()
        {
            var p = new Point[3];
            for (int i = 0; i < p.Length; ++i) p[i] = nodes[selectedNodeIndex[i]];
            return (p[1].X * p[2].Y + p[2].X * p[0].Y + p[0].X * p[1].Y - p[2].X * p[1].Y - p[0].X * p[2].Y - p[1].X * p[0].Y) / 2;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int s = -1;
            for (int i = 0; i < nodes.Count; ++i)
            {
                var x = e.X - nodes[i].X;
                var y = e.Y - nodes[i].Y;
                var r2 = x * x + y * y;
                if (r2 < 100)
                {
                    s = i;
                    break;
                }
            }
            if (radioButton1.Checked)
            {
                if (s == -1) nodes.Add(new Point(e.X, e.Y));
                else nodes.RemoveAt(s);
            }
            else if (radioButton2.Checked)
            {
                if (s == -1) return;
                else selectedNodeIndex.Add(s);
                if (selectedNodeIndex.Count == 3)
                {
                    selectedNodeIndex.Sort();
                    if (SignedArea() < 0) selectedNodeIndex.Sort((a, b) => b - a);
                    var ele = new int[3];
                    for (int i = 0; i < 3; ++i) ele[i] = selectedNodeIndex[i];
                    bool add = true;
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        if (ele[0] == elements[i][0] && ele[1] == elements[i][1] && ele[2] == elements[i][2])
                        {
                            elements.RemoveAt(i);
                            add = false;
                            break;
                        }
                    }
                    if (add) elements.Add(ele);
                    selectedNodeIndex.Clear();
                }
            }
            else if (radioButton3.Checked)
            {
                if (s == -1) return;
                else if (fixXNodeIndex.Contains(s)) fixXNodeIndex.Remove(s);
                else fixXNodeIndex.Add(s);
            }
            else if (radioButton4.Checked)
            {
                if (s == -1) return;
                else if (fixYNodeIndex.Contains(s)) fixYNodeIndex.Remove(s);
                else fixYNodeIndex.Add(s);
            }
            else if (radioButton5.Checked)
            {
                if (s == -1) return;
                bool add = true;
                for (int i = 0; i < forceXNodeIndexWithValue.Count; ++i)
                {
                    if (forceXNodeIndexWithValue[i].index == s)
                    {
                        if (forceXNodeIndexWithValue[i].value == 1) forceXNodeIndexWithValue[i] = (s, 2);
                        else if (forceXNodeIndexWithValue[i].value == 2) forceXNodeIndexWithValue.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) forceXNodeIndexWithValue.Add((s, 1));
            }
            else if (radioButton6.Checked)
            {
                if (s == -1) return;
                bool add = true;
                for (int i = 0; i < forceYNodeIndexWithValue.Count; ++i)
                {
                    if (forceYNodeIndexWithValue[i].index == s)
                    {
                        if (forceYNodeIndexWithValue[i].value == 1) forceYNodeIndexWithValue[i] = (s, 2);
                        else if (forceYNodeIndexWithValue[i].value == 2) forceYNodeIndexWithValue.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) forceYNodeIndexWithValue.Add((s, 1));
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

            for (int i = 0; i < fixXNodeIndex.Count; ++i)
            {
                var tri = new PointF[] { new Point(0, 0), new Point(-20, -10), new Point(-20, 10) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += nodes[fixXNodeIndex[i]].X;
                    tri[j].Y += nodes[fixXNodeIndex[i]].Y;
                }
                e.Graphics.DrawPolygon(new Pen(Color.Blue), tri);
            }
            for (int i = 0; i < fixYNodeIndex.Count; ++i)
            {
                var tri = new PointF[] { new Point(0, 0), new Point(-10, 20), new Point(10, 20) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += nodes[fixYNodeIndex[i]].X;
                    tri[j].Y += nodes[fixYNodeIndex[i]].Y;
                }
                e.Graphics.DrawPolygon(new Pen(Color.Blue), tri);
            }
            for (int i = 0; i < forceXNodeIndexWithValue.Count; ++i)
            {
                float f = 0.5f * forceXNodeIndexWithValue[i].value;
                PointF[] tri, line;
                tri = new PointF[] { new PointF(0 + 50 * f, 0), new PointF(-10 + 50 * f, -5), new PointF(-10 + 50 * f, 5) };
                line = new PointF[] { new PointF(0, 0), new PointF(50 * f, 0) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += nodes[forceXNodeIndexWithValue[i].index].X;
                    tri[j].Y += nodes[forceXNodeIndexWithValue[i].index].Y;
                }
                for (int j = 0; j < line.Length; ++j)
                {
                    line[j].X += nodes[forceXNodeIndexWithValue[i].index].X;
                    line[j].Y += nodes[forceXNodeIndexWithValue[i].index].Y;
                }
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
            for (int i = 0; i < forceYNodeIndexWithValue.Count; ++i)
            {
                float f = 0.5f * forceYNodeIndexWithValue[i].value;
                PointF[] tri, line;
                tri = new PointF[] { new PointF(0, 0 - 50 * f), new PointF(-5, 10 - 50 * f), new PointF(5, 10 - 50 * f) };
                line = new PointF[] { new PointF(0, 0), new PointF(0, -50 * f) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += nodes[forceYNodeIndexWithValue[i].index].X;
                    tri[j].Y += nodes[forceYNodeIndexWithValue[i].index].Y;
                }
                for (int j = 0; j < line.Length; ++j)
                {
                    line[j].X += nodes[forceYNodeIndexWithValue[i].index].X;
                    line[j].Y += nodes[forceYNodeIndexWithValue[i].index].Y;
                }
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
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
                    foreach (var node in nodes) sw.WriteLine(node.X.ToString() + "," + node.Y.ToString());
                    sw.WriteLine();
                    sw.WriteLine("elements");
                    foreach (var ele in elements) sw.WriteLine(ele[0].ToString() + "," + ele[1].ToString() + "," + ele[2].ToString());
                    sw.WriteLine();
                    sw.WriteLine("fix X");
                    foreach (var idx in fixXNodeIndex) sw.WriteLine(idx.ToString());
                    sw.WriteLine();
                    sw.WriteLine("fix Y");
                    foreach (var idx in fixYNodeIndex) sw.WriteLine(idx.ToString());
                    sw.WriteLine();
                    sw.WriteLine("force X");
                    foreach (var f in forceXNodeIndexWithValue) sw.WriteLine(f.index.ToString() + "," + f.value.ToString());
                    sw.WriteLine();
                    sw.WriteLine("force Y");
                    foreach (var f in forceYNodeIndexWithValue) sw.WriteLine(f.index.ToString() + "," + f.value.ToString());
                    sw.WriteLine();
                }
            }
        }
    }
}
