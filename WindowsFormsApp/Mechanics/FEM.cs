using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mechanics
{
    class FEM
    {
        public Preprocessor.Mesh mesh;
        public Preprocessor.MeshEncoder meshEncoder;

        const double E = 1000000.0;
        const double nu = 0.3;
        const double thickness = 0.1;
        const double k_spring = 1.0;
        public List<int> globalFixIndexes;
        public List<int> globalUnfixIndexes;
        private double[][,] B_Matrix;
        private double[][,] D_Matrix;
        public double[] forces_A;       // foces without fixed coordinate per node
        private double[] delta_A;       // displecements without fixed coordinate per node
        public double[] forces;         // all forces per node
        public double[] delta;          // all displecements per node
        public double[][] delta_m;      // displecement per element. {u_i, u_j, u_k, v_i, v_j, v_k} (i, j, k are node numbers of the element)
        public double[][] epsilon;      // strain per element. {ε_x, ε_y, γ_xy}
        public double[][] sigma;        // stress per element. {σ_x, σ_y, τ_xy}
        public double[][] epsilon_node; // average of strains of elements with a node
        public double[][] sigma_node;   // average of stress of elements with a node

        public FEM()
        {
            mesh = new Preprocessor.Mesh();
            meshEncoder = new Preprocessor.MeshEncoder(mesh);
        }

        public void DefaultParameter()
        {
            // node
            const int N = 10;
            mesh.points = new Point[(N + 1) * (N + 1)].ToList();
            for (int i = 0; i < N + 1; ++i) for (int j = 0; j < N + 1; ++j) mesh.points[(N + 1) * (N - j) + i] = new Point(i * 40 + 80, j * 40 + 80);

            // triangle
            mesh.triangles = new int[2 * N * N][].ToList();
            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < 2 * N; ++j)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 4 == 0) mesh.triangles[2 * N * i + j] = new int[] { N + 1, N + 2, 0 };
                        if (j % 4 == 1) mesh.triangles[2 * N * i + j] = new int[] { N + 2, 1, 0 };
                        if (j % 4 == 2) mesh.triangles[2 * N * i + j] = new int[] { N + 2, 2, 1 };
                        if (j % 4 == 3) mesh.triangles[2 * N * i + j] = new int[] { N + 2, N + 3, 2 };
                    }
                    else
                    {
                        if (j % 4 == 0) mesh.triangles[2 * N * i + j] = new int[] { N + 3, 2, 1 };
                        if (j % 4 == 1) mesh.triangles[2 * N * i + j] = new int[] { N + 2, N + 3, 1 };
                        if (j % 4 == 2) mesh.triangles[2 * N * i + j] = new int[] { N + 1, N + 2, 1 };
                        if (j % 4 == 3) mesh.triangles[2 * N * i + j] = new int[] { N + 1, 1, 0 };
                    }
                    for (int k = 0; k < 3; ++k) mesh.triangles[2 * N * i + j][k] += 2 * (j / 4) + (N + 1) * i;
                }
            }
            var _triangles = new List<int[]>();
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                if (86 <= i && i <= 93) continue;
                _triangles.Add(mesh.triangles[i]);
            }
            mesh.triangles = _triangles;

            // fix nodes
            mesh.fixXs = new List<int>();
            mesh.fixXs.Add(0);
            mesh.fixYs = new List<int>();
            for (int i = 0; i < N + 1; ++i) mesh.fixYs.Add(i);

            // force
            mesh.forceXs = new List<(int index, int value)>();
            mesh.forceYs = new List<(int index, int value)>();
            mesh.forceYs.Add((mesh.points.Count - 1 - N, 1));
            for (int i = 1; i < N; ++i) mesh.forceYs.Add((mesh.points.Count - 1 - N + i, 2));
            mesh.forceYs.Add((mesh.points.Count - 1, 1));
        }

        public void CreateForces_A()
        {
            globalFixIndexes = new List<int>();
            foreach (var i in mesh.fixXs) globalFixIndexes.Add(2 * i);
            foreach (var i in mesh.fixYs) globalFixIndexes.Add(2 * i + 1);
            globalFixIndexes.Sort();
            globalUnfixIndexes = new List<int>();
            for (int i = 0; i < mesh.points.Count * 2; ++i) if (!globalFixIndexes.Contains(i)) globalUnfixIndexes.Add(i);
            forces_A = new double[globalUnfixIndexes.Count];
            const double f = 0.5;
            for (int i = 0; i < mesh.forceXs.Count; ++i)
                forces_A[globalUnfixIndexes.FindIndex(a => a == 2 * mesh.forceXs[i].index)] = f * mesh.forceXs[i].value;
            for (int i = 0; i < mesh.forceYs.Count; ++i)
                forces_A[globalUnfixIndexes.FindIndex(a => a == 2 * mesh.forceYs[i].index) + 1] = f * mesh.forceYs[i].value;
        }

        public void Solver()
        {
            B_Matrix = new double[mesh.triangles.Count][,];
            D_Matrix = new double[mesh.triangles.Count][,];
            var K_m = new double[mesh.triangles.Count][,];
            for (int i = 0; i < K_m.Length; ++i)
            {
                double Delta =
                    (mesh.points[mesh.triangles[i][1]].X * mesh.points[mesh.triangles[i][2]].Y
                    + mesh.points[mesh.triangles[i][2]].X * mesh.points[mesh.triangles[i][0]].Y
                    + mesh.points[mesh.triangles[i][0]].X * mesh.points[mesh.triangles[i][1]].Y
                    - mesh.points[mesh.triangles[i][2]].X * mesh.points[mesh.triangles[i][1]].Y
                    - mesh.points[mesh.triangles[i][0]].X * mesh.points[mesh.triangles[i][2]].Y
                    - mesh.points[mesh.triangles[i][1]].X * mesh.points[mesh.triangles[i][0]].Y
                    ) / 2.0;
                if (Delta < 0) throw new Exception("Invalide Delta");
                B_Matrix[i] = new double[3, 6];
                for (int j = 0; j < 6; ++j)
                {
                    B_Matrix[i][0, j] = (j < 3) ? mesh.points[mesh.triangles[i][(1 + j) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j) % 3]].Y : 0;
                    B_Matrix[i][1, j] = (j < 3) ? 0 : mesh.points[mesh.triangles[i][(2 + j) % 3]].X - mesh.points[mesh.triangles[i][(1 + j) % 3]].X;
                    B_Matrix[i][2, j] = (j < 3) ? mesh.points[mesh.triangles[i][(2 + j) % 3]].X - mesh.points[mesh.triangles[i][(1 + j) % 3]].X : mesh.points[mesh.triangles[i][(1 + j) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j) % 3]].Y;
                    for (int k = 0; k < 3; ++k) B_Matrix[i][k, j] /= 2.0 * Delta;
                }
                D_Matrix[i] = new double[3, 3];
                D_Matrix[i][0, 0] = 1; D_Matrix[i][0, 1] = nu; D_Matrix[i][0, 2] = 0;
                D_Matrix[i][1, 0] = nu; D_Matrix[i][1, 1] = 1; D_Matrix[i][1, 2] = 0;
                D_Matrix[i][2, 0] = 0; D_Matrix[i][2, 1] = 0; D_Matrix[i][2, 2] = (1.0 - nu) / 2.0;
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) D_Matrix[i][j, k] *= E / (1 - nu * nu); // for plane stress
                var DB = new double[3, 6];
                for (int j = 0; j < 3; ++j)
                {
                    for (int k = 0; k < 6; ++k)
                    {
                        DB[j, k] = 0;
                        for (int l = 0; l < 3; ++l) DB[j, k] += D_Matrix[i][j, l] * B_Matrix[i][l, k];
                    }
                }
                K_m[i] = new double[6, 6];
                for (int j = 0; j < 6; ++j)
                {
                    for (int k = 0; k < 6; ++k)
                    {
                        K_m[i][j, k] = 0;
                        for (int l = 0; l < 3; ++l) K_m[i][j, k] += B_Matrix[i][l, j] * DB[l, k];
                        K_m[i][j, k] *= thickness * Delta;
                    }
                }
            }

            var K = new double[mesh.points.Count * 2, mesh.points.Count * 2];
            for (int i = 0; i < K_m.Length; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        K[2 * mesh.triangles[i][j], 2 * mesh.triangles[i][k]] += K_m[i][j, k];                   // u   order of K and order of K_m are different.
                        K[2 * mesh.triangles[i][j] + 1, 2 * mesh.triangles[i][k] + 1] += K_m[i][j + 3, k + 3];   // v
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
            delta = new double[2 * mesh.points.Count];
            for (int i = 0; i < globalFixIndexes.Count; ++i) delta[globalFixIndexes[i]] = 0;
            for (int i = 0; i < globalUnfixIndexes.Count; ++i) delta[globalUnfixIndexes[i]] = delta_A[i];

            epsilon = new double[mesh.triangles.Count][];
            delta_m = new double[mesh.triangles.Count][];
            sigma = new double[mesh.triangles.Count][];
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                epsilon[i] = new double[3];
                delta_m[i] = new double[6];
                for (int j = 0; j < delta_m[i].Length; ++j) delta_m[i][j] = delta[2 * mesh.triangles[i][j % 3] + j / 3]; // order of delta and order of delta_m are different.
                for (int j = 0; j < epsilon[i].Length; ++j)
                {
                    epsilon[i][j] = 0;
                    for (int k = 0; k < 6; ++k) epsilon[i][j] += B_Matrix[i][j, k] * delta_m[i][k];
                }
                sigma[i] = new double[3];
                for (int j = 0; j < sigma[i].Length; ++j)
                {
                    sigma[i][j] = 0;
                    for (int k = 0; k < 3; ++k) sigma[i][j] += D_Matrix[i][j, k] * epsilon[i][k];
                }
            }
            epsilon_node = new double[mesh.points.Count][];
            sigma_node = new double[mesh.points.Count][];
            var e_list = new List<double[]>[mesh.points.Count];
            var s_list = new List<double[]>[mesh.points.Count];
            for (int i = 0; i < e_list.Length; ++i) e_list[i] = new List<double[]>();
            for (int i = 0; i < s_list.Length; ++i) s_list[i] = new List<double[]>();
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                for (int j = 0; j < mesh.triangles[i].Length; ++j)
                {
                    e_list[mesh.triangles[i][j]].Add(epsilon[i]);
                    s_list[mesh.triangles[i][j]].Add(sigma[i]);
                }
            }
            for (int i = 0; i < mesh.points.Count; ++i)
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
}
