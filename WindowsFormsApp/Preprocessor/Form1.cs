using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
        List<int> selectedNodeIndexes = new List<int>();
        List<int[]> triangles = new List<int[]>();
        List<int[]> quadangles = new List<int[]>();
        List<int> fixXNodeIndex = new List<int>();
        List<int> fixYNodeIndex = new List<int>();
        List<(int index, int value)> forceXNodeIndexWithValue = new List<(int, int)>();
        List<(int index, int value)> forceYNodeIndexWithValue = new List<(int, int)>();

        double TriangleArea(Point[] p)
        {
            if (p.Length != 3) throw new Exception("Invalid argument of SignedTriangleArea()");
            return (p[1].X * p[2].Y + p[2].X * p[0].Y + p[0].X * p[1].Y - p[2].X * p[1].Y - p[0].X * p[2].Y - p[1].X * p[0].Y) / 2;
        }

        void UpdateTriangles()
        {
            selectedNodeIndexes.Sort();
            var triangle = selectedNodeIndexes.ToArray();
            var node = new Point[3];
            for (int i = 0; i < node.Length; ++i) node[i] = nodes[triangle[i]];
            var t = new int[3];
            triangle.CopyTo(t, 0);
            if (TriangleArea(node) < 0) for (int i = 0; i < 2; ++i) triangle[i] = t[(i + 1) % 2];
            if (triangles.Any(x => x.SequenceEqual(triangle))) triangles.RemoveAll(x => x.SequenceEqual(triangle));
            else triangles.Add(triangle);
        }

        void UpdateQuadangles()
        {
            selectedNodeIndexes.Sort();
            var quadangle = selectedNodeIndexes.ToArray();
            var node = new Point[3];
            for (int i = 0; i < node.Length; ++i) node[i] = nodes[quadangle[i]];
            var q = new int[4];
            quadangle.CopyTo(q, 0);
            if (TriangleArea(node) < 0) for (int i = 0; i < 2; ++i) quadangle[i] = q[(i + 1) % 2];
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < node.Length; ++j) node[j] = nodes[quadangle[new int[] { 0, 2, 3 }[j]]];
                if (TriangleArea(node) > 0) break;
                quadangle.CopyTo(q, 0);
                for (int j = 0; j < 3; ++j) quadangle[j] = q[(j + 1) % 3];
            }
            if (quadangles.Any(x => x.SequenceEqual(quadangle))) quadangles.RemoveAll(x => x.SequenceEqual(quadangle));
            else quadangles.Add(quadangle);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int index = -1;
            for (int i = 0; i < nodes.Count; ++i)
            {
                var x = e.X - nodes[i].X;
                var y = e.Y - nodes[i].Y;
                var r2 = x * x + y * y;
                if (r2 < 100)
                {
                    index = i;
                    break;
                }
            }
            if (radioButton_node.Checked)
            {
                if (index == -1) nodes.Add(new Point(e.X, e.Y));
                else if (!triangles.Any(x => x.Contains(index)) && !quadangles.Any(x => x.Contains(index))
                    && !fixXNodeIndex.Contains(index) && !fixYNodeIndex.Contains(index)
                    && !forceXNodeIndexWithValue.Any(x => x.index == index) && !forceYNodeIndexWithValue.Any(x => x.index == index))
                    nodes.RemoveAt(index);
            }
            else if (radioButton_triangle.Checked)
            {
                if (index == -1) return;
                else selectedNodeIndexes.Add(index);
                if (selectedNodeIndexes.Count == 3)
                {
                    UpdateTriangles();
                    selectedNodeIndexes.Clear();
                }
            }
            else if (radioButton_quadangle.Checked)
            {
                if (index == -1) return;
                else selectedNodeIndexes.Add(index);
                if (selectedNodeIndexes.Count == 4)
                {
                    UpdateQuadangles();
                    selectedNodeIndexes.Clear();
                }
            }
            else if (radioButton_fixX.Checked)
            {
                if (index == -1) return;
                else if (fixXNodeIndex.Contains(index)) fixXNodeIndex.Remove(index);
                else fixXNodeIndex.Add(index);
            }
            else if (radioButton_fixY.Checked)
            {
                if (index == -1) return;
                else if (fixYNodeIndex.Contains(index)) fixYNodeIndex.Remove(index);
                else fixYNodeIndex.Add(index);
            }
            else if (radioButton_forceX.Checked)
            {
                if (index == -1) return;
                bool add = true;
                for (int i = 0; i < forceXNodeIndexWithValue.Count; ++i)
                {
                    if (forceXNodeIndexWithValue[i].index == index)
                    {
                        if (forceXNodeIndexWithValue[i].value == 1) forceXNodeIndexWithValue[i] = (index, 2);
                        else if (forceXNodeIndexWithValue[i].value == 2) forceXNodeIndexWithValue.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) forceXNodeIndexWithValue.Add((index, 1));
            }
            else if (radioButton_forceY.Checked)
            {
                if (index == -1) return;
                bool add = true;
                for (int i = 0; i < forceYNodeIndexWithValue.Count; ++i)
                {
                    if (forceYNodeIndexWithValue[i].index == index)
                    {
                        if (forceYNodeIndexWithValue[i].value == 1) forceYNodeIndexWithValue[i] = (index, 2);
                        else if (forceYNodeIndexWithValue[i].value == 2) forceYNodeIndexWithValue.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) forceYNodeIndexWithValue.Add((index, 1));
            }
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var triangle in triangles)
            {
                var ps = new Point[3];
                for (int i = 0; i < ps.Length; ++i) ps[i] = nodes[triangle[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            foreach (var quadangle in quadangles)
            {
                var ps = new Point[4];
                for (int i = 0; i < ps.Length; ++i) ps[i] = nodes[quadangle[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }

            foreach (var node in nodes) e.Graphics.FillEllipse(Brushes.Blue, node.X - 5, node.Y - 5, 10, 10);
            foreach (var i in selectedNodeIndexes) e.Graphics.FillEllipse(Brushes.Red, nodes[i].X - 5, nodes[i].Y - 5, 10, 10);

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

        string EncodeToCSV()
        {
            var sb = new StringBuilder();
            sb.AppendLine("nodes");
            foreach (var node in nodes) sb.AppendLine(node.X.ToString() + "," + node.Y.ToString());
            sb.AppendLine();
            sb.AppendLine("triangles");
            foreach (var triangle in triangles) sb.AppendLine(triangle[0].ToString() + "," + triangle[1].ToString() + "," + triangle[2].ToString());
            sb.AppendLine();
            sb.AppendLine("quadangles");
            foreach (var quadangle in quadangles) sb.AppendLine(quadangle[0].ToString() + "," + quadangle[1].ToString() + "," + quadangle[2].ToString() + "," + quadangle[3].ToString());
            sb.AppendLine();
            sb.AppendLine("fix X");
            foreach (var idx in fixXNodeIndex) sb.AppendLine(idx.ToString());
            sb.AppendLine();
            sb.AppendLine("fix Y");
            foreach (var idx in fixYNodeIndex) sb.AppendLine(idx.ToString());
            sb.AppendLine();
            sb.AppendLine("force X");
            foreach (var f in forceXNodeIndexWithValue) sb.AppendLine(f.index.ToString() + "," + f.value.ToString());
            sb.AppendLine();
            sb.AppendLine("force Y");
            foreach (var f in forceYNodeIndexWithValue) sb.AppendLine(f.index.ToString() + "," + f.value.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        void DecodeFromCSV(string text)
        {
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var words = new string[lines.Length][];
            for (int i = 0; i < lines.Length; ++i) words[i] = lines[i].Split(new char[] { ',' });
            var labels = new string[] { "nodes", "triangles", "quadangles", "fix X", "fix Y", "force X", "force Y" };
            for (int i = 0; i < labels.Length; ++i)
            {
                for (int j = 0; j < words.Length; ++j)
                {
                    if (words[j][0] == labels[i])
                    {
                        switch(i)
                        {
                            case 0:
                                nodes = new List<Point>();
                                for (++j; words[j][0] != ""; ++j) nodes.Add(new Point(Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
                                break;
                            case 1:
                                triangles = new List<int[]>();
                                for (++j; words[j][0] != ""; ++j) triangles.Add(new int[3] { Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1]), Convert.ToInt16(words[j][2]) });
                                break;
                            case 2:
                                quadangles = new List<int[]>();
                                for (++j; words[j][0] != ""; ++j) quadangles.Add(new int[4] { Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1]), Convert.ToInt16(words[j][2]), Convert.ToInt16(words[j][3]) });
                                break;
                            case 3:
                                fixXNodeIndex = new List<int>();
                                for (++j; words[j][0] != ""; ++j) fixXNodeIndex.Add(Convert.ToInt16(words[j][0]));
                                break;
                            case 4:
                                fixYNodeIndex = new List<int>();
                                for (++j; words[j][0] != ""; ++j) fixYNodeIndex.Add(Convert.ToInt16(words[j][0]));
                                break;
                            case 5:
                                forceXNodeIndexWithValue = new List<(int index, int value)>();
                                for (++j; words[j][0] != ""; ++j) forceXNodeIndexWithValue.Add((Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
                                break;
                            case 6:
                                forceYNodeIndexWithValue = new List<(int index, int value)>();
                                for (++j; words[j][0] != ""; ++j) forceYNodeIndexWithValue.Add((Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
                                break;
                        }
                        break;
                    }
                    if (j == words.Length - 1)
                    {
                        MessageBox.Show("Invalid text");
                        break;
                    }
                }
            }                
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.FileName = "Mesh.csv";
            if (sfd.ShowDialog() == DialogResult.OK) using (var sw = new System.IO.StreamWriter(sfd.FileName)) sw.Write(EncodeToCSV());
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSVファイル|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK) using (var sr = new System.IO.StreamReader(ofd.FileName)) DecodeFromCSV(sr.ReadToEnd());
            Invalidate();
        }
    }
}
