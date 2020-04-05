using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Viewer
{
    public partial class Form1 : Form
    {
        //Create dialog for opening save file
        OpenFileDialog ofd = new OpenFileDialog();

        public Form1()
        {
            ofd = new OpenFileDialog();
            InitializeComponent();
        }

        private void fileOpenButton_Click(object sender, EventArgs e)
        {
            DialogResult drResult = ofd.ShowDialog();
        }
    }
}
