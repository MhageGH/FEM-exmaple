using System;
using System.Collections.Generic;
using System.Linq;

namespace Mechanics
{
    class FEM
    {
        public Preprocessor.Mesh mesh = new Preprocessor.Mesh();

        const double elasticity = 1000000.0;    // E
        const double poissons_ratio = 0.3;      // ν
        const double thickness = 0.1;
        const double unit_force = 0.5;

        int[] iFix;
        int[] iForce;
        double[][,] B_Matrixes;
        double[][,] D_Matrixes;
        double[] reduced_displacements; //δ_A : unknown quantity in reduced space which doesn't include coordinates of fixed nodes. 
        double[] reduced_forces;        // f_A

        public double[] displacements;  // δ
        public double[][] strains;      // ε_x, ε_y, γ_xy
        public double[][] stresses;     // σ_x, σ_y, τ_xy
        public bool solved = false;

        public void Solve()
        {
            GetReducedForces();
            var stiffnessMatrixes_e = GetStiffnessMatrixes_e();                                  // K_e
            var stiffnessMatrixes = new double[mesh.points.Count * 2, mesh.points.Count * 2];    // K
            for (int i = 0; i < stiffnessMatrixes_e.Length; ++i) for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) for (int l = 0; l < 2; ++l)
                            stiffnessMatrixes[2 * mesh.triangles[i][j] + l, 2 * mesh.triangles[i][k] + l] += stiffnessMatrixes_e[i][j + 3 * l, k + 3 * l];  //  order of K and order of K_e are different.
            var reducedStiffnessMatrixes = new double[reduced_forces.Length, reduced_forces.Length];    // K_AA
            for (int i = 0; i < reducedStiffnessMatrixes.GetLength(0); ++i) for (int j = 0; j < reducedStiffnessMatrixes.GetLength(1); ++j) reducedStiffnessMatrixes[i, j] = stiffnessMatrixes[iForce[i], iForce[j]];    // because displacements of fixed nodes are 0.
            reduced_displacements = SolveSimultaneousEquations(reducedStiffnessMatrixes, reduced_forces); // δ_A = f_A / K_AA
            solved = true;
        }

        void GetReducedForces()
        {
            iFix = mesh.fixXs.Select(x => 2 * x).Union(mesh.fixYs.Select(x => 2 * x + 1)).OrderBy(x => x).ToArray();
            iForce = Enumerable.Range(0, mesh.points.Count * 2).Where(x => !iFix.Contains(x)).ToArray();
            reduced_forces = new double[iForce.Length];
            foreach (var f in mesh.forceXs) reduced_forces[iForce.ToList().FindIndex(a => a == 2 * f.index)] = unit_force * f.value;
            foreach (var f in mesh.forceYs) reduced_forces[iForce.ToList().FindIndex(a => a == 2 * f.index) + 1] = unit_force * f.value;
        }

        double[][,] GetStiffnessMatrixes_e()
        {
            B_Matrixes = new double[mesh.triangles.Count][,].Select(x => x = new double[3, 6]).ToArray();
            D_Matrixes = new double[mesh.triangles.Count][,].Select(x => x = new double[3, 3]).ToArray();
            var stiffnessMatrixes_e = new double[mesh.triangles.Count][,].Select(x => x = new double[6, 6]).ToArray();
            for (int i = 0; i < stiffnessMatrixes_e.Length; ++i)
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
                D_Matrixes[i] = new double[,] { { 1, poissons_ratio, 0 }, { poissons_ratio, 1, 0 }, { 0, 0, (1.0 - poissons_ratio) / 2.0 } };
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) D_Matrixes[i][j, k] *= elasticity / (1 - poissons_ratio * poissons_ratio); // for plane stress
                var DB = new double[3, 6];
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) DB[j, k] += D_Matrixes[i][j, l] * B_Matrixes[i][l, k];
                for (int j = 0; j < 6; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) stiffnessMatrixes_e[i][j, k] += B_Matrixes[i][l, j] * DB[l, k] * thickness * area;
            }
            return stiffnessMatrixes_e;
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
            GetDisplacements();
            var strains_e = GetStrains_e();
            GetStrains(strains_e);
            GetStresses(strains_e);
        }

        void GetDisplacements()
        {
            displacements = new double[2 * mesh.points.Count];
            for (int i = 0; i < iFix.Length; ++i) displacements[iFix[i]] = 0;
            for (int i = 0; i < iForce.Length; ++i) displacements[iForce[i]] = reduced_displacements[i];
        }

        double [][] GetStrains_e()
        {
            var strains_e = new double[mesh.triangles.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                var d = new double[6];
                for (int j = 0; j < d.Length; ++j) d[j] = displacements[2 * mesh.triangles[i][j % 3] + j / 3]; // order of displacements and order of d are different.
                for (int j = 0; j < strains_e[i].Length; ++j) for (int k = 0; k < 6; ++k) strains_e[i][j] += B_Matrixes[i][j, k] * d[k];
            }
            return strains_e;
        }

        void GetStrains(double[][] strains_e)
        {
            var averages = new List<double[]>[mesh.points.Count].Select(x => x = new List<double[]>()).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) averages[mesh.triangles[i][j]].Add(strains_e[i]);
            strains = new double[mesh.points.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < strains.Length; ++i) for (int j = 0; j < strains[i].Length; ++j) foreach (var average in averages[i]) strains[i][j] += average[j] / averages[i].Count;
        }

        void GetStresses(double[][] strains_e)
        {
            var stresses_e = new double[mesh.triangles.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < stresses_e[i].Length; ++j) for (int k = 0; k < 3; ++k) stresses_e[i][j] += D_Matrixes[i][j, k] * strains_e[i][k];
            var averages = new List<double[]>[mesh.points.Count].Select(x => x = new List<double[]>()).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) averages[mesh.triangles[i][j]].Add(stresses_e[i]);
            stresses = new double[mesh.points.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < stresses.Length; ++i) for (int j = 0; j < stresses[i].Length; ++j) foreach (var average in averages[i]) stresses[i][j] += average[j] / averages[i].Count;
        }
    }
}
