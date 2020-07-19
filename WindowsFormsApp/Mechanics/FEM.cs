using System;
using System.Collections.Generic;
using System.Linq;

namespace Mechanics
{
    class FEM
    {
        public Preprocessor.Mesh mesh = new Preprocessor.Mesh();

        const double E = 1000000.0;
        const double nu = 0.3;
        const double thickness = 0.1;
        const double unit_force = 0.5;

        int[] iFix;
        int[] iForce;
        double[][,] B_Matrixes;
        double[][,] D_Matrixes;
        double[] delta_A;       // displecements without fixed coordinate per node

        public double[] forces_A;       // foces without fixed coordinate per node
        public double[] delta;          // displecements
        public double[][] epsilon;      // strains {ε_x, ε_y, γ_xy}
        public double[][] sigma;        // stress {σ_x, σ_y, τ_xy}
        public bool solved = false;

        public void Solve()
        {
            CreateForces_A();
            var K_m = CreateElementMatrixes();            
            var K = new double[mesh.points.Count * 2, mesh.points.Count * 2];
            for (int i = 0; i < K_m.Length; ++i) for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) for (int l = 0; l < 2; ++l)
                            K[2 * mesh.triangles[i][j] + l, 2 * mesh.triangles[i][k] + l] += K_m[i][j + 3 * l, k + 3 * l];  //  order of K and order of K_m are different.
            var K_AA = new double[forces_A.Length, forces_A.Length];
            for (int i = 0; i < K_AA.GetLength(0); ++i) for (int j = 0; j < K_AA.GetLength(1); ++j) K_AA[i, j] = K[iForce[i], iForce[j]];
            delta_A = SolveSimultaneousEquations(K_AA, forces_A);
            solved = true;
        }

        void CreateForces_A()
        {
            iFix = mesh.fixXs.Select(x => 2 * x).Union(mesh.fixYs.Select(x => 2 * x + 1)).OrderBy(x => x).ToArray();
            iForce = Enumerable.Range(0, mesh.points.Count * 2).Where(x => !iFix.Contains(x)).ToArray();
            forces_A = new double[iForce.Length];
            foreach (var f in mesh.forceXs) forces_A[iForce.ToList().FindIndex(a => a == 2 * f.index)] = unit_force * f.value;
            foreach (var f in mesh.forceYs) forces_A[iForce.ToList().FindIndex(a => a == 2 * f.index) + 1] = unit_force * f.value;
        }

        double[][,] CreateElementMatrixes()
        {
            B_Matrixes = new double[mesh.triangles.Count][,].Select(x => x = new double[3, 6]).ToArray();
            D_Matrixes = new double[mesh.triangles.Count][,].Select(x => x = new double[3, 3]).ToArray();
            var K_m = new double[mesh.triangles.Count][,].Select(x => x = new double[6, 6]).ToArray();
            for (int i = 0; i < K_m.Length; ++i)
            {
                double area = 0;
                for (int j = 0; j < 6; ++j) area += (j < 3 ? 1 : -1) * mesh.points[mesh.triangles[i][(j + 1 + j / 3) % 3]].X * mesh.points[mesh.triangles[i][(j + 2 - j / 3) % 3]].Y / 2.0;
                if (area < 0) throw new Exception("Invalide area");
                for (int j = 0; j < 6; ++j)
                {
                    B_Matrixes[i][0, j] = (j < 3) ? mesh.points[mesh.triangles[i][(1 + j) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j) % 3]].Y : 0;
                    B_Matrixes[i][1, j] = (j < 3) ? 0 : mesh.points[mesh.triangles[i][(2 + j) % 3]].X - mesh.points[mesh.triangles[i][(1 + j) % 3]].X;
                    B_Matrixes[i][2, j] = (j < 3) ? mesh.points[mesh.triangles[i][(2 + j) % 3]].X - mesh.points[mesh.triangles[i][(1 + j) % 3]].X : mesh.points[mesh.triangles[i][(1 + j) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j) % 3]].Y;
                    for (int k = 0; k < 3; ++k) B_Matrixes[i][k, j] /= 2.0 * area;
                }
                D_Matrixes[i] = new double[,] { { 1, nu, 0 }, { nu, 1, 0 }, { 0, 0, (1.0 - nu) / 2.0 } };
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) D_Matrixes[i][j, k] *= E / (1 - nu * nu); // for plane stress
                var DB = new double[3, 6];
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) DB[j, k] += D_Matrixes[i][j, l] * B_Matrixes[i][l, k];
                for (int j = 0; j < 6; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) K_m[i][j, k] += B_Matrixes[i][l, j] * DB[l, k] * thickness * area;
            }
            return K_m;
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
            double[][] epsilon_e, sigma_e;
            GetDelta();
            GetValuesForElements(out epsilon_e, out sigma_e);
            GetValuesForNodes(epsilon_e, sigma_e);
        }

        void GetDelta()
        {
            delta = new double[2 * mesh.points.Count];
            for (int i = 0; i < iFix.Length; ++i) delta[iFix[i]] = 0;
            for (int i = 0; i < iForce.Length; ++i) delta[iForce[i]] = delta_A[i];
        }

        void GetValuesForElements(out double[][] epsilon_e, out double[][] sigma_e)
        {
            epsilon_e = new double[mesh.triangles.Count][].Select(x => x = new double[3]).ToArray();
            sigma_e = new double[mesh.triangles.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                var d = new double[6];
                for (int j = 0; j < d.Length; ++j) d[j] = delta[2 * mesh.triangles[i][j % 3] + j / 3]; // order of delta and order of d are different.
                for (int j = 0; j < epsilon_e[i].Length; ++j) for (int k = 0; k < 6; ++k) epsilon_e[i][j] += B_Matrixes[i][j, k] * d[k];
                for (int j = 0; j < sigma_e[i].Length; ++j) for (int k = 0; k < 3; ++k) sigma_e[i][j] += D_Matrixes[i][j, k] * epsilon_e[i][k];
            }
        }

        void GetValuesForNodes(double[][] epsilon_e, double[][] sigma_e)
        {
            var e_list = new List<double[]>[mesh.points.Count].Select(x => x = new List<double[]>()).ToArray();
            var s_list = new List<double[]>[mesh.points.Count].Select(x => x = new List<double[]>()).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) e_list[mesh.triangles[i][j]].Add(epsilon_e[i]);
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) s_list[mesh.triangles[i][j]].Add(sigma_e[i]);
            epsilon = new double[mesh.points.Count][].Select(x => x = new double[3]).ToArray();
            sigma = new double[mesh.points.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < epsilon.Length; ++i) for (int j = 0; j < epsilon[i].Length; ++j) foreach (var e in e_list[i]) epsilon[i][j] += e[j] / e_list[i].Count;
            for (int i = 0; i < sigma.Length; ++i) for (int j = 0; j < sigma[i].Length; ++j) foreach (var s in s_list[i]) sigma[i][j] += s[j] / s_list[i].Count;
        }
    }
}
