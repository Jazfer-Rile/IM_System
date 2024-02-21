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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (MainClass.IsValidUser(txtUser.Text, txtPass.Text) == true)
            {
                this.Hide();
                frmMain frm = new frmMain();
                frm.Show();
            }
            else
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("Invalid username or password");

            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (txtPass.PasswordChar == '\0')
            {
                guna2Button2.BringToFront();
                txtPass.PasswordChar = '●';
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (txtPass.PasswordChar == '●')
            {
                guna2Button3.BringToFront();
                txtPass.PasswordChar = '\0';
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
