using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mechanics
{
    public partial class Form2 : Form
    {
        FEM fem;

        public Form2(FEM fem)
        {
            InitializeComponent();
            this.fem = fem;
        }
    }
}
