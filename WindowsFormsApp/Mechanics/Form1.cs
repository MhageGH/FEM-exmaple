using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Mechanics
{
    public partial class Form1 : Form
    {
        FEM fem;
        Preprocessor.MeshEncoder meshEncoder;
        Form2 form2;

        enum ElementType { none, triangle, quadrangle }
        ElementType selectedElementType = ElementType.none;
        int selectedElementNumber = 0;

        public Form1()
        {
            InitializeComponent();
            fem = new FEM();
            meshEncoder = new Preprocessor.MeshEncoder(fem.mesh);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var scale = (float)(Convert.ToDouble(scaleToolStripTextBox.Text) * 1e-3);
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
            AddGradientTriangles(values, gradientTriangles, fem.mesh.triangles.ToArray(), scale);
            foreach (var quadrangle in fem.mesh.quadrangles)
            {
                var triangles = new int[2][] { new int[] { quadrangle[0], quadrangle[1], quadrangle[2] }, new int[] { quadrangle[0], quadrangle[2], quadrangle[3] }};
                AddGradientTriangles(values, gradientTriangles, triangles, scale);
            }
            foreach (var gradientTriangle in gradientTriangles)
            {
                if (fem.solved) e.Graphics.FillRectangle(gradientTriangle.GetGradientBrush(), this.ClientRectangle);
                else e.Graphics.FillPolygon(Brushes.LawnGreen, gradientTriangle.points);
            }
            var post_triangle_ps = new PointF[3];
            for (int j = 0; j < fem.mesh.triangles.Count; ++j)
            {
                var ps = new PointF[3];
                for (int i = 0; i < ps.Length; ++i) ps[i] = new PointF(fem.mesh.points[fem.mesh.triangles[j][i]].X / scale, fem.mesh.points[fem.mesh.triangles[j][i]].Y / scale);
                if (selectedElementType == ElementType.triangle && j == selectedElementNumber) ps.CopyTo(post_triangle_ps, 0);
                else e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            var post_quadrangle_ps = new PointF[4];
            for (int j = 0; j < fem.mesh.quadrangles.Count; ++j)
            {
                var ps = new PointF[4];
                for (int i = 0; i < ps.Length; ++i) ps[i] = new PointF(fem.mesh.points[fem.mesh.quadrangles[j][i]].X / scale, fem.mesh.points[fem.mesh.quadrangles[j][i]].Y / scale);
                if (selectedElementType == ElementType.quadrangle && j == selectedElementNumber) ps.CopyTo(post_quadrangle_ps, 0);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            if (selectedElementType == ElementType.triangle) e.Graphics.DrawPolygon(new Pen(Color.Red, 3), post_triangle_ps);
            else if (selectedElementType == ElementType.quadrangle) e.Graphics.DrawPolygon(new Pen(Color.Red, 3), post_quadrangle_ps);
            foreach (var i in fem.mesh.fixXs)
            {
                var tri = new PointF[] { new PointF(0, 0), new PointF(-20, -10), new PointF(-20, 10) };
                tri = tri.Select(x => new PointF(x.X + fem.mesh.points[i].X, x.Y + fem.mesh.points[i].Y)).ToArray();
                e.Graphics.DrawPolygon(Pens.Blue, tri);
            }
            foreach (var i in fem.mesh.fixYs)
            {
                var tri = new PointF[] { new PointF(0, 0), new PointF(-10, 20), new PointF(10, 20) };
                tri = tri.Select(x => new PointF(x.X + fem.mesh.points[i].X, x.Y + fem.mesh.points[i].Y)).ToArray();
                e.Graphics.DrawPolygon(Pens.Blue, tri);
            }
            foreach(var f in fem.mesh.forceXs)
            {
                var tri = new PointF[] { new PointF(0 + 50 * f.value / 2, 0), new PointF(-10 + 50 * f.value / 2, -5), new PointF(-10 + 50 * f.value / 2, 5) };
                tri = tri.Select(x => new PointF(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                var line = new PointF[] { new PointF(0, 0), new PointF(50 * f.value / 2, 0) };
                line = line.Select(x => new PointF(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
            foreach (var f in fem.mesh.forceYs)
            {
                var tri = new PointF[] { new PointF(0, 0 - 50 * f.value / 2), new PointF(-5, 10 - 50 * f.value / 2), new PointF(5, 10 - 50 * f.value / 2) };
                tri = tri.Select(x => new PointF(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                var line = new PointF[] { new PointF(0, 0), new PointF(0, -50 * f.value / 2) };
                line = line.Select(x => new PointF(x.X + fem.mesh.points[f.index].X, x.Y + fem.mesh.points[f.index].Y)).ToArray();
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
            var bar = new GradientTriangle[]{
                new GradientTriangle(new PointF[] { new PointF(530, 220), new PointF(560, 220), new PointF(530, 300) }, new Color[] { Color.Red, Color.Red, Color.LawnGreen }),
                new GradientTriangle(new PointF[] { new PointF(560, 220), new PointF(560, 300), new PointF(530, 300) }, new Color[] { Color.Red, Color.LawnGreen, Color.LawnGreen }),
                new GradientTriangle(new PointF[] { new PointF(530, 300), new PointF(560, 300), new PointF(530, 380) }, new Color[] { Color.LawnGreen, Color.LawnGreen, Color.Blue }),
                new GradientTriangle(new PointF[] { new PointF(560, 300), new PointF(560, 380), new PointF(530, 380) }, new Color[] { Color.LawnGreen, Color.Blue, Color.Blue })
            };
            foreach(var t in bar) e.Graphics.FillRectangle(t.GetGradientBrush(), this.ClientRectangle);
            if (fem.solved)
            {
                var i = toolStripMenuItem2.SelectedIndex;
                string unit = "";
                if (i < 2) unit = " [m]";
                else if (i < 5) unit = "";
                else if (i < 8) unit = " [Pa]";
                e.Graphics.DrawString(max.ToString("e2") + unit, DefaultFont, Brushes.Black, 560, 220);
                e.Graphics.DrawString(min.ToString("e2") + unit, DefaultFont, Brushes.Black, 560, 380);
            }
            e.Graphics.DrawLine(Pens.Black, new Point(20, 530), new Point(20 + 100, 530));
            e.Graphics.DrawLine(Pens.Black, new Point(20, 530 - 5), new Point(20, 530 + 5));
            e.Graphics.DrawLine(Pens.Black, new Point(20 + 100, 530 - 5), new Point(20 + 100, 530 + 5));
            e.Graphics.DrawString((scale * 1e+3 * 100).ToString("F0") + " [mm]", DefaultFont, Brushes.Black, 20 + 30, 530 - 15);
        }

        private void AddGradientTriangles(double[] values, List<GradientTriangle> gradientTriangles, int[][] triangles, float unitLength)
        {
            foreach (var triangle in triangles)
            {
                var points = new List<PointF>();
                var colors = new List<Color>();
                foreach (var node in triangle)
                {
                    points.Add(fem.mesh.points[node]);
                    var c0 = values[node] < 0.5 ? Color.Blue : Color.LawnGreen;
                    var c1 = values[node] < 0.5 ? Color.LawnGreen : Color.Red;
                    var s = values[node] < 0.5 ? 2 * values[node] : 2 * (values[node] - 0.5);
                    colors.Add(Color.FromArgb((int)(c0.R * (1 - s) + c1.R * s), (int)(c0.G * (1 - s) + c1.G * s), (int)(c0.B * (1 - s) + c1.B * s)));
                }
                gradientTriangles.Add(new GradientTriangle(points.Select(x => new PointF(x.X / unitLength, x.Y / unitLength)).ToArray(), colors.ToArray()));
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
            fem.elasticity = Convert.ToDouble(ElasticityToolStripTextBox.Text)*1e+9;
            fem.poissons_ratio = Convert.ToDouble(PoissonsRatioToolStripTextBox.Text);
            fem.thickness = Convert.ToDouble(ThicknessToolStripTextBox.Text)*1e-3;
            fem.unit_force = Convert.ToDouble(UnitForceToolStripTextBox.Text);
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

        private void scaleToolStripTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) this.Invalidate();
        }

        private void elementInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form2 = new Form2(fem);
            form2.Show();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            var scale = (float)(Convert.ToDouble(scaleToolStripTextBox.Text) * 1e-3);
            for (int i = 0; i < fem.mesh.triangles.Count; ++i)
            {
                var triangle = fem.mesh.triangles[i];
                var v = new PointF[3].Select((x, j) => new PointF(fem.mesh.points[triangle[j]].X / scale - e.X, fem.mesh.points[triangle[j]].Y / scale - e.Y)).ToArray();
                if (CrossProduct(v[0], v[1]) >= 0 && CrossProduct(v[1], v[2]) >= 0 && CrossProduct(v[2], v[0]) >= 0)
                {
                    selectedElementType = ElementType.triangle;
                    selectedElementNumber = i;
                    this.Invalidate();
                    form2.ElementTypeLabel.Text = "Triangle";
                    form2.ElementNumberLabel.Text = i.ToString();
                    form2.NodeNumberListLabel.Text = triangle[0].ToString() + ", " + triangle[1].ToString() + ", " + triangle[2].ToString();
                    string s = "";
                    for (int j = 0; j < triangle.Length; ++j) s += "(" + fem.mesh.points[triangle[j]].X.ToString() + ", " + fem.mesh.points[triangle[j]].Y.ToString() + ")" + ((j < triangle.Length - 1) ? ", " : "");
                    form2.NodePositionListLabel.Text = s;
                    return;
                }
            }
            for (int i = 0; i < fem.mesh.quadrangles.Count; ++i)
            {
                var quadrangle = fem.mesh.quadrangles[i];
                var triangles = new int[2][] { new int[] { quadrangle[0], quadrangle[1], quadrangle[2] }, new int[] { quadrangle[0], quadrangle[2], quadrangle[3] } };
                for (int k = 0; k < triangles.Length; ++k)
                {
                    var triangle = triangles[k];
                    var v = new PointF[3].Select((x, j) => new PointF(fem.mesh.points[triangle[j]].X / scale - e.X, fem.mesh.points[triangle[j]].Y / scale - e.Y)).ToArray();
                    if (CrossProduct(v[0], v[1]) >= 0 && CrossProduct(v[1], v[2]) >= 0 && CrossProduct(v[2], v[0]) >= 0)
                    {
                        selectedElementType = ElementType.quadrangle;
                        selectedElementNumber = i;
                        this.Invalidate();
                        form2.ElementTypeLabel.Text = "Quadrangle";
                        form2.ElementNumberLabel.Text = i.ToString();
                        form2.NodeNumberListLabel.Text = quadrangle[0].ToString() + ", " + quadrangle[1].ToString() + ", " + quadrangle[2].ToString() + ", " + quadrangle[3].ToString();
                        string s = "";
                        for (int j = 0; j < quadrangle.Length; ++j) s += "(" + fem.mesh.points[quadrangle[j]].X.ToString() + ", " + fem.mesh.points[quadrangle[j]].Y.ToString() + ")" + ((j < quadrangle.Length - 1) ? ", " : "");
                        form2.NodePositionListLabel.Text = s;
                        return;
                    }
                }
            }
        }

        float CrossProduct(PointF p0, PointF p1)
        {
            return p0.X*p1.Y - p0.Y * p1.X;
        }
    }

    class GradientTriangle
    {
        public PointF[] points;
        Color[] colors;
        Color centerColor;

        public GradientTriangle(PointF[] points, Color[] colors)
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
