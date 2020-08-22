using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preprocessor
{
    public class Mesh
    {
        public List<Point> points = new List<Point>();
        public List<int[]> triangles = new List<int[]>();
        public List<int[]> quadrangles = new List<int[]>();
        public List<int> fixXs = new List<int>();
        public List<int> fixYs = new List<int>();
        public List<(int index, int value)> forceXs = new List<(int, int)>();
        public List<(int index, int value)> forceYs = new List<(int, int)>();
        public Mode mode = Mode.planeStress;

        public enum Mode { planeStress, planeStrain } 
    }
    public class MeshEncoder
    {
        Mesh mesh;

        void ReassignNodeNumber()
        {
            var _points = mesh.points.Select((p, i) => (p, i)).ToList();
            _points = _points.OrderBy(_point => _point.p.Y).ToList();
            _points = _points.OrderBy(_point => _point.p.X).ToList();
            mesh.points = _points.Select(_point => _point.p).ToList();
            var table = new List<int>();
            for (int i = 0; i < mesh.points.Count; ++i) table.Add(_points.FindIndex(_point => _point.i == i));
            for (int i = 0; i < mesh.triangles.Count; ++i)
            {
                for (int j = 0; j < mesh.triangles[i].Length; ++j) mesh.triangles[i][j] = table[mesh.triangles[i][j]];
                mesh.triangles[i] = GetNodeOrderOfPolygon(mesh.triangles[i]);
            }
            for (int i = 0; i < mesh.quadrangles.Count; ++i)
            {
                for (int j = 0; j < mesh.quadrangles[i].Length; ++j) mesh.quadrangles[i][j] = table[mesh.quadrangles[i][j]];
                mesh.quadrangles[i] = GetNodeOrderOfPolygon(mesh.quadrangles[i]);
            }
            for (int i = 0; i < mesh.fixXs.Count; ++i) mesh.fixXs[i] = table[mesh.fixXs[i]];
            for (int i = 0; i < mesh.fixYs.Count; ++i) mesh.fixYs[i] = table[mesh.fixYs[i]];
            for (int i = 0; i < mesh.forceXs.Count; ++i) mesh.forceXs[i] = (table[mesh.forceXs[i].index], mesh.forceXs[i].value);
            for (int i = 0; i < mesh.forceYs.Count; ++i) mesh.forceYs[i] = (table[mesh.forceYs[i].index], mesh.forceYs[i].value);
        }
        double TriangleArea(Point[] p)
        {
            if (p.Length != 3) throw new Exception("Invalid argument of SignedTriangleArea()");
            return (p[1].X * p[2].Y + p[2].X * p[0].Y + p[0].X * p[1].Y - p[2].X * p[1].Y - p[0].X * p[2].Y - p[1].X * p[0].Y) / 2;
        }

        public int[] GetNodeOrderOfPolygon(int[] nodes)
        {
            nodes = nodes.OrderBy(x => x).ToArray();
            var p = new Point[3];
            for (int i = 0; i < p.Length; ++i) p[i] = mesh.points[nodes[i]];
            var n = new int[4];
            nodes.CopyTo(n, 0);
            if (TriangleArea(p) < 0) for (int i = 0; i < 2; ++i) nodes[i] = n[(i + 1) % 2];
            if (nodes.Length == 3) return nodes;
            else if (nodes.Length == 4)
            {
                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < p.Length; ++j) p[j] = mesh.points[nodes[new int[] { 0, 2, 3 }[j]]];
                    if (TriangleArea(p) > 0) break;
                    nodes.CopyTo(n, 0);
                    for (int j = 0; j < 3; ++j) nodes[j] = n[(j + 1) % 3];
                }
                return nodes;
            }
            throw new Exception("GetNodeOrder() failed.");
        }

        public MeshEncoder(Mesh mesh) { this.mesh = mesh; }

        public string EncodeToCSV()
        {
            ReassignNodeNumber();
            var sb = new StringBuilder();
            sb.AppendLine("points");
            foreach (var point in mesh.points) sb.AppendLine(point.X.ToString() + "," + point.Y.ToString());
            sb.AppendLine();
            sb.AppendLine("triangles");
            foreach (var triangle in mesh.triangles) sb.AppendLine(triangle[0].ToString() + "," + triangle[1].ToString() + "," + triangle[2].ToString());
            sb.AppendLine();
            sb.AppendLine("quadrangles");
            foreach (var quadrangle in mesh.quadrangles) sb.AppendLine(quadrangle[0].ToString() + "," + quadrangle[1].ToString() + "," + quadrangle[2].ToString() + "," + quadrangle[3].ToString());
            sb.AppendLine();
            sb.AppendLine("fix X");
            foreach (var idx in mesh.fixXs) sb.AppendLine(idx.ToString());
            sb.AppendLine();
            sb.AppendLine("fix Y");
            foreach (var idx in mesh.fixYs) sb.AppendLine(idx.ToString());
            sb.AppendLine();
            sb.AppendLine("force X");
            foreach (var f in mesh.forceXs) sb.AppendLine(f.index.ToString() + "," + f.value.ToString());
            sb.AppendLine();
            sb.AppendLine("force Y");
            foreach (var f in mesh.forceYs) sb.AppendLine(f.index.ToString() + "," + f.value.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        public void DecodeFromCSV(string text)
        {
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var words = new string[lines.Length][];
            for (int i = 0; i < lines.Length; ++i) words[i] = lines[i].Split(new char[] { ',' });
            var labels = new string[] { "points", "triangles", "quadrangles", "fix X", "fix Y", "force X", "force Y" };
            for (int i = 0; i < labels.Length; ++i)
            {
                for (int j = 0; j < words.Length; ++j)
                {
                    if (words[j][0] == labels[i])
                    {
                        switch (i)
                        {
                            case 0:
                                mesh.points = new List<Point>();
                                for (++j; words[j][0] != ""; ++j) mesh.points.Add(new Point(Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
                                break;
                            case 1:
                                mesh.triangles = new List<int[]>();
                                for (++j; words[j][0] != ""; ++j) mesh.triangles.Add(new int[3] { Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1]), Convert.ToInt16(words[j][2]) });
                                break;
                            case 2:
                                mesh.quadrangles = new List<int[]>();
                                for (++j; words[j][0] != ""; ++j) mesh.quadrangles.Add(new int[4] { Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1]), Convert.ToInt16(words[j][2]), Convert.ToInt16(words[j][3]) });
                                break;
                            case 3:
                                mesh.fixXs = new List<int>();
                                for (++j; words[j][0] != ""; ++j) mesh.fixXs.Add(Convert.ToInt16(words[j][0]));
                                break;
                            case 4:
                                mesh.fixYs = new List<int>();
                                for (++j; words[j][0] != ""; ++j) mesh.fixYs.Add(Convert.ToInt16(words[j][0]));
                                break;
                            case 5:
                                mesh.forceXs = new List<(int index, int value)>();
                                for (++j; words[j][0] != ""; ++j) mesh.forceXs.Add((Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
                                break;
                            case 6:
                                mesh.forceYs = new List<(int index, int value)>();
                                for (++j; words[j][0] != ""; ++j) mesh.forceYs.Add((Convert.ToInt16(words[j][0]), Convert.ToInt16(words[j][1])));
                                break;
                        }
                        break;
                    }
                    if (j == words.Length - 1) throw new Exception("Invalid text");
                }
            }
        }
    }
}
