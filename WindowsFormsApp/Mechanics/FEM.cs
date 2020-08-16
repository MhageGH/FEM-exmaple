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
        double[,] D_Matrix;

        int[] iFix;
        int[] iForce;
        double[] reduced_displacements; //δ_A : unknown quantity in reduced space which doesn't include coordinates of fixed nodes. 
        double[] reduced_forces;        // f_A
        MatrixesOfTriangle matrixesOfTriangle;
        MatrixesOfQuadrangle matrixesOfQuadrangle;

        public double[] displacements;  // δ
        public double[][] strains;      // ε_x, ε_y, γ_xy
        public double[][] stresses;     // σ_x, σ_y, τ_xy
        public bool solved = false;

        public void Solve()
        {
            GetReducedForces();
            GetD_Matrix();
            matrixesOfTriangle = new MatrixesOfTriangle(mesh, D_Matrix, thickness);
            matrixesOfQuadrangle = new MatrixesOfQuadrangle(mesh, D_Matrix, thickness);
            var overallStiffnessMatrix = new double[mesh.points.Count * 2, mesh.points.Count * 2];    // K
            for (int i = 0; i < matrixesOfTriangle.StiffnessMatrixes.Length; ++i) for (int j = 0; j < 3; ++j) for (int k = 0; k < 3; ++k) for (int l = 0; l < 2; ++l)
                            overallStiffnessMatrix[2 * mesh.triangles[i][j] + l, 2 * mesh.triangles[i][k] + l] += matrixesOfTriangle.StiffnessMatrixes[i][2 * j + l, 2 * k + l];
            // TODO for quadrangle

            var reducedStifnessMatrix = new double[reduced_forces.Length, reduced_forces.Length];    // K_AA
            for (int i = 0; i < reducedStifnessMatrix.GetLength(0); ++i) for (int j = 0; j < reducedStifnessMatrix.GetLength(1); ++j) reducedStifnessMatrix[i, j] = overallStiffnessMatrix[iForce[i], iForce[j]];    // because displacements of fixed nodes are 0.
            reduced_displacements = SolveSimultaneousEquations(reducedStifnessMatrix, reduced_forces); // δ_A = f_A / K_AA
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
            var strains_triangle = GetStrains_triangle();       // strains for element
            var strains_quadrangle = GetStrains_quadrangle();   // strains for element
            GetStrains(strains_triangle, strains_quadrangle);   // strains for node
            GetStresses(strains_triangle, strains_quadrangle);  // stress for node
        }

        void GetDisplacements()
        {
            displacements = new double[2 * mesh.points.Count];
            for (int i = 0; i < iFix.Length; ++i) displacements[iFix[i]] = 0;
            for (int i = 0; i < iForce.Length; ++i) displacements[iForce[i]] = reduced_displacements[i];
        }

        double[][] GetStrains_triangle()
        {
            var strains_triangle = new double[mesh.triangles.Count][].Select(x => new double[3]).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                var d = new double[6];
                for (int j = 0; j < d.Length; ++j) d[j] = displacements[2 * mesh.triangles[i][j / 2] + j % 2];
                for (int j = 0; j < strains_triangle[i].Length; ++j) for (int k = 0; k < 6; ++k) strains_triangle[i][j] += matrixesOfTriangle.B_Matrixes[i][j, k] * d[k];
            }
            return strains_triangle;
        }

        double[][] GetStrains_quadrangle()
        {
            var strains_quadrangle = new double[mesh.quadrangles.Count][].Select(x => new double[4]).ToArray();
            // TODO
            return strains_quadrangle;
        }

        void GetStrains(double[][] strains_triangle, double[][] strains_quadrangle)
        {
            // TODO consider quadrangle
            var averages = new List<double[]>[mesh.points.Count].Select(x => new List<double[]>()).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) averages[mesh.triangles[i][j]].Add(strains_triangle[i]);
            strains = new double[mesh.points.Count][].Select(x => new double[3]).ToArray();
            for (int i = 0; i < strains.Length; ++i) for (int j = 0; j < strains[i].Length; ++j) foreach (var average in averages[i]) strains[i][j] += average[j] / averages[i].Count;
        }

        void GetStresses(double[][] strains_triangle, double[][] strains_quadrangle)
        {
            // TODO consider quadrangle
            var stresses_e = new double[mesh.triangles.Count][].Select(x => new double[3]).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < stresses_e[i].Length; ++j) for (int k = 0; k < 3; ++k) stresses_e[i][j] += D_Matrix[j, k] * strains_triangle[i][k];
            var averages = new List<double[]>[mesh.points.Count].Select(x => new List<double[]>()).ToArray();
            for (int i = 0; i < mesh.triangles.Count; ++i) for (int j = 0; j < mesh.triangles[i].Length; ++j) averages[mesh.triangles[i][j]].Add(stresses_e[i]);
            stresses = new double[mesh.points.Count][].Select(x => new double[3]).ToArray();
            for (int i = 0; i < stresses.Length; ++i) for (int j = 0; j < stresses[i].Length; ++j) foreach (var average in averages[i]) stresses[i][j] += average[j] / averages[i].Count;
        }
    }

    interface IMatrixesOfElement
    {
        double[][,] B_Matrixes { get; } // B_e at center of element

        double[][,] StiffnessMatrixes { get; }  // K_e
    }

    class MatrixesOfTriangle : IMatrixesOfElement
    {
        Preprocessor.Mesh mesh;

        public double[][,] B_Matrixes { get; private set; }   

        public double[][,] StiffnessMatrixes { get; private set; }

        public MatrixesOfTriangle(Preprocessor.Mesh mesh, double[,] D_Matrix, double thickness)
        {
            this.mesh = mesh;
            var areas = GetAreas();
            CreateB_Matrixes(areas);
            CreateStiffnessMatrixes(areas, D_Matrix, thickness);
        }

        double[] GetAreas()
        {
            double[] areas = new double[mesh.triangles.Count];
            for (int i = 0; i < areas.Length; ++i) for (int j = 0; j < 6; ++j)
                    areas[i] += (j < 3 ? 1 : -1) * mesh.points[mesh.triangles[i][(j + 1 + j / 3) % 3]].X * mesh.points[mesh.triangles[i][(j + 2 - j / 3) % 3]].Y / 2.0;
            for (int i = 0; i < areas.Length; ++i) if (areas[i] <= 0) throw new Exception("Invalide area");
            return areas;
        }

        void CreateB_Matrixes(double[] areas)
        {
            B_Matrixes = new double[mesh.triangles.Count][,].Select(x => new double[3, 6]).ToArray();
            for (int i = 0; i < B_Matrixes.Length; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    B_Matrixes[i][0, j] = (j % 2 == 0) ? mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].Y : 0;
                    B_Matrixes[i][1, j] = (j % 2 == 1) ? mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].X - mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].X : 0;
                    B_Matrixes[i][2, j] = (j % 2 == 0) ?
                        mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].X - mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].X :
                        mesh.points[mesh.triangles[i][(1 + j / 2) % 3]].Y - mesh.points[mesh.triangles[i][(2 + j / 2) % 3]].Y;
                    for (int k = 0; k < 3; ++k) B_Matrixes[i][k, j] /= 2.0 * areas[i];
                }
            }
        }

        void CreateStiffnessMatrixes(double[] areas, double[,] D_Matrix, double thickness)
        {
            StiffnessMatrixes = new double[mesh.triangles.Count][,].Select(x => new double[6, 6]).ToArray();
            for (int i = 0; i < StiffnessMatrixes.Length; ++i)
            {
                var DB = new double[3, 6];
                for (int j = 0; j < 3; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) DB[j, k] += D_Matrix[j, l] * B_Matrixes[i][l, k];
                for (int j = 0; j < 6; ++j) for (int k = 0; k < 6; ++k) for (int l = 0; l < 3; ++l) StiffnessMatrixes[i][j, k] += B_Matrixes[i][l, j] * DB[l, k] * thickness * areas[i];
            }
        }
    }

    class MatrixesOfQuadrangle : IMatrixesOfElement
    {
        Preprocessor.Mesh mesh;

        public double[][,] B_Matrixes { get; private set; } // at center of element

        public double[][,] StiffnessMatrixes { get; private set; }

        // Use 2-dimensional 2 point Legendre-Gauss formula for integration to create stiffness matrixes.
        double[][][,] B_MatrixesAtIntegrationPoints;  // [point number in element][element number][row, column]
        readonly double[][] integrationPoints = new double[4][] {
            new double[2] { -1 / Math.Sqrt(3), -1 / Math.Sqrt(3) }, 
            new double[2] { -1 / Math.Sqrt(3), +1 / Math.Sqrt(3) }, 
            new double[2] { +1 / Math.Sqrt(3), -1 / Math.Sqrt(3) }, 
            new double[2] { +1 / Math.Sqrt(3), +1 / Math.Sqrt(3) }
        };
        readonly double[][] integrationWeights = new double[4][] {
            new double[2]{1, 1},
            new double[2]{1, 1},
            new double[2]{1, 1},
            new double[2]{1, 1}
        };

        public MatrixesOfQuadrangle(Preprocessor.Mesh mesh, double[,] D_Matrix, double thickness)
        {
            this.mesh = mesh;
            CreateB_Matrixes();
            CreateStiffnessMatrixes(D_Matrix, thickness);
        }

        void CreateB_Matrixes()
        {
            B_Matrixes = new double[mesh.quadrangles.Count][,].Select(x => new double[3, 8]).ToArray();
            B_MatrixesAtIntegrationPoints = new double[4][][,].Select(y => new double[mesh.quadrangles.Count][,].Select(x => new double[3, 8]).ToArray()).ToArray();
            for (int i = 0; i < mesh.quadrangles.Count; ++i)
            {
                var x = new double[4].Select((s, k) => (double)mesh.points[mesh.quadrangles[i][k]].X).ToArray();
                var y = new double[4].Select((s, k) => (double)mesh.points[mesh.quadrangles[i][k]].Y).ToArray();
                for (int pointNumber = 0; pointNumber < 5; ++pointNumber) // pointNumber < 4 : integration point, pointNumber = 4 : center
                {
                    double xi = (pointNumber < 4) ? integrationPoints[pointNumber][0] : 0;     // ξ
                    double eta = (pointNumber < 4) ? integrationPoints[pointNumber][1] : 0;    // η
                    var J00 = ((-1 + eta) * x[0] + (1 - eta) * x[1] + (1 + eta) * x[2] + (-1 - eta) * x[3]) / 4;    // ∂x/∂ξ
                    var J01 = ((-1 + xi) * x[0] + (-1 - xi) * x[1] + (1 + xi) * x[2] + (1 - xi) * x[3]) / 4;        // ∂x/∂η
                    var J10 = ((-1 + eta) * y[0] + (1 - eta) * y[1] + (1 + eta) * y[2] + (-1 - eta) * y[3]) / 4;    // ∂y/∂ξ
                    var J11 = ((-1 + xi) * y[0] + (-1 - xi) * y[1] + (1 + xi) * y[2] + (1 - xi) * y[3]) / 4;        // ∂y/∂η
                    var detJ = J00 * J11 - J01 * J10;
                    if (detJ <= 0) throw new Exception("Invalide detJ");
                    var dNdxi = new double[4] { (-1 + eta) / 4, (1 - eta) / 4, (1 + eta) / 4, (-1 - eta) / 4 };   // ∂Ni/∂ξ
                    var dNdeta = new double[4] { (-1 + xi) / 4, (-1 - xi) / 4, (1 + xi) / 4, (1 - xi) / 4 };      // ∂Ni/∂η
                    for (int j = 0; j < 8; ++j)
                    {
                        var B = new double[3];
                        B[0] = (j % 2 == 0) ? dNdxi[j / 2] * J11 / detJ - dNdeta[j / 2] * J10 / detJ : 0;
                        B[1] = (j % 2 == 0) ? 0 : -dNdxi[j / 2] * J01 / detJ + dNdeta[j / 2] * J00 / detJ;
                        B[2] = (j % 2 == 0) ? -dNdxi[j / 2] * J01 / detJ + dNdeta[j / 2] * J00 / detJ : dNdxi[j / 2] * J11 / detJ - dNdeta[j / 2] * J10 / detJ;
                        if (pointNumber < 4) for (int k = 0; k < B.Length; ++k) B_MatrixesAtIntegrationPoints[pointNumber][i][k, j] = B[k];
                        else for (int k = 0; k < B.Length; ++k) B_Matrixes[i][k, j] = B[k];
                    }
                }
            }
        }

        void CreateStiffnessMatrixes(double[,]D_Matrix, double thickness)
        {
            // TODO
        }
    }
}
