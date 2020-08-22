using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Preprocessor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            mesh = new Mesh();
            meshEncoder = new MeshEncoder(mesh);
        }

        private Mesh mesh;
        private MeshEncoder meshEncoder;
        private List<int> selectedNodes = new List<int>();
        private int movingNode = -1;
        private int baseNode = -1;
        private AlignMode alignMode = AlignMode.vertical;

        private enum AlignMode { vertical, horizontal };

        void UpdateTriangles()
        {
            var triangle = mesh.NormalizeNodeOrderOfPolygon(selectedNodes.ToArray());
            if (mesh.triangles.Any(x => x.SequenceEqual(triangle))) mesh.triangles.RemoveAll(x => x.SequenceEqual(triangle));
            else mesh.triangles.Add(triangle);
        }

        void UpdateQuadrangles()
        {
            var quadrangle = mesh.NormalizeNodeOrderOfPolygon(selectedNodes.ToArray());
            if (mesh.quadrangles.Any(x => x.SequenceEqual(quadrangle))) mesh.quadrangles.RemoveAll(x => x.SequenceEqual(quadrangle));
            else mesh.quadrangles.Add(quadrangle);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int index = -1;
            for (int i = 0; i < mesh.points.Count; ++i)
            {
                var x = e.X - mesh.points[i].X;
                var y = e.Y - mesh.points[i].Y;
                var r2 = x * x + y * y;
                if (r2 < 100)
                {
                    index = i;
                    break;
                }
            }
            if (radioButton_node.Checked)
            {
                if (radioButton_add.Checked)
                {
                    if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle) return;
                    if (index == -1) mesh.points.Add(new Point(e.X, e.Y));
                    else if (!mesh.triangles.Any(x => x.Contains(index)) && !mesh.quadrangles.Any(x => x.Contains(index))
                        && !mesh.fixXs.Contains(index) && !mesh.fixYs.Contains(index)
                        && !mesh.forceXs.Any(x => x.index == index) && !mesh.forceYs.Any(x => x.index == index))
                    {
                        mesh.points.RemoveAt(index);
                        foreach (var triangle in mesh.triangles) for (int i = 0; i < triangle.Length; ++i) if (triangle[i] > index) triangle[i]--;
                        foreach (var quadrangle in mesh.quadrangles) for (int i = 0; i < quadrangle.Length; ++i) if (quadrangle[i] > index) quadrangle[i]--;
                        for (int i = 0; i < mesh.fixXs.Count; ++i) if (mesh.fixXs[i] > index) mesh.fixXs[i]--;
                        for (int i = 0; i < mesh.fixYs.Count; ++i) if (mesh.fixYs[i] > index) mesh.fixYs[i]--;
                        for (int i = 0; i < mesh.forceXs.Count; ++i) if (mesh.forceXs[i].index > index) mesh.forceXs[i] = (mesh.forceXs[i].index - 1, mesh.forceXs[i].value);
                        for (int i = 0; i < mesh.forceYs.Count; ++i) if (mesh.forceYs[i].index > index) mesh.forceYs[i] = (mesh.forceYs[i].index - 1, mesh.forceYs[i].value);
                    }
                }
                else if (radioButton_move.Checked)
                {
                    if (e.Button==MouseButtons.Right)
                    {
                        movingNode = -1;
                        Invalidate();
                        return;
                    }
                    if (e.Button == MouseButtons.Middle) return;
                    if (index == -1 && movingNode == -1) return;
                    else if (index != -1 && movingNode != -1) return;
                    else if (index != -1 && movingNode == -1) movingNode = index;
                    else if (index == -1 && movingNode != -1)
                    {
                        mesh.points[movingNode] = new Point(e.X, e.Y);
                        mesh.triangles = mesh.triangles.Select(x => x.Contains(movingNode) ? mesh.NormalizeNodeOrderOfPolygon(x) : x).ToList();
                        mesh.quadrangles = mesh.quadrangles.Select(x => x.Contains(movingNode) ? mesh.NormalizeNodeOrderOfPolygon(x) : x).ToList();
                        movingNode = -1;
                    }
                }
                else if (radioButton_align.Checked)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        baseNode = -1;
                        Invalidate();
                        return;
                    }
                    if (e.Button == MouseButtons.Middle) return;
                    if (index == -1) return;
                    if (baseNode == -1)
                    {
                        baseNode = index;
                        alignMode = AlignMode.vertical;
                    }
                    else if (index == baseNode)
                    {
                        if (alignMode == AlignMode.vertical) alignMode = AlignMode.horizontal;
                        else if (alignMode == AlignMode.horizontal) alignMode = AlignMode.vertical;
                    }
                    else
                    {
                        if (alignMode == AlignMode.vertical) mesh.points[index] = new Point(mesh.points[baseNode].X, mesh.points[index].Y);
                        else if (alignMode == AlignMode.horizontal) mesh.points[index] = new Point(mesh.points[index].X, mesh.points[baseNode].Y);
                    }
                }
            }
            else if (radioButton_triangle.Checked)
            {
                if (e.Button == MouseButtons.Middle) return;
                if (e.Button == MouseButtons.Right)
                {
                    selectedNodes.Clear();
                    Invalidate();
                    return;
                }
                if (index == -1) return;
                else if (!selectedNodes.Contains(index)) selectedNodes.Add(index);
                if (selectedNodes.Count == 3)
                {
                    UpdateTriangles();
                    selectedNodes.Clear();
                }
            }
            else if (radioButton_quadrangle.Checked)
            {
                if (e.Button == MouseButtons.Middle) return;
                if (e.Button == MouseButtons.Right)
                {
                    selectedNodes.Clear();
                    Invalidate();
                    return;
                }
                if (index == -1) return;
                else if (!selectedNodes.Contains(index)) selectedNodes.Add(index);
                if (selectedNodes.Count == 4)
                {
                    UpdateQuadrangles();
                    selectedNodes.Clear();
                }
            }
            else if (radioButton_fixX.Checked)
            {
                if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle) return;
                if (index == -1) return;
                else if (mesh.fixXs.Contains(index)) mesh.fixXs.Remove(index);
                else mesh.fixXs.Add(index);
            }
            else if (radioButton_fixY.Checked)
            {
                if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle) return;
                if (index == -1) return;
                else if (mesh.fixYs.Contains(index)) mesh.fixYs.Remove(index);
                else mesh.fixYs.Add(index);
            }
            else if (radioButton_forceX.Checked)
            {
                if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle) return;
                if (index == -1) return;
                bool add = true;
                for (int i = 0; i < mesh.forceXs.Count; ++i)
                {
                    if (mesh.forceXs[i].index == index)
                    {
                        if (mesh.forceXs[i].value == 1) mesh.forceXs[i] = (index, 2);
                        else if (mesh.forceXs[i].value == 2) mesh.forceXs.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) mesh.forceXs.Add((index, 1));
            }
            else if (radioButton_forceY.Checked)
            {
                if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle) return;
                if (index == -1) return;
                bool add = true;
                for (int i = 0; i < mesh.forceYs.Count; ++i)
                {
                    if (mesh.forceYs[i].index == index)
                    {
                        if (mesh.forceYs[i].value == 1) mesh.forceYs[i] = (index, 2);
                        else if (mesh.forceYs[i].value == 2) mesh.forceYs.RemoveAt(i);
                        add = false;
                        break;
                    }
                }
                if (add) mesh.forceYs.Add((index, 1));
            }
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var triangle in mesh.triangles)
            {
                var ps = new Point[3];
                for (int i = 0; i < ps.Length; ++i) ps[i] = mesh.points[triangle[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }
            foreach (var quadrangle in mesh.quadrangles)
            {
                var ps = new Point[4];
                for (int i = 0; i < ps.Length; ++i) ps[i] = mesh.points[quadrangle[i]];
                e.Graphics.FillPolygon(Brushes.LawnGreen, ps);
                e.Graphics.DrawPolygon(Pens.Black, ps);
            }

            foreach (var p in mesh.points) e.Graphics.FillEllipse(Brushes.Blue, p.X - 5, p.Y - 5, 10, 10);
            foreach (var i in selectedNodes) e.Graphics.FillEllipse(Brushes.Red, mesh.points[i].X - 5, mesh.points[i].Y - 5, 10, 10);
            if (movingNode != -1)
            {
                e.Graphics.FillEllipse(Brushes.White, mesh.points[movingNode].X - 5, mesh.points[movingNode].Y - 5, 10, 10);
                e.Graphics.DrawEllipse(Pens.Blue, mesh.points[movingNode].X - 5, mesh.points[movingNode].Y - 5, 10, 10);
            }
            if (baseNode != -1)
            {
                if (alignMode == AlignMode.vertical) e.Graphics.DrawLine(Pens.Silver, mesh.points[baseNode].X, mesh.points[baseNode].Y - 100, mesh.points[baseNode].X, mesh.points[baseNode].Y + 100);
                if (alignMode == AlignMode.horizontal) e.Graphics.DrawLine(Pens.Silver, mesh.points[baseNode].X - 100, mesh.points[baseNode].Y, mesh.points[baseNode].X + 100, mesh.points[baseNode].Y);
            }
            if (checkBox_VisibleNodeNumber.Checked)
                for (int i = 0; i < mesh.points.Count; ++i) e.Graphics.DrawString(i.ToString(), DefaultFont, Brushes.Black, mesh.points[i].X + 5, mesh.points[i].Y + 5);
            if (checkBox_VisibleTriangleNumber.Checked)
                for (int i = 0; i < mesh.triangles.Count; ++i) e.Graphics.DrawString(i.ToString() + Environment.NewLine + 
                    "(" + mesh.triangles[i][0].ToString() + "-" + mesh.triangles[i][1].ToString() + "-" + mesh.triangles[i][2].ToString() + ")", DefaultFont, Brushes.Black,
                    (mesh.points[mesh.triangles[i][0]].X + mesh.points[mesh.triangles[i][1]].X + mesh.points[mesh.triangles[i][2]].X) / 3, 
                    (mesh.points[mesh.triangles[i][0]].Y + mesh.points[mesh.triangles[i][1]].Y + mesh.points[mesh.triangles[i][2]].Y) / 3,
                    new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            if (checkBox_VisibleQuadrangleNumber.Checked)
                for (int i = 0; i < mesh.quadrangles.Count; ++i) e.Graphics.DrawString(i.ToString() + Environment.NewLine +
                    "(" + mesh.quadrangles[i][0].ToString() + "-" + mesh.quadrangles[i][1].ToString() + "-" + mesh.quadrangles[i][2].ToString() + "-" + mesh.quadrangles[i][3].ToString() + ")", DefaultFont, Brushes.Black,
                    (mesh.points[mesh.quadrangles[i][0]].X + mesh.points[mesh.quadrangles[i][1]].X + mesh.points[mesh.quadrangles[i][2]].X + mesh.points[mesh.quadrangles[i][3]].X) / 4,
                    (mesh.points[mesh.quadrangles[i][0]].Y + mesh.points[mesh.quadrangles[i][1]].Y + mesh.points[mesh.quadrangles[i][2]].Y + mesh.points[mesh.quadrangles[i][3]].Y) / 4,
                    new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });


            foreach (var i in mesh.fixXs)
            {
                var tri = new Point[] { new Point(0, 0), new Point(-20, -10), new Point(-20, 10) };
                tri = tri.Select(x => new Point(x.X + mesh.points[i].X, x.Y + mesh.points[i].Y)).ToArray();
                e.Graphics.DrawPolygon(Pens.Blue, tri);
            }
            foreach (var i in mesh.fixYs)
            {
                var tri = new Point[] { new Point(0, 0), new Point(-10, 20), new Point(10, 20) };
                tri = tri.Select(x => new Point(x.X + mesh.points[i].X, x.Y + mesh.points[i].Y)).ToArray();
                e.Graphics.DrawPolygon(Pens.Blue, tri);
            }
            foreach (var f in mesh.forceXs)
            {
                var tri = new Point[] { new Point(0 + 50 * f.value / 2, 0), new Point(-10 + 50 * f.value / 2, -5), new Point(-10 + 50 * f.value / 2, 5) };
                tri = tri.Select(x => new Point(x.X + mesh.points[f.index].X, x.Y + mesh.points[f.index].Y)).ToArray();
                var line = new Point[] { new Point(0, 0), new Point(50 * f.value / 2, 0) };
                line = line.Select(x => new Point(x.X + mesh.points[f.index].X, x.Y + mesh.points[f.index].Y)).ToArray();
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
            foreach (var f in mesh.forceYs)
            {
                var tri = new Point[] { new Point(0, 0 - 50 * f.value / 2), new Point(-5, 10 - 50 * f.value / 2), new Point(5, 10 - 50 * f.value / 2) };
                tri = tri.Select(x => new Point(x.X + mesh.points[f.index].X, x.Y + mesh.points[f.index].Y)).ToArray();
                var line = new Point[] { new Point(0, 0), new Point(0, -50 * f.value / 2) };
                line = line.Select(x => new Point(x.X + mesh.points[f.index].X, x.Y + mesh.points[f.index].Y)).ToArray();
                e.Graphics.FillPolygon(Brushes.Red, tri);
                e.Graphics.DrawLine(Pens.Red, line[0], line[1]);
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.FileName = "Mesh.csv";
            if (sfd.ShowDialog() == DialogResult.OK) using (var sw = new System.IO.StreamWriter(sfd.FileName)) sw.Write(meshEncoder.EncodeToCSV());
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSVファイル|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK) using (var sr = new System.IO.StreamReader(ofd.FileName)) meshEncoder.DecodeFromCSV(sr.ReadToEnd());
            Invalidate();
        }

        private void radioButton_node_CheckedChanged(object sender, EventArgs e)
        {
            radioButton_move.Enabled = true;
            radioButton_align.Enabled = true;
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to put or remove node";            
            else toolStripStatusLabel1.Text = "-";
    }

        void RadioButtonNotForNode()
        {
            radioButton_move.Enabled = false;
            radioButton_align.Enabled = false;
            radioButton_add.Checked = true;
            selectedNodes.Clear();
            movingNode = -1;
            baseNode = -1;
            Invalidate();
        }

        private void radioButton_triangle_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonNotForNode();
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to select node to form element.     Right click to cancel.";
            else toolStripStatusLabel1.Text = "-";
        }

        private void radioButton_quadrangle_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonNotForNode();
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to select node to form element.     Right click to cancel.";
            else toolStripStatusLabel1.Text = "-";
        }

        private void radioButton_fixX_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonNotForNode();
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to select node";
            else toolStripStatusLabel1.Text = "-";
        }

        private void radioButton_fixY_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonNotForNode();
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to select node";
            else toolStripStatusLabel1.Text = "-";
        }

        private void radioButton_forceX_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonNotForNode();
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to select node";
            else toolStripStatusLabel1.Text = "-";
        }

        private void radioButton_forceY_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonNotForNode();
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to select node";
            else toolStripStatusLabel1.Text = "-";
        }

        private void radioButton_move_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_move.Checked == false) movingNode = -1;
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to move node.     Right click to cancel.";
            else toolStripStatusLabel1.Text = "-";
            Invalidate();
        }

        private void checkBox_VisibleNodeNumber_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void checkBox_VisibleTriangleNumber_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void checkBox_VisibleQuadrangleNumber_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void radioButton_align_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_align.Checked == false) baseNode = -1;
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to align node.     Right click to cancel.";
            else toolStripStatusLabel1.Text = "-";
            Invalidate();
        }

        private void radioButton_add_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) toolStripStatusLabel1.Text = "Left click to put or remove node";
            else toolStripStatusLabel1.Text = "-";
        }
    }
}
