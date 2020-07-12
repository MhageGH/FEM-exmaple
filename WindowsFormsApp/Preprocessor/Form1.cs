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

        List<Point> points = new List<Point>();
        List<int> selectedNodes = new List<int>();
        List<int[]> triangles = new List<int[]>();
        List<int[]> quadangles = new List<int[]>();
        List<int> fixXs = new List<int>();
        List<int> fixYs = new List<int>();
        List<(int index, int value)> forceXs = new List<(int, int)>();
        List<(int index, int value)> forceYs = new List<(int, int)>();

        double TriangleArea(Point[] p)
        {
            if (p.Length != 3) throw new Exception("Invalid argument of SignedTriangleArea()");
            return (p[1].X * p[2].Y + p[2].X * p[0].Y + p[0].X * p[1].Y - p[2].X * p[1].Y - p[0].X * p[2].Y - p[1].X * p[0].Y) / 2;
        }

        void UpdateTriangles()
        {
            selectedNodes.Sort();
            var triangle = selectedNodes.ToArray();
            var p = new Point[3];
            for (int i = 0; i < p.Length; ++i) p[i] = points[triangle[i]];
            var t = new int[3];
            triangle.CopyTo(t, 0);
            if (TriangleArea(p) < 0) for (int i = 0; i < 2; ++i) triangle[i] = t[(i + 1) % 2];
            if (triangles.Any(x => x.SequenceEqual(triangle))) triangles.RemoveAll(x => x.SequenceEqual(triangle));
            else triangles.Add(triangle);
        }

        void UpdateQuadangles()
        {
            selectedNodes.Sort();
            var quadangle = selectedNodes.ToArray();
            var p = new Point[3];
            for (int i = 0; i < p.Length; ++i) p[i] = points[quadangle[i]];
            var q = new int[4];
            quadangle.CopyTo(q, 0);
            if (TriangleArea(p) < 0) for (int i = 0; i < 2; ++i) quadangle[i] = q[(i + 1) % 2];
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < p.Length; ++j) p[j] = points[quadangle[new int[] { 0, 2, 3 }[j]]];
                if (TriangleArea(p) > 0) break;
                quadangle.CopyTo(q, 0);
                for (int j = 0; j < 3; ++j) quadangle[j] = q[(j + 1) % 3];
            }
            if (quadangles.Any(x => x.SequenceEqual(quadangle))) quadangles.RemoveAll(x => x.SequenceEqual(quadangle));
            else quadangles.Add(quadangle);
        }

        void ReassignNodeNumber()
        {
            var _points = points.Select((p, i) => (p, i)).ToList();
            _points = _points.OrderBy(_point => _point.p.Y).ToList();
            _points = _points.OrderBy(_point => _point.p.X).ToList();
            points = _points.Select(_point => _point.p).ToList();
            var table = new List<int>();
            for (int i = 0; i < points.Count; ++i) table.Add(_points.FindIndex(_point => _point.i == i));
            foreach (var triangle in triangles) for (int i = 0; i < triangle.Length; ++i) triangle[i] = table[triangle[i]];
            foreach (var quadangle in quadangles) for (int i = 0; i < quadangle.Length; ++i) quadangle[i] = table[quadangle[i]];
            for (int i = 0; i < fixXs.Count; ++i) fixXs[i] = table[fixXs[i]];
            for (int i = 0; i < fixYs.Count; ++i) fixYs[i] = table[fixYs[i]];
            for (int i = 0; i < forceXs.Count; ++i) forceXs[i] = (table[forceXs[i].index], forceXs[i].value);
            for (int i = 0; i < forceYs.Count; ++i) forceYs[i] = (table[forceYs[i].index], forceYs[i].value);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int index = -1;
            for (int i = 0; i < points.Count; ++i)
            {
                var x = e.X - points[i].X;
                var y = e.Y - points[i].Y;
                var r2 = x * x + y * y;
                if (r2 < 100)
                {
                    index = i;
                    break;
                }
            }
            if (radioButton_node.Checked)
            {
                if (index == -1) points.Add(new Point(e.X, e.Y));
                else if (!triangles.Any(x => x.Contains(index)) && !quadangles.Any(x => x.Contains(index))
                    && !fixXs.Contains(index) && !fixYs.Contains(index)
                    && !forceXs.Any(x => x.index == index) && !forceYs.Any(x => x.index == index))
                {
                    points.RemoveAt(index);
                    foreach (var triangle in triangles) for (int i = 0; i < triangle.Length; ++i) if (triangle[i] > index) triangle[i]--;
                    foreach (var quadangle in quadangles) for (int i = 0; i < quadangle.Length; ++i) if (quadangle[i] > index) quadangle[i]--;
                    for (int i = 0; i < fixXs.Count; ++i) if (fixXs[i] > index) fixXs[i]--;
                    for (int i = 0; i < fixYs.Count; ++i) if (fixYs[i] > index) fixYs[i]--;
                    for (int i = 0; i < forceXs.Count; ++i) if (forceXs[i].index > index) forceXs[i] = (forceXs[i].index - 1, forceXs[i].value);
                    for (int i = 0; i < forceYs.Count; ++i) if (forceYs[i].index > index) forceYs[i] = (forceYs[i].index - 1, forceYs[i].value);
                }
            }
            else if (radioButton_triangle.Checked)
            {
                if (index == -1) return;
                else selectedNodes.Add(index);
                if (selectedNodes.Count == 3)
                {
                    UpdateTriangles();
                    selectedNodes.Clear();
                }
            }
            else if (radioButton_quadangle.Checked)
            {
                if (index == -1) return;
                else selectedNodes.Add(index);
                if (selectedNodes.Count == 4)
                {
                    UpdateQuadangles();
                    selectedNodes.Clear();
                }
            }
            else if (radioButton_fixX.Checked)
            {
                if (index == -1) return;
                else if (fixXs.Contains(index)) fixXs.Remove(index);
                else fixXs.Add(index);
            }
            else if (radioButton_fixY.Checked)
            {
                if (index == -1) return;
                else if (fixYs.Contains(index)) fixYs.Remove(index);
                else fixYs.Add(index);
            }
            else if (radioButton_forceX.Checked)
            {
                if (index == -1) return;
                bool add = true;
                for (int i = 0; i < forceXs.Count; ++i)
                {
                    if (forceXs[i].index == index)
                    {
                        if (forceXs[i].value == 1) forceXs[i] = (index, 2);
                        else if (forceXs[i].value == 2) forceXs.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) forceXs.Add((index, 1));
            }
            else if (radioButton_forceY.Checked)
            {
                if (index == -1) return;
                bool add = true;
                for (int i = 0; i < forceYs.Count; ++i)
                {
                    if (forceYs[i].index == index)
                    {
                        if (forceYs[i].value == 1) forceYs[i] = (index, 2);
                        else if (forceYs[i].value == 2) forceYs.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) forceYs.Add((index, 1));
            }
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var triangle in triangles)
            {
                var ps = new Point[3];
                for (int i = 0; i < ps.Length; ++i) ps[i] = points[triangle[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            foreach (var quadangle in quadangles)
            {
                var ps = new Point[4];
                for (int i = 0; i < ps.Length; ++i) ps[i] = points[quadangle[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }

            foreach (var p in points) e.Graphics.FillEllipse(Brushes.Blue, p.X - 5, p.Y - 5, 10, 10);
            foreach (var i in selectedNodes) e.Graphics.FillEllipse(Brushes.Red, points[i].X - 5, points[i].Y - 5, 10, 10);

            for (int i = 0; i < fixXs.Count; ++i)
            {
                var tri = new PointF[] { new Point(0, 0), new Point(-20, -10), new Point(-20, 10) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += points[fixXs[i]].X;
                    tri[j].Y += points[fixXs[i]].Y;
                }
                e.Graphics.DrawPolygon(new Pen(Color.Blue), tri);
            }
            for (int i = 0; i < fixYs.Count; ++i)
            {
                var tri = new PointF[] { new Point(0, 0), new Point(-10, 20), new Point(10, 20) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += points[fixYs[i]].X;
                    tri[j].Y += points[fixYs[i]].Y;
                }
                e.Graphics.DrawPolygon(new Pen(Color.Blue), tri);
            }
            for (int i = 0; i < forceXs.Count; ++i)
            {
                float f = 0.5f * forceXs[i].value;
                PointF[] tri, line;
                tri = new PointF[] { new PointF(0 + 50 * f, 0), new PointF(-10 + 50 * f, -5), new PointF(-10 + 50 * f, 5) };
                line = new PointF[] { new PointF(0, 0), new PointF(50 * f, 0) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += points[forceXs[i].index].X;
                    tri[j].Y += points[forceXs[i].index].Y;
                }
                for (int j = 0; j < line.Length; ++j)
                {
                    line[j].X += points[forceXs[i].index].X;
                    line[j].Y += points[forceXs[i].index].Y;
                }
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
            for (int i = 0; i < forceYs.Count; ++i)
            {
                float f = 0.5f * forceYs[i].value;
                PointF[] tri, line;
                tri = new PointF[] { new PointF(0, 0 - 50 * f), new PointF(-5, 10 - 50 * f), new PointF(5, 10 - 50 * f) };
                line = new PointF[] { new PointF(0, 0), new PointF(0, -50 * f) };
                for (int j = 0; j < tri.Length; ++j)
                {
                    tri[j].X += points[forceYs[i].index].X;
                    tri[j].Y += points[forceYs[i].index].Y;
                }
                for (int j = 0; j < line.Length; ++j)
                {
                    line[j].X += points[forceYs[i].index].X;
                    line[j].Y += points[forceYs[i].index].Y;
                }
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
        }

        string EncodeToCSV()
        {
            ReassignNodeNumber();
            var sb = new StringBuilder();
            sb.AppendLine("points");
            foreach (var point in points) sb.AppendLine(point.X.ToString() + "," + point.Y.ToString());
            sb.AppendLine();
            sb.AppendLine("triangles");
            foreach (var triangle in triangles) sb.AppendLine(triangle[0].ToString() + "," + triangle[1].ToString() + "," + triangle[2].ToString());
            sb.AppendLine();
            sb.AppendLine("quadangles");
            foreach (var quadangle in quadangles) sb.AppendLine(quadangle[0].ToString() + "," + quadangle[1].ToString() + "," + quadangle[2].ToString() + "," + quadangle[3].ToString());
            sb.AppendLine();
            sb.AppendLine("fix X");
            foreach (var idx in fixXs) sb.AppendLine(idx.ToString());
            sb.AppendLine();
            sb.AppendLine("fix Y");
            foreach (var idx in fixYs) sb.AppendLine(idx.ToString());
            sb.AppendLine();
            sb.AppendLine("force X");
            foreach (var f in forceXs) sb.AppendLine(f.index.ToString() + "," + f.value.ToString());
            sb.AppendLine();
            sb.AppendLine("force Y");
            foreach (var f in forceYs) sb.AppendLine(f.index.ToString() + "," + f.value.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        void DecodeFromCSV(string text)
        {
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var words = new string[lines.Length][];
            for (int i = 0; i < lines.Length; ++i) words[i] = lines[i].Split(new char[] { ',' });
            var labels = new string[] { "points", "triangles", "quadangles", "fix X", "fix Y", "force X", "force Y" };
            for (int i = 0; i < labels.Length; ++i)
            {
                for (int j = 0; j < words.Length; ++j)
                {
                    if (words[j][0] == labels[i])
                    {
                        switch(i)
                        {
                            case 0:
                                points = new List<Point>();
                                for (++j; words[j][0] != ""; ++j) points.Add(new Point(Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
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
                                fixXs = new List<int>();
                                for (++j; words[j][0] != ""; ++j) fixXs.Add(Convert.ToInt16(words[j][0]));
                                break;
                            case 4:
                                fixYs = new List<int>();
                                for (++j; words[j][0] != ""; ++j) fixYs.Add(Convert.ToInt16(words[j][0]));
                                break;
                            case 5:
                                forceXs = new List<(int index, int value)>();
                                for (++j; words[j][0] != ""; ++j) forceXs.Add((Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
                                break;
                            case 6:
                                forceYs = new List<(int index, int value)>();
                                for (++j; words[j][0] != ""; ++j) forceYs.Add((Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
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
