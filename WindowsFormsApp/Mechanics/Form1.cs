using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Mechanics
{
    public partial class Form1 : Form
    {
        FEM fem;

        public Form1()
        {
            InitializeComponent();
            fem = new FEM();
            fem.DefaultParameter();
            fem.CreateForces_A();
            fem.Solver();
            fem.PostProcessing();
            listBox1.SelectedIndex = 6;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var triangles = new Triangle[fem.mesh.triangles.Count];

            var components = new double[fem.mesh.points.Count];
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.delta[2 * i]);
                    break;
                case 1:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.delta[2 * i + 1]);
                    break;
                case 2:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.epsilon_node[i][0]);
                    break;
                case 3:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.epsilon_node[i][1]);
                    break;
                case 4:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.epsilon_node[i][2]);
                    break;
                case 5:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.sigma_node[i][0]);
                    break;
                case 6:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.sigma_node[i][1]);
                    break;
                case 7:
                    for (int i = 0; i < components.Length; ++i) components[i] = Math.Abs(fem.sigma_node[i][2]);
                    break;
            }
            double max = -1.0e10, min = 1.0e10;
            for (int i = 0; i < components.Length; ++i) if (max < components[i]) max = components[i];
            for (int i = 0; i < components.Length; ++i) if (min > components[i]) min = components[i];
            for (int i = 0; i < components.Length; ++i) components[i] = max > min ? (components[i] - min) / (max - min) : max;

            for (int i = 0; i < triangles.Length; ++i)
            {
                var points = new Point[3];
                var colors = new Color[3];
                for (int j = 0; j < points.Length; ++j)
                {
                    points[j] = new Point(fem.mesh.points[fem.mesh.triangles[i][j]].X, fem.mesh.points[fem.mesh.triangles[i][j]].Y);
                    var c = components[fem.mesh.triangles[i][j]];
                    Color color0, color1;
                    double s;
                    if (c < 0.5)
                    {
                        s = 2 * c;
                        color0 = Color.Blue;
                        color1 = Color.LawnGreen;
                    }
                    else
                    {
                        s = 2 * (c - 0.5);
                        color0 = Color.LawnGreen;
                        color1 = Color.Red;
                    }
                    colors[j] = Color.FromArgb(
                        (int)(color0.R * (1 - s) + color1.R * s),
                        (int)(color0.G * (1 - s) + color1.G * s),
                        (int)(color0.B * (1 - s) + color1.B * s)
                        );
                }
                triangles[i] = new Triangle(points, colors);
            }
            foreach (var triangle in triangles)
            {
                e.Graphics.FillRectangle(triangle.GetGradientBrush(), this.ClientRectangle);
                e.Graphics.DrawPolygon(new Pen(Color.Black), triangle.points);
            }
            for (int j = 0; j < fem.globalFixIndexes.Count; ++j)
            {
                Point[] tri;
                if (fem.globalFixIndexes[j] % 2 == 1) tri = new Point[] { new Point(0, 0), new Point(-10, 20), new Point(10, 20) };
                else tri = new Point[] { new Point(0, 0), new Point(-20, -10), new Point(-20, 10) };
                var n = fem.globalFixIndexes[j] / 2;
                var p = new Point(fem.mesh.points[n].X, fem.mesh.points[n].Y);
                for (int i = 0; i < tri.Length; ++i)
                {
                    tri[i].X += p.X;
                    tri[i].Y += p.Y;
                }
                e.Graphics.DrawPolygon(new Pen(Color.Blue), tri);
            }
            for (int j = 0; j < fem.forces_A.Length; ++j)
            {
                if (fem.forces_A[j] != 0)
                {
                    float f = (float)fem.forces_A[j];
                    Point[] tri, line;
                    if (fem.globalUnfixIndexes[j] % 2 == 1)
                    {
                        tri = new Point[] { new Point(0, (int)(0 - 50 * f)), new Point(-5, (int)(10 - 50 * f)), new Point(5, (int)(10 - 50 * f)) };
                        line = new Point[] { new Point(0, 0), new Point(0, (int)(-50 * f)) };
                    }
                    else
                    {
                        tri = new Point[] { new Point((int)(0 + 50 * f), 0), new Point((int)(-10 + 50 * f), -5), new Point((int)(-10 + 50 * f), 5) };
                        line = new Point[] { new Point(0, 0), new Point((int)(-50 * f), 0) };
                    }
                    var n = fem.globalUnfixIndexes[j] / 2;
                    var p = new Point(fem.mesh.points[n].X, fem.mesh.points[n].Y);
                    for (int i = 0; i < tri.Length; ++i)
                    {
                        tri[i].X += p.X;
                        tri[i].Y += p.Y;
                    }
                    for (int i = 0; i < line.Length; ++i)
                    {
                        line[i].X += p.X;
                        line[i].Y += p.Y;
                    }
                    e.Graphics.FillPolygon(Brushes.Red, tri);
                    e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
                }
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
            fem.CreateForces_A();
            fem.Solver();
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
