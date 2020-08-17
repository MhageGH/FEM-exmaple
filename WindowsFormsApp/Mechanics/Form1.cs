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
        Preprocessor.MeshEncoder meshEncoder;

        public Form1()
        {
            InitializeComponent();
            fem = new FEM();
            meshEncoder = new Preprocessor.MeshEncoder(fem.mesh);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var values = new double[fem.mesh.points.Count];
            double max = -1.0e10, min = 1.0e10;
            if (fem.solved)
            {
                switch (toolStripMenuItem2.SelectedIndex)
                {
                    case 0:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.displacements[2 * i]);
                        break;
                    case 1:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.displacements[2 * i + 1]);
                        break;
                    case 2:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.strains[i][0]);
                        break;
                    case 3:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.strains[i][1]);
                        break;
                    case 4:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.strains[i][2]);
                        break;
                    case 5:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.stresses[i][0]);
                        break;
                    case 6:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.stresses[i][1]);
                        break;
                    case 7:
                        for (int i = 0; i < values.Length; ++i) values[i] = Math.Abs(fem.stresses[i][2]);
                        break;
                }
                for (int i = 0; i < values.Length; ++i) if (max < values[i]) max = values[i];
                for (int i = 0; i < values.Length; ++i) if (min > values[i]) min = values[i];
                for (int i = 0; i < values.Length; ++i) values[i] = max > min ? (values[i] - min) / (max - min) : max;
            }
            var gradientTriangles = new List<GradientTriangle>();
            AddGradientTriangles(values, gradientTriangles, fem.mesh.triangles.ToArray());
            foreach (var quadrangle in fem.mesh.quadrangles)
            {
                var triangles = new int[2][] { new int[] { quadrangle[0], quadrangle[1], quadrangle[2] }, new int[] { quadrangle[0], quadrangle[2], quadrangle[3] }};
                AddGradientTriangles(values, gradientTriangles, triangles);
            }
            foreach (var gradientTriangle in gradientTriangles)
            {
                if (fem.solved) e.Graphics.FillRectangle(gradientTriangle.GetGradientBrush(), this.ClientRectangle);
                else e.Graphics.FillPolygon(Brushes.LawnGreen, gradientTriangle.points);
            }
            foreach (var triangle in fem.mesh.triangles)
            {
                var ps = new Point[3];
                for (int i = 0; i < ps.Length; ++i) ps[i] = fem.mesh.points[triangle[i]];
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            foreach (var quadrangle in fem.mesh.quadrangles)
            {
                var ps = new Point[4];
                for (int i = 0; i < ps.Length; ++i) ps[i] = fem.mesh.points[quadrangle[i]];
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            foreach (var i in fem.mesh.fixXs)
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
            var bar = new GradientTriangle[]{
                new GradientTriangle(new Point[] { new Point(530, 220), new Point(560, 220), new Point(530, 300) }, new Color[] { Color.Red, Color.Red, Color.LawnGreen }),
                new GradientTriangle(new Point[] { new Point(560, 220), new Point(560, 300), new Point(530, 300) }, new Color[] { Color.Red, Color.LawnGreen, Color.LawnGreen }),
                new GradientTriangle(new Point[] { new Point(530, 300), new Point(560, 300), new Point(530, 380) }, new Color[] { Color.LawnGreen, Color.LawnGreen, Color.Blue }),
                new GradientTriangle(new Point[] { new Point(560, 300), new Point(560, 380), new Point(530, 380) }, new Color[] { Color.LawnGreen, Color.Blue, Color.Blue })
            };
            foreach(var t in bar) e.Graphics.FillRectangle(t.GetGradientBrush(), this.ClientRectangle);
            if (fem.solved)
            {
                e.Graphics.DrawString(max.ToString("e2"), DefaultFont, Brushes.Black, 560, 220);
                e.Graphics.DrawString(min.ToString("e2"), DefaultFont, Brushes.Black, 560, 380);
            }
        }

        private void AddGradientTriangles(double[] values, List<GradientTriangle> gradientTriangles, int[][] triangles)
        {
            foreach (var triangle in triangles)
            {
                var points = new List<Point>();
                var colors = new List<Color>();
                foreach (var node in triangle)
                {
                    points.Add(fem.mesh.points[node]);
                    var c0 = values[node] < 0.5 ? Color.Blue : Color.LawnGreen;
                    var c1 = values[node] < 0.5 ? Color.LawnGreen : Color.Red;
                    var s = values[node] < 0.5 ? 2 * values[node] : 2 * (values[node] - 0.5);
                    colors.Add(Color.FromArgb((int)(c0.R * (1 - s) + c1.R * s), (int)(c0.G * (1 - s) + c1.G * s), (int)(c0.B * (1 - s) + c1.B * s)));
                }
                gradientTriangles.Add(new GradientTriangle(points.ToArray(), colors.ToArray()));
            }
        }

        private void loadMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSVファイル|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK) using (var sr = new System.IO.StreamReader(ofd.FileName)) meshEncoder.DecodeFromCSV(sr.ReadToEnd());
            label_mesh.Text = System.IO.Path.GetFileName(ofd.FileName);
            fem.solved = false;
            label_state.Text = "Not solved";
            this.Invalidate();
        }

        private void solveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fem.Solve();
            fem.PostProcessing();
            label_state.Text = "Solved";
            this.Invalidate();
        }

        private void toolStripMenuItem2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label_parameter.Text = ((ToolStripComboBox)sender).Text;
            this.Invalidate();
        }

        private void meshEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Preprocessor.Form1().Show();
        }
    }

    class GradientTriangle
    {
        public Point[] points;
        Color[] colors;
        Color centerColor;

        public GradientTriangle(Point[] points, Color[] colors)
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
