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
            var triangles = new Triangle[fem.triangles.Count];

            var components = new double[fem.points.Count];
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
                    points[j] = new Point(fem.points[fem.triangles[i][j]].X, fem.points[fem.triangles[i][j]].Y);
                    var c = components[fem.triangles[i][j]];
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
                var p = new Point(fem.points[n].X, fem.points[n].Y);
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
                    var p = new Point(fem.points[n].X, fem.points[n].Y);
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

        private void button2_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSVファイル|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (var sr = new System.IO.StreamReader(ofd.FileName))
                {
                    while (!sr.EndOfStream)
                    {
                        var points = new List<Point>();
                        if (!sr.ReadLine().Contains("nodes")) break;
                        while (true)
                        {
                            var s = sr.ReadLine();
                            if (s == null || !Regex.IsMatch(s, "[^,]")) break;
                            var strs = s.Split(new char[] { ',' });
                            points.Add(new Point(Convert.ToInt16(strs[0]), Convert.ToInt16(strs[1])));
                        }
                        var triangles = new List<int[]>();
                        if (!sr.ReadLine().Contains("elements")) break;
                        while (true)
                        {
                            var s = sr.ReadLine();
                            if (s == null || !Regex.IsMatch(s, "[^,]")) break;
                            var strs = s.Split(new char[] { ',' });
                            triangles.Add(new int[3] { Convert.ToInt16(strs[0]), Convert.ToInt16(strs[1]), Convert.ToInt16(strs[2]) });
                        }
                        var fixXNodeIndex = new List<int>();
                        if (!sr.ReadLine().Contains("fix X")) break;
                        while (true)
                        {
                            var s = sr.ReadLine();
                            if (s == null || !Regex.IsMatch(s, "[^,]")) break;
                            var strs = s.Split(new char[] { ',' });
                            fixXNodeIndex.Add(Convert.ToInt16(strs[0]));
                        }
                        var fixYNodeIndex = new List<int>();
                        if (!sr.ReadLine().Contains("fix Y")) break;
                        while (true)
                        {
                            var s = sr.ReadLine();
                            if (s == null || !Regex.IsMatch(s, "[^,]")) break;
                            var strs = s.Split(new char[] { ',' });
                            fixYNodeIndex.Add(Convert.ToInt16(strs[0]));
                        }
                        var forceXNodeIndexWithValue = new List<(int index, int value)>();
                        if (!sr.ReadLine().Contains("force X")) break;
                        while (true)
                        {
                            var s = sr.ReadLine();
                            if (s == null || !Regex.IsMatch(s, "[^,]")) break;
                            var strs = s.Split(new char[] { ',' });
                            forceXNodeIndexWithValue.Add((Convert.ToInt16(strs[0]), Convert.ToInt16(strs[1])));
                        }
                        var forceYNodeIndexWithValue = new List<(int index, int value)>();
                        if (!sr.ReadLine().Contains("force Y")) break;
                        while (true)
                        {
                            var s = sr.ReadLine();
                            if (s == null || !Regex.IsMatch(s, "[^,]")) break;
                            var strs = s.Split(new char[] { ',' });
                            forceYNodeIndexWithValue.Add((Convert.ToInt16(strs[0]), Convert.ToInt16(strs[1])));
                        }
                        fem.points = points;
                        fem.triangles = triangles;
                        fem.fixXs = fixXNodeIndex;
                        fem.fixYs = fixYNodeIndex;
                        fem.forceXs = forceXNodeIndexWithValue;
                        fem.forceYs = forceYNodeIndexWithValue;
                    }
                }
            }
            fem.CreateForces_A();
            fem.Solver();
            fem.PostProcessing();
            this.Invalidate();
        }
    }

    class FEM
    {
        public List<Point> points;
        public List<int[]> triangles;
        public List<int> fixXs;
        public List<int> fixYs;
        public List<(int index, int value)> forceXs;
        public List<(int index, int value)> forceYs;

        const double E = 1000000.0;
        const double nu = 0.3;
        const double thickness = 0.1;
        const double k_spring = 1.0;
        public List<int> globalFixIndexes;
        public List<int> globalUnfixIndexes;
        private double[][,] B;
        private double[][,] D;
        public double[] forces_A;       // foces without fixed coordinate per node
        private double[] delta_A;       // displecements without fixed coordinate per node
        public double[] forces;         // all forces per node
        public double[] delta;          // all displecements per node
        public double[][] delta_m;      // displecement per element. {u_i, u_j, u_k, v_i, v_j, v_k} (i, j, k are node numbers of the element)
        public double[][] epsilon;      // strain per element. {ε_x, ε_y, γ_xy}
        public double[][] sigma;        // stress per element. {σ_x, σ_y, τ_xy}
        public double[][] epsilon_node; // average of strains of elements with a node
        public double[][] sigma_node;   // average of stress of elements with a node

        public void DefaultParameter()
        {
            // node
            const int N = 10;
            points = new Point[(N + 1) * (N + 1)].ToList();
            for (int i = 0; i < N + 1; ++i) for (int j = 0; j < N + 1; ++j) points[(N + 1) * (N - j) + i] = new Point(i * 40 + 80, j * 40 + 80);

            // triangle
            triangles = new int[2 * N * N][].ToList();
            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < 2 * N; ++j)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 4 == 0) triangles[2 * N * i + j] = new int[] { N+1, N + 2, 0 };
                        if (j % 4 == 1) triangles[2 * N * i + j] = new int[] { N + 2, 1, 0 };
                        if (j % 4 == 2) triangles[2 * N * i + j] = new int[] { N + 2, 2, 1 };
                        if (j % 4 == 3) triangles[2 * N * i + j] = new int[] { N + 2, N + 3, 2 };
                    }
                    else
                    {
                        if (j % 4 == 0) triangles[2 * N * i + j] = new int[] { N + 3, 2, 1 };
                        if (j % 4 == 1) triangles[2 * N * i + j] = new int[] { N + 2, N + 3, 1 };
                        if (j % 4 == 2) triangles[2 * N * i + j] = new int[] { N + 1, N + 2, 1 };
                        if (j % 4 == 3) triangles[2 * N * i + j] = new int[] { N + 1, 1, 0 };
                    }
                    for (int k = 0; k < 3; ++k) triangles[2 * N * i + j][k] += 2 * (j / 4) + (N + 1) * i;
                }
            }
            var _triangles = new List<int[]>();
            for (int i = 0; i < triangles.Count; ++i)
            {
                if (86 <= i && i <= 93) continue;
                _triangles.Add(triangles[i]);
            }
            triangles = _triangles;

            // fix nodes
            fixXs = new List<int>();
            fixXs.Add(0);
            fixYs = new List<int>();
            for (int i = 0; i < N + 1; ++i) fixYs.Add(i);

            // force
            forceXs = new List<(int index, int value)>();
            forceYs = new List<(int index, int value)>();
            forceYs.Add((points.Count - 1 - N, 1));
            for (int i = 1; i < N; ++i) forceYs.Add((points.Count - 1 - N + i, 2));
            forceYs.Add((points.Count - 1, 1));
        }

        public void CreateForces_A()
        {
            globalFixIndexes = new List<int>();
            foreach (var i in fixXs) globalFixIndexes.Add(2 * i);
            foreach (var i in fixYs) globalFixIndexes.Add(2 * i + 1);
            globalFixIndexes.Sort();
            globalUnfixIndexes = new List<int>();
            for (int i = 0; i < points.Count * 2; ++i) if (!globalFixIndexes.Contains(i)) globalUnfixIndexes.Add(i);
            forces_A = new double[globalUnfixIndexes.Count];
            const double f = 0.5;
            for (int i = 0; i < forceXs.Count; ++i)
                forces_A[globalUnfixIndexes.FindIndex(a => a == 2 * forceXs[i].index)] = f * forceXs[i].value;
            for (int i = 0; i < forceYs.Count; ++i)
                forces_A[globalUnfixIndexes.FindIndex(a => a == 2 * forceYs[i].index) + 1] = f * forceYs[i].value;
        }

        public void Solver()
        {
            B = new double[triangles.Count][,];
            D = new double[triangles.Count][,];
            var K_m = new double[triangles.Count][,];
            for (int i = 0; i < K_m.Length; ++i)
            {
                double Delta =
                    (points[triangles[i][1]].X * points[triangles[i][2]].Y
                    + points[triangles[i][2]].X * points[triangles[i][0]].Y
                    + points[triangles[i][0]].X * points[triangles[i][1]].Y
                    - points[triangles[i][2]].X * points[triangles[i][1]].Y
                    - points[triangles[i][0]].X * points[triangles[i][2]].Y
                    - points[triangles[i][1]].X * points[triangles[i][0]].Y
                    ) / 2.0;
                if (Delta < 0) MessageBox.Show("Invalide Delta");
                B[i] = new double[3, 6];
                for (int j = 0; j < 6; ++j)
                {
                    B[i][0, j] = (j < 3) ? points[triangles[i][(1 + j) % 3]].Y - points[triangles[i][(2 + j) % 3]].Y : 0;
                    B[i][1, j] = (j < 3) ? 0 : points[triangles[i][(2 + j) % 3]].X - points[triangles[i][(1 + j) % 3]].X;
                    B[i][2, j] = (j < 3) ? points[triangles[i][(2 + j) % 3]].X - points[triangles[i][(1 + j) % 3]].X : points[triangles[i][(1 + j) % 3]].Y - points[triangles[i][(2 + j) % 3]].Y;
                    for (int k = 0; k < 3; ++k) B[i][k, j] /= 2.0 * Delta;
                }
                D[i] = new double[3, 3];
                D[i][0, 0] = 1; D[i][0, 1] = nu; D[i][0, 2] = 0;
                D[i][1, 0] = nu; D[i][1, 1] = 1; D[i][1, 2] = 0;
                D[i][2, 0] = 0; D[i][2, 1] = 0; D[i][2, 2] = (1.0 - nu) / 2.0;
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

            var K = new double[points.Count * 2, points.Count * 2];
            for (int i = 0; i < K_m.Length; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        K[2 * triangles[i][j], 2 * triangles[i][k]] += K_m[i][j, k];                   // u   order of K and order of K_m are different.
                        K[2 * triangles[i][j] + 1, 2 * triangles[i][k] + 1] += K_m[i][j + 3, k + 3];   // v
                    }
                }
            }

            var K_AA = new double[forces_A.Length, forces_A.Length];
            for (int i = 0; i < K_AA.GetLength(0); ++i) for (int j = 0; j < K_AA.GetLength(1); ++j) K_AA[i, j] = K[globalUnfixIndexes[i], globalUnfixIndexes[j]];
            delta_A = SolveSimultaneousEquations(K_AA, forces_A);
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
            delta = new double[2 * points.Count];
            for (int i = 0; i < globalFixIndexes.Count; ++i) delta[globalFixIndexes[i]] = 0;
            for (int i = 0; i < globalUnfixIndexes.Count; ++i) delta[globalUnfixIndexes[i]] = delta_A[i];

            epsilon = new double[triangles.Count][];
            delta_m = new double[triangles.Count][];
            sigma = new double[triangles.Count][];
            for (int i = 0; i < triangles.Count; ++i)
            {
                epsilon[i] = new double[3];
                delta_m[i] = new double[6];
                for (int j = 0; j < delta_m[i].Length; ++j) delta_m[i][j] = delta[2 * triangles[i][j % 3] + j / 3]; // order of delta and order of delta_m are different.
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
            epsilon_node = new double[points.Count][];
            sigma_node = new double[points.Count][];
            var e_list = new List<double[]>[points.Count];
            var s_list = new List<double[]>[points.Count];
            for (int i = 0; i < e_list.Length; ++i) e_list[i] = new List<double[]>();
            for (int i = 0; i < s_list.Length; ++i) s_list[i] = new List<double[]>();
            for (int i = 0; i < triangles.Count; ++i)
            {
                for (int j = 0; j < triangles[i].Length; ++j)
                {
                    e_list[triangles[i][j]].Add(epsilon[i]);
                    s_list[triangles[i][j]].Add(sigma[i]);
                }
            }
            for (int i = 0; i < points.Count; ++i)
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
