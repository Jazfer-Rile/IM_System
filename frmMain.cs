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
using Guna.UI2.WinForms;


namespace IM_System
{
    public partial class frmMain : Form
    {
        static frmMain _obj;
        private bool allowLogout = true;
        public static frmMain Instance
        {
            get { if (_obj == null) { _obj = new frmMain(); } return _obj; }
        }
        public frmMain()
        {
            InitializeComponent();
            guna2MessageDialog1.Parent = this;
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

            btnDashBoard.PerformClick();
            
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmUserView());
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmCategoryView());
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmSupplierView());
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmCustomerView());
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmProductView());
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmPurchaseView());
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmSaleView());
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

            if (guna2MessageDialog1.Show("Are you sure you want to exit apllication?") == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnDashBoard_Click(object sender, EventArgs e)
        {
            Addcontrols(new frmDashBoard());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

            DialogResult result = guna2MessageDialog1.Show("Logout Application?");

            if (result == DialogResult.Yes)
            {
                this.Hide();
                frmLogin login = new frmLogin();
                login.ShowDialog();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }
    }
}
