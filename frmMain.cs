using IM_System.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_System
{
    public partial class frmMain : Form
    {
        static frmMain _obj;
        public static frmMain Instance
        {
            get { if (_obj == null) { _obj = new frmMain(); } return _obj; }
        }
        public frmMain()
        {
            InitializeComponent();
        }
        
        public void Addcontrols(Form F)
        {
            this.CenterPanel.Controls.Clear();
            F.Dock = DockStyle.Fill;
            F.TopLevel = false;
            CenterPanel.Controls.Add(F);
            F.Show();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _obj = this;
            btnMax.PerformClick();

            lblUser.Text = MainClass.USER;
            guna2CirclePictureBox1.Image = MainClass.img;
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmUserView());
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmCategoryView());
        }
    }
}
