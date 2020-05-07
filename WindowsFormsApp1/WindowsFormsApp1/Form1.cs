using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        FEM fem;

        public Form1()
        {
            InitializeComponent();
            fem = new FEM();
            fem.Calculate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var offset = new PointF(10, 30);
            float rate = 200;
            var triangles = new Triangle[fem.elements.Length];
            for (int i = 0; i < triangles.Length; ++i)
            {
                var points = new PointF[3];
                var colors = new Color[3];
                for (int j = 0; j < points.Length; ++j)
                {
                    points[j] = new PointF(offset.X + rate * fem.nodes[fem.elements[i][j]].X, offset.Y + rate * fem.nodes[fem.elements[i][j]].Y);
                    var t = fem.temperatures[fem.elements[i][j]];
                    Color color0, color1;
                    double s;
                    if (t < 0.5)
                    {
                        s = 2 * t;
                        color0 = Color.Blue;
                        color1 = Color.LawnGreen;
                    }
                    else
                    {
                        s = 2 * (t - 0.5);
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

        }
    }
    
    class FEM
    {
        const double k_TC = 1.0;

        public PointF[] nodes;
        public double[] temperatures;
        public int[][] elements;
        public (int idx, double T)[] dirichletBC;

        public FEM()
        {
            nodes = new PointF[]
            {
                new PointF(0, 0),
                new PointF(1, 0),
                new PointF(2, 0),
                new PointF(0, 1),
                new PointF(1, 1),
                new PointF(2, 1),
                new PointF(0, 2),
                new PointF(1, 2),
                new PointF(2, 2)
            };
            dirichletBC = new (int, double)[]
            {
                (0, 0),
                (1, 0),
                (2, 0),
                (3, 0),
                (6, Math.Sin(0 * Math.PI / 4)),
                (7, Math.Sin(1 * Math.PI / 4)),
                (8, Math.Sin(2 * Math.PI / 4))
            };  // other boundary conditions are Neumann type q_n = 0.
            elements = new int[][] 
            {
                new int[] { 0, 4, 3 },
                new int[] { 0, 1, 4 },
                new int[] { 1, 2, 4 },
                new int[] { 2, 5, 4 },
                new int[] { 5, 8, 4 },
                new int[] { 8, 7, 4 },
                new int[] { 7, 6, 4 },
                new int[] { 3, 4, 6 }
            };
        }
        public void Calculate()
        {
            var a = new double[elements.Length][];
            var b = new double[elements.Length][];
            var A = new double[elements.Length];
            for (int i = 0; i < elements.Length; ++i)
            {
                a[i] = new double[3];
                for (int j = 0; j < 3; ++j) a[i][j] = nodes[elements[i][(j + 2) % 3]].X - nodes[elements[i][(j + 1) % 3]].X;
                b[i] = new double[3];
                for (int j = 0; j < 3; ++j) b[i][j] = nodes[elements[i][(j + 1) % 3]].Y - nodes[elements[i][(j + 2) % 3]].Y;
                A[i] = (a[i][2] * b[i][1] - a[i][1] * b[i][2]) / 2;
            }
            var sk = new double[elements.Length][,];
            for (int i = 0; i < elements.Length; ++i) sk[i] = new double[3, 3];
            for (int i = 0; i < elements.Length; ++i) for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k)
                        sk[i][j, k] = (b[i][j] * b[i][k] + a[i][j] * a[i][k]) * k_TC / 4 / A[i];
            var K = new double[nodes.Length, nodes.Length];
            for (int i = 0; i < elements.Length; ++i) for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k)
                        K[elements[i][j], elements[i][k]] += sk[i][j, k];
            for (int i = 0; i < dirichletBC.Length; ++i) for (int j = 0; j < nodes.Length; ++j)
                    K[dirichletBC[i].idx, j] = (j == dirichletBC[i].idx) ? 1 : 0;
            var Q = new double[nodes.Length];
            for (int i = 0; i < dirichletBC.Length; ++i) Q[dirichletBC[i].idx] = dirichletBC[i].T;
            for (int i = 0; i < nodes.Length; ++i)
            {
                bool boundary = false;
                for (int j = 0; j < dirichletBC.Length; ++j) if (i == dirichletBC[j].idx) boundary = true;
                if (boundary) continue;
                for (int j = 0; j < dirichletBC.Length; ++j)
                {
                    Q[i] -= K[i, dirichletBC[j].idx] * Q[dirichletBC[j].idx];
                    K[i, dirichletBC[j].idx] = 0;
                }
            }
            temperatures = SolveSimultaneousEquations(K, Q);
        }

        double[] SolveSimultaneousEquations(double[,] a, double[] b)
        {
            var n = b.Length;
            for (var k = 0; k < n - 1; ++k)
            {
                var w = 1 / a[k, k];
                for (var i = k + 1; i < n; ++i)
                {
                    a[i, k] *= w;
                    for (var j = k + 1; j < n; ++j) a[i, j] -= a[i, k] * a[k, j];
                }
            }
            var y = new double[n];
            for (int i = 0; i < n; ++i)
            {
                y[i] = b[i];
                for (int j = 0; j < i; ++j) y[i] -= a[i, j] * y[j];
            }
            var x = new double[n];
            for (int i = 0; i < n; ++i)
            {
                x[n - 1 - i] = y[n - 1 - i];
                for (int j = 0; j < i; ++j) x[n - 1 - i] -= a[n - 1 - i, n - 1 - j] * x[n - 1 - j];
                x[n - 1 - i] /= a[n - 1 - i, n - 1 - i];
            }
            return x;
        }
    }

    class Triangle
    {
        public PointF[] points;
        Color[] colors;
        Color centerColor;

        public Triangle(PointF[] points, Color[] colors)
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
