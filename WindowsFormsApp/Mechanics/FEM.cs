﻿using System;
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
        double[,] D_Matrix;

        int[] iFix;
        int[] iForce;
        double[][,] B_Matrixes_triangle;
        double[][,] B_Matrixes_quadrangle;
        double[] reduced_displacements; //δ_A : unknown quantity in reduced space which doesn't include coordinates of fixed nodes. 
        double[] reduced_forces;        // f_A

        public double[] displacements;  // δ
        public double[][] strains;      // ε_x, ε_y, γ_xy
        public double[][] stresses;     // σ_x, σ_y, τ_xy
        public bool solved = false;

        public void Solve()
        {
            GetReducedForces();
            GetD_Matrix();
            var areas = GetAreas_triangle();
            GetB_Matrixes_triangle(areas);                                                       // B_e (triangle)  
            GetB_Matrixes_quadrangle();                                                          // B_e (quadrangle)
            var stifnessMatrixes_triangle = GetStiffnessMatrixes_triangle(areas);                // K_e (triangle)  
            var stifnessMatrixes_quadrangle = GetStiffnessMatrixes_quadrangle();                 // K_e (quadrangle)
            var stiffnessMatrixes = new double[mesh.points.Count * 2, mesh.points.Count * 2];    // K (overall)     
            for (int i = 0; i < stifnessMatrixes_triangle.Length; ++i) for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) for (int l = 0; l < 2; ++l)
                            stiffnessMatrixes[2 * mesh.triangles[i][j] + l, 2 * mesh.triangles[i][k] + l] += stifnessMatrixes_triangle[i][2 * j + l, 2 * k + l];
            // TODO for quadrangle
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

        void GetD_Matrix()
        {
            if (mesh.mode == Preprocessor.Mesh.Mode.planeStress)
            {
                D_Matrix = new double[,] { { 1, poissons_ratio, 0 }, { poissons_ratio, 1, 0 }, { 0, 0, (1 - poissons_ratio) / 2 } };
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) D_Matrix[j, k] *= elasticity / (1 - poissons_ratio * poissons_ratio);
            }
            else if (mesh.mode == Preprocessor.Mesh.Mode.planeStrain)
            {
                D_Matrix = new double[,] { { 1, poissons_ratio / (1 - poissons_ratio), 0 }, { poissons_ratio / (1 - poissons_ratio), 1, 0 }, { 0, 0, (1 - 2 * poissons_ratio) / 2 / (1 - poissons_ratio) } };
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) D_Matrix[j, k] *= elasticity * (1 - poissons_ratio) / (1 - 2 * poissons_ratio) / (1 + poissons_ratio);
            }
        }

        double[] GetAreas_triangle()
        {
            double[] areas = new double[mesh.triangles.Count];
            for (int i = 0; i < areas.Length; ++i) for (int j = 0; j < 6; ++j) 
                    areas[i] += (j < 3 ? 1 : -1) * mesh.points[mesh.triangles[i][(j + 1 + j / 3) % 3]].X * mesh.points[mesh.triangles[i][(j + 2 - j / 3) % 3]].Y / 2.0;
            for (int i = 0; i < areas.Length; ++i) if (areas[i] < 0) throw new Exception("Invalide area");
            return areas;
        }

        void GetB_Matrixes_triangle(double[] areas)
        {
            B_Matrixes_triangle = new double[mesh.triangles.Count][,].Select(x => x = new double[3, 6]).ToArray();
            for (int i = 0; i < B_Matrixes_triangle.Length; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    B_Matrixes_triangle[i][0, j] = (j % 2 == 0) ? mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].Y : 0;
                    B_Matrixes_triangle[i][1, j] = (j % 2 == 1) ? mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].X - mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].X : 0;
                    B_Matrixes_triangle[i][2, j] = (j % 2 == 0) ?
                        mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].X - mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].X :
                        mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].Y;
                    for (int k = 0; k < 3; ++k) B_Matrixes_triangle[i][k, j] /= 2.0 * areas[i];
                }
            }
        }

        double[][,] GetStiffnessMatrixes_triangle(double[] areas)
        {
            var stifnessMatrixes_triangle = new double[mesh.triangles.Count][,].Select(x => x = new double[6, 6]).ToArray();
            for (int i = 0; i < stifnessMatrixes_triangle.Length; ++i)
            {
                var DB = new double[3, 6];
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) DB[j, k] += D_Matrix[j, l] * B_Matrixes_triangle[i][l, k];
                for (int j = 0; j < 6; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) stifnessMatrixes_triangle[i][j, k] += B_Matrixes_triangle[i][l, j] * DB[l, k] * thickness * areas[i];
            }
            return stifnessMatrixes_triangle;
        }

        void GetB_Matrixes_quadrangle()
        {
            B_Matrixes_quadrangle = new double[mesh.triangles.Count][,].Select(x => x = new double[3, 8]).ToArray();
            for (int i = 0; i < B_Matrixes_triangle.Length; ++i)
            {
                // TODO
            }
        }
        
        double[][,] GetStiffnessMatrixes_quadrangle()
        {
            var stiffnessMatrixes_quadrangle = new double[mesh.quadrangles.Count][,].Select(x => x = new double[8, 8]).ToArray();
            for (int i = 0; i < stiffnessMatrixes_quadrangle.Length; ++i)
            {
                // TODO
            }
            return stiffnessMatrixes_quadrangle;
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
            var strains_triangle = GetStrains_triangle();
            var strains_quadrangle = GetStrains_quadrangle();
            GetStrains(strains_triangle, strains_quadrangle);
            GetStresses(strains_triangle, strains_quadrangle);
        }

        void GetDisplacements()
        {
            displacements = new double[2 * mesh.points.Count];
            for (int i = 0; i < iFix.Length; ++i) displacements[iFix[i]] = 0;
            for (int i = 0; i < iForce.Length; ++i) displacements[iForce[i]] = reduced_displacements[i];
        }

        double[][] GetStrains_triangle()
        {
            var strains_triangle = new double[mesh.triangles.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                var d = new double[6];
                for (int j = 0; j < d.Length; ++j) d[j] = displacements[2 * mesh.triangles[i][j / 2] + j % 2];
                for (int j = 0; j < strains_triangle[i].Length; ++j) for (int k = 0; k < 6; ++k) strains_triangle[i][j] += B_Matrixes_triangle[i][j, k] * d[k];
            }
            return strains_triangle;
        }

        double[][] GetStrains_quadrangle()
        {
            var strains_quadrangle = new double[mesh.quadrangles.Count][].Select(x => x = new double[4]).ToArray();
            // TODO
            return strains_quadrangle;
        }

        void GetStrains(double[][] strains_triangle, double[][] strains_quadrangle)
        {
            // TODO consider quadrangle
            var averages = new List<double[]>[mesh.points.Count].Select(x => x = new List<double[]>()).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) averages[mesh.triangles[i][j]].Add(strains_triangle[i]);
            strains = new double[mesh.points.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < strains.Length; ++i) for (int j = 0; j < strains[i].Length; ++j) foreach (var average in averages[i]) strains[i][j] += average[j] / averages[i].Count;
        }

        void GetStresses(double[][] strains_triangle, double[][] strains_quadrangle)
        {
            // TODO consider quadrangle
            var stresses_e = new double[mesh.triangles.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < stresses_e[i].Length; ++j) for (int k = 0; k < 3; ++k) stresses_e[i][j] += D_Matrix[j, k] * strains_triangle[i][k];
            var averages = new List<double[]>[mesh.points.Count].Select(x => x = new List<double[]>()).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) averages[mesh.triangles[i][j]].Add(stresses_e[i]);
            stresses = new double[mesh.points.Count][].Select(x => x = new double[3]).ToArray();
            for (int i = 0; i < stresses.Length; ++i) for (int j = 0; j < stresses[i].Length; ++j) foreach (var average in averages[i]) stresses[i][j] += average[j] / averages[i].Count;
        }
    }
}
