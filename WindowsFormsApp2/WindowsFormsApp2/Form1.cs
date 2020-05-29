using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;


namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        FEM fem;

        public Form1()
        {
            InitializeComponent();
            fem = new FEM();
            fem.PreProcessing();
            fem.Solver();
            fem.PostProcessing();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var offset = new PointF(80, 80);
            float rate = 200;
            float y_max = 2.0f;
            var triangles = new Triangle[fem.elements.Length];

            var components = new double[fem.nodes.Length];
            //for (int i = 0; i < components.Length; ++i) components[i] = fem.delta[2 * i + 1];
            for (int i = 0; i < components.Length; ++i) components[i] = fem.sigma_node[i][1];
            double max = -1.0e10, min = 1.0e10;
            for (int i = 0; i < components.Length; ++i) if (max < components[i]) max = components[i];
            for (int i = 0; i < components.Length; ++i) if (min > components[i]) min = components[i];
            for (int i = 0; i < components.Length; ++i) components[i] = (components[i] - min) / (max - min);

            for (int i = 0; i < triangles.Length; ++i)
            {
                var points = new PointF[3];
                var colors = new Color[3];
                for (int j = 0; j < points.Length; ++j)
                {
                    points[j] = new PointF(offset.X + rate * fem.nodes[fem.elements[i][j]].X, offset.Y + rate * (y_max - fem.nodes[fem.elements[i][j]].Y));
                    var c = components[fem.elements[i][j]];
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
            for (int j = 0; j < fem.fixedIndexList.Count; ++j)
            {
                PointF[] tri;
                if (fem.fixedIndexList[j] % 2 == 1) tri = new PointF[] { new Point(0, 0), new Point(-10, 20), new Point(10, 20) };
                else tri = new PointF[] { new Point(0, 0), new Point(-20, -10), new Point(-20, 10) };
                var n = fem.fixedIndexList[j] / 2;
                var p = new PointF(offset.X + rate * fem.nodes[n].X, offset.Y + rate * (y_max - fem.nodes[n].Y));
                for (int i = 0; i < tri.Length; ++i)
                {
                    tri[i].X += p.X;
                    tri[i].Y += p.Y;
                }
                e.Graphics.DrawPolygon(new Pen(Color.Blue), tri);
            }
            for (int j = 0; j < fem.f_A.Length; ++j)
            {
                if (fem.f_A[j] != 0)
                {
                    float f = (float)fem.f_A[j];
                    PointF[] tri, line;
                    if (fem.unfixedIndexList[j] % 2 == 1)
                    {
                        tri = new PointF[] { new PointF(0, 0 - 50 * f), new PointF(-5, 10 - 50 * f), new PointF(5, 10 - 50 * f) };
                        line = new PointF[] { new PointF(0, 0), new PointF(0, -50 * f) };
                    }
                    else
                    {
                        tri = new PointF[] { new PointF(0 - 50 * f, 0), new PointF(-10 - 50 * f, -5), new PointF(-10 - 50 * f, 5) };
                        line = new PointF[] { new PointF(0, 0), new PointF(-50 * f, 0) };
                    }
                    var n = fem.unfixedIndexList[j] / 2;
                    var p = new PointF(offset.X + rate * fem.nodes[n].X, offset.Y + rate * (y_max - fem.nodes[n].Y));
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

        class FEM
        {
            const double E = 1000000.0;
            const double nu = 0.3;
            const double thickness = 0.1;
            const double k_spring = 1.0;
            public PointF[] nodes;
            public int[][] elements;
            public List<int> fixedIndexList;
            public List<int> unfixedIndexList;
            private double[][,] B;
            private double[][,] D;
            public double[] f_A;           // foces without fixed coordinate per node
            private double[] delta_A;       // displecements without fixed coordinate per node
            public double[] f;              // all forces per node
            public double[] delta;          // all displecements per node
            public double[][] delta_m;      // displecement per element. {u_i, u_j, u_k, v_i, v_j, v_k} (i, j, k are node numbers of the element)
            public double[][] epsilon;      // strain per element. {ε_x, ε_y, γ_xy}
            public double[][] sigma;        // stress per element. {σ_x, σ_y, τ_xy}
            public double[][] epsilon_node; // average of strains of elements with a node
            public double[][] sigma_node;   // average of stress of elements with a node

            public void PreProcessing()
            {
                // node
                const float width = 2.0f, height = 2.0f;
                const int N = 10;
                nodes = new PointF[(N + 1) * (N + 1)];
                for (int i = 0; i < N + 1; ++i) for (int j = 0; j < N + 1; ++j) nodes[(N + 1) * j + i] = new PointF(width * i / N, height * j / N);

                // element (triangle)
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
                            if (j % 4 == 1) elements[2 * N * i + j] = new int[] { 1, N + 3, N + 2 };
                            if (j % 4 == 2) elements[2 * N * i + j] = new int[] { 1, N + 2, N + 1 };
                            if (j % 4 == 3) elements[2 * N * i + j] = new int[] { 0, 1, N + 1 };
                        }
                        for (int k = 0; k < 3; ++k) elements[2 * N * i + j][k] += 2 * (j / 4) + (N + 1) * i;
                    }
                }

                var e = new List<int[]>();
                for (int i = 0; i < elements.Length; ++i)
                {
                    if (86 <= i && i <= 93) continue;
                    e.Add(elements[i]);
                }
                elements = e.ToArray();


                // restraint condition and external force condition
                fixedIndexList = new List<int>();
                fixedIndexList.Add(0);
                for (int i = 0; i < N + 1; ++i) fixedIndexList.Add(2 * i + 1);
                //for (int i = 0; i < N + 1; ++i) fixedIndexList.Add(2 * i * (N + 1));
                var bs = new bool[nodes.Length * 2];
                for (int i = 0; i < fixedIndexList.Count; ++i) bs[fixedIndexList[i]] = true;
                unfixedIndexList = new List<int>();
                for (int i = 0; i < bs.Length; ++i) if (!bs[i]) unfixedIndexList.Add(i);
                if (fixedIndexList.Count + unfixedIndexList.Count != nodes.Length * 2) MessageBox.Show("Invalide fixedIndexList");
                f_A = new double[unfixedIndexList.Count];
                const double f = 1.0;
                for (int i = 0; i < N + 1; ++i) for (int j = 0; j < 2; ++j) f_A[f_A.Length - 1 - 2 * i - j] = (j % 2 == 0) ? f : 0;
                f_A[f_A.Length - 1] = f / 2;
                f_A[f_A.Length - 1 - 2 * N] = f / 2;
            }

            public void Solver()
            {
                B = new double[elements.Length][,];
                D = new double[elements.Length][,];
                var K_m = new double[elements.Length][,];
                for (int i = 0; i < K_m.Length; ++i)
                {
                    double Delta = 
                        ( nodes[elements[i][1]].X * nodes[elements[i][2]].Y
                        + nodes[elements[i][2]].X * nodes[elements[i][0]].Y
                        + nodes[elements[i][0]].X * nodes[elements[i][1]].Y
                        - nodes[elements[i][2]].X * nodes[elements[i][1]].Y
                        - nodes[elements[i][0]].X * nodes[elements[i][2]].Y
                        - nodes[elements[i][1]].X * nodes[elements[i][0]].Y
                        ) / 2.0;
                    if (Delta < 0) MessageBox.Show("Invalide Delta");
                    B[i] = new double[3, 6];
                    for (int j = 0; j < 6; ++j)
                    {
                        B[i][0, j] = (j < 3) ? nodes[elements[i][(1 + j) % 3]].Y - nodes[elements[i][(2 + j) % 3]].Y : 0;
                        B[i][1, j] = (j < 3) ? 0 : nodes[elements[i][(2 + j) % 3]].X - nodes[elements[i][(1 + j) % 3]].X;
                        B[i][2, j] = (j < 3) ? nodes[elements[i][(2 + j) % 3]].X - nodes[elements[i][(1 + j) % 3]].X : nodes[elements[i][(1 + j) % 3]].Y - nodes[elements[i][(2 + j) % 3]].Y;
                        for (int k = 0; k < 3; ++k) B[i][k, j] /= 2.0 * Delta;
                    }
                    D[i] = new double[3, 3];
                    D[i][0, 0] =  1; D[i][0, 1] = nu; D[i][0, 2] = 0;
                    D[i][1, 0] = nu; D[i][1, 1] =  1; D[i][1, 2] = 0;
                    D[i][2, 0] =  0; D[i][2, 1] =  0; D[i][2, 2] = (1.0 - nu) / 2.0;
                    for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) D[i][j, k] *= E / (1 - nu * nu); // for plane stress
                    var DB = new double[3, 6];
                    for (int j = 0; j < 3; ++j)
                    {
                        for (int k = 0; k < 6; ++k)
                        {
                            DB[j, k] = 0;
                            for (int l = 0; l < 3; ++l) DB[j, k] += D[i][j, l] * B[i][l, k];
                        }
                    }
                    K_m[i] = new double[6, 6];
                    for (int j = 0; j < 6; ++j)
                    {
                        for (int k = 0; k < 6; ++k)
                        {
                            K_m[i][j, k] = 0;
                            for (int l = 0; l < 3; ++l) K_m[i][j, k] += B[i][l, j] * DB[l, k];
                            K_m[i][j, k] *= thickness * Delta;
                        }
                    }
                }

                var K = new double[nodes.Length * 2, nodes.Length * 2];
                for (int i = 0; i < K_m.Length; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        for (int k = 0; k < 3; ++k)
                        {
                            K[2 * elements[i][j], 2 * elements[i][k]] += K_m[i][j, k];                   // u   order of K and order of K_m are different.
                            K[2 * elements[i][j] + 1, 2 * elements[i][k] + 1] += K_m[i][j + 3, k + 3];   // v
                        }
                    }
                }

                var K_AA = new double[f_A.Length, f_A.Length];
                for (int i = 0; i < K_AA.GetLength(0); ++i) for (int j = 0; j < K_AA.GetLength(1); ++j) K_AA[i, j] = K[unfixedIndexList[i], unfixedIndexList[j]];
                delta_A = SolveSimultaneousEquations(K_AA, f_A);
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

            public void PostProcessing()
            {
                delta = new double[2 * nodes.Length];
                for (int i = 0; i < fixedIndexList.Count; ++i) delta[fixedIndexList[i]] = 0;
                for (int i = 0; i < unfixedIndexList.Count; ++i) delta[unfixedIndexList[i]] = delta_A[i];

                epsilon = new double[elements.Length][];
                delta_m = new double[elements.Length][];
                sigma = new double[elements.Length][];
                for (int i = 0; i < elements.Length; ++i)
                {
                    epsilon[i] = new double[3];
                    delta_m[i] = new double[6];
                    for (int j = 0; j < delta_m[i].Length; ++j) delta_m[i][j] = delta[2 * elements[i][j % 3] + j / 3]; // order of delta and order of delta_m are different.
                    for (int j = 0; j < epsilon[i].Length; ++j)
                    {
                        epsilon[i][j] = 0;
                        for (int k = 0; k < 6; ++k) epsilon[i][j] += B[i][j, k] * delta_m[i][k];
                    }
                    sigma[i] = new double[3];
                    for (int j = 0; j < sigma[i].Length; ++j)
                    {
                        sigma[i][j] = 0;
                        for (int k = 0; k < 3; ++k) sigma[i][j] += D[i][j, k] * epsilon[i][k];
                    }
                }
                epsilon_node = new double[nodes.Length][];
                sigma_node = new double[nodes.Length][];
                var e_list = new List<double[]>[nodes.Length];
                var s_list = new List<double[]>[nodes.Length];
                for (int i = 0; i < e_list.Length; ++i) e_list[i] = new List<double[]>();
                for (int i = 0; i < s_list.Length; ++i) s_list[i] = new List<double[]>();
                for (int i = 0; i < elements.Length; ++i)
                {
                    for (int j = 0; j < elements[i].Length; ++j)
                    {
                        e_list[elements[i][j]].Add(epsilon[i]);
                        s_list[elements[i][j]].Add(sigma[i]);
                    }
                }
                for (int i = 0; i < nodes.Length; ++i)
                {
                    epsilon_node[i] = new double[3];
                    for (int j = 0; j < epsilon_node[i].Length; ++j)
                    {
                        epsilon_node[i][j] = 0;
                        for (int k = 0; k < e_list[i].Count; ++k) epsilon_node[i][j] += e_list[i][k][j];
                        epsilon_node[i][j] /= e_list[i].Count;
                    }
                    sigma_node[i] = new double[3];
                    for (int j = 0; j < sigma_node[i].Length; ++j)
                    {
                        sigma_node[i][j] = 0;
                        for (int k = 0; k < s_list[i].Count; ++k) sigma_node[i][j] += s_list[i][k][j];
                        sigma_node[i][j] /= s_list[i].Count;
                    }
                }
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
}
