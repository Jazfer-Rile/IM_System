using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IM_System;
using IM_System.Model;

namespace IM_System.View
{
    public partial class frmUserView : Form
    {
        public frmUserView()
        {
            InitializeComponent();
            guna2MessageDialog1.Parent = frmMain.Instance;
        }

        public virtual void btnAdd_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new frmUserAdd());
            LoadData();
        }

        public virtual void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvname);
            lb.Items.Add(dgvuserName);
            lb.Items.Add(dgvpass);
            lb.Items.Add(dgvphon);
            lb.Items.Add(dgvrole);

            string qry = @"Select userID, uName, uUsername, uPass, uPhone, uRole from Users
                where uName like '%" + txtSearch.Text + "%' order by userID desc";

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not in the header row
            if (e.RowIndex >= 0)
            {
                //Update
                if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvEdit")
                {
                    frmUserAdd frm = new frmUserAdd();
                    frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    frm.txtName.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvname"].Value);
                    frm.txtUser.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvuserName"].Value);
                    frm.txtPass.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvpass"].Value);
                    frm.txtPhone.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvphon"].Value);
                    frm.cbRole.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvRole"].Value);

                    MainClass.BlurBackground(frm);
                    LoadData();
                }
                //Delete
                if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

                    if (guna2MessageDialog1.Show("Are you sure you want to delete this user?") == DialogResult.Yes)
                    {
                        int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);

                        // Check if the user being deleted is an admin
                        string checkAdminQuery = "SELECT COUNT(*) FROM Users WHERE uRole = 'Admin'";
                        int adminCount = Convert.ToInt32(MainClass.GetSingleValue(checkAdminQuery));

                        if (adminCount > 1)
                        {
                            string qry = "DELETE FROM Users WHERE userID = " + id;
                            Hashtable ht = new Hashtable();
                            if (MainClass.SQl(qry, ht) > 0)
                            {
                                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                                guna2MessageDialog1.Show("Deleted successfully.");
                                LoadData();
                            }
                        }
                        else
                        {
                            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Warning;
                            guna2MessageDialog1.Show("Cannot delete the last remaining admin.");
                        }
                    }
                }


            }
        }

        private void frmUserView_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
