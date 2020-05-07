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
            var offset = new PointF(50, 50);
            float rate = 200;
            float y_max = 2.0f;
            var triangles = new Triangle[fem.elements.Length];
            for (int i = 0; i < triangles.Length; ++i)
            {
                var points = new PointF[3];
                var colors = new Color[3];
                for (int j = 0; j < points.Length; ++j)
                {
                    points[j] = new PointF(offset.X + rate * fem.nodes[fem.elements[i][j]].X, offset.Y + rate * (y_max - fem.nodes[fem.elements[i][j]].Y));
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
            const float width = 2.0f, height = 2.0f;
            const int N = 20;
            nodes = new PointF[(N + 1) * (N + 1)];
            for (int i = 0; i < N + 1; ++i) for (int j = 0; j < N + 1; ++j) nodes[(N + 1) * j + i] = new PointF(width * i / N, height * j / N);
            dirichletBC = new (int, double)[N + 1 + N - 1 + N + 1];
            for (int i = 0; i < N + 1; ++i) dirichletBC[i] = (i, 0);
            for (int i = 1; i < N; ++i) dirichletBC[N + i] = ((N + 1) * i, 0);
            for (int i = 0; i < N + 1; ++i) dirichletBC[N + 1 + N - 1 + i] = ((N + 1) * N + i, Math.Sin(width * i / N * Math.PI / 4));// other boundary conditions are Neumann type q_n = 0.
            elements = new int[2 * N * N][];
            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < 2 * N; ++j)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 4 == 0) elements[2 * N * i + j] = new int[] { 0, N + 2, N + 1 };
                        if (j % 4 == 1) elements[2 * N * i + j] = new int[] { 0, 1, N + 2 };
                        if (j % 4 == 2) elements[2 * N * i + j] = new int[] { 1, 2, N + 2 };
                        if (j % 4 == 3) elements[2 * N * i + j] = new int[] { 2, N + 3, N + 2 };
                    }
                    else
                    {
                        if (j % 4 == 0) elements[2 * N * i + j] = new int[] { 1, 2, N + 3 };
                        if (j % 4 == 1) elements[2 * N * i + j] = new int[] { 1, N + 3, N + 2};
                        if (j % 4 == 2) elements[2 * N * i + j] = new int[] { 1, N + 2, N + 1 };
                        if (j % 4 == 3) elements[2 * N * i + j] = new int[] { 0, 1, N + 1 };
                    }
                    for (int k = 0; k < 3; ++k) elements[2 * N * i + j][k] += 2 * (j / 4) + (N + 1) * i;
                }
            }
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
