using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Mechanics
{
    public partial class Form1 : Form
    {
        FEM fem;

        public Form1()
        {
            InitializeComponent();
            listBox1.SelectedIndex = 0;
            fem = new FEM();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var values = new double[fem.mesh.points.Count];
            if (fem.solved)
            {
                switch (listBox1.SelectedIndex)
                {
                    case 0:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.delta[2 * i]);
                        break;
                    case 1:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.delta[2 * i + 1]);
                        break;
                    case 2:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.epsilon_node[i][0]);
                        break;
                    case 3:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.epsilon_node[i][1]);
                        break;
                    case 4:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.epsilon_node[i][2]);
                        break;
                    case 5:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.sigma_node[i][0]);
                        break;
                    case 6:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.sigma_node[i][1]);
                        break;
                    case 7:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.sigma_node[i][2]);
                        break;
                }
                double max = -1.0e10, min = 1.0e10;
                for (int i = 0; i < values.Length; ++i) if (max < values[i]) max = values[i];
                for (int i = 0; i < values.Length; ++i) if (min > values[i]) min = values[i];
                for (int i = 0; i < values.Length; ++i) values[i] = max > min ? (values[i] - min) / (max - min) : max;
            }
            var triangles = new List<Triangle>();
            foreach (var triangle in fem.mesh.triangles)
            {
                var points = new List<Point>();
                var colors = new List<Color>();
                foreach(var node in triangle)
                {
                    points.Add(fem.mesh.points[node]);
                    var c0 = values[node] < 0.5 ? Color.Blue : Color.LawnGreen;
                    var c1 = values[node] < 0.5 ? Color.LawnGreen : Color.Red;
                    var s = values[node] < 0.5 ? 2 * values[node] : 2 * (values[node] - 0.5);
                    colors.Add(Color.FromArgb((int)(c0.R * (1 - s) + c1.R * s), (int)(c0.G * (1 - s) + c1.G * s), (int)(c0.B * (1 - s) + c1.B * s)));
                }
                triangles.Add(new Triangle(points.ToArray(), colors.ToArray()));
            }
            foreach (var triangle in triangles)
            {
                if (fem.solved) e.Graphics.FillRectangle(triangle.GetGradientBrush(), this.ClientRectangle);
                else e.Graphics.FillPolygon(Brushes.LawnGreen, triangle.points);
                e.Graphics.DrawPolygon(new Pen(Color.Black), triangle.points);
            }
            foreach(var i in fem.mesh.fixXs)
            {
                var tri = new Point[] { new Point(0, 0), new Point(-20, -10), new Point(-20, 10) };
                tri = tri.Select(x => new Point(x.X + fem.mesh.points[i].X, x.Y + fem.mesh.points[i].Y)).ToArray();
                e.Graphics.DrawPolygon(Pens.Blue, tri);
            }
            foreach (var i in fem.mesh.fixYs)
            {
                var tri = new Point[] { new Point(0, 0), new Point(-10, 20), new Point(10, 20) };
                tri = tri.Select(x => new Point(x.X + fem.mesh.points[i].X, x.Y + fem.mesh.points[i].Y)).ToArray();
                e.Graphics.DrawPolygon(Pens.Blue, tri);
            }
            foreach(var f in fem.mesh.forceXs)
            {
                var tri = new Point[] { new Point(0 + 50 * f.value / 2, 0), new Point(-10 + 50 * f.value / 2, -5), new Point(-10 + 50 * f.value / 2, 5) };
                tri = tri.Select(x => new Point(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                var line = new Point[] { new Point(0, 0), new Point(50 * f.value / 2, 0) };
                line = line.Select(x => new Point(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
            foreach (var f in fem.mesh.forceYs)
            {
                var tri = new Point[] { new Point(0, 0 - 50 * f.value / 2), new Point(-5, 10 - 50 * f.value / 2), new Point(5, 10 - 50 * f.value / 2) };
                tri = tri.Select(x => new Point(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                var line = new Point[] { new Point(0, 0), new Point(0, -50 * f.value / 2) };
                line = line.Select(x => new Point(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSVファイル|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK) using (var sr = new System.IO.StreamReader(ofd.FileName)) fem.meshEncoder.DecodeFromCSV(sr.ReadToEnd());
            fem.solved = false;
            this.Invalidate();
        }

        private void button_solve_Click(object sender, EventArgs e)
        {
            fem.CreateForces_A();
            fem.Solve();
            fem.PostProcessing();
            this.Invalidate();
        }
    }

    class Triangle
    {
        public Point[] points;
        Color[] colors;
        Color centerColor;

        public Triangle(Point[] points, Color[] colors)
        {
            this.points = points;
            this.colors = colors;
            this.centerColor = Color.FromArgb(
                (colors[0].R + colors[1].R + colors[2].R) / 3,
                (colors[0].G + colors[1].G + colors[2].G) / 3,
                (colors[0].B + colors[1].B + colors[2].B) / 3
                );
        }

        public PathGradientBrush GetGradientBrush()
        {
            var gp = new GraphicsPath();
            gp.AddPolygon(points);
            var gb = new PathGradientBrush(gp);
            gb.CenterColor = centerColor;
            gb.SurroundColors = colors;
            return gb;
        }
    }
}
