using IM_System.Model;
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

namespace IM_System.View
{
    public partial class frmSupplierView : Form
    {
        public frmSupplierView()
        {
            InitializeComponent();
        }

        public virtual void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        public virtual void btnAdd_Click(object sender, EventArgs e)
        {

            MainClass.BlurBackground(new frmSupplierAdd());
            LoadData();
        }

        private void frmSupplierView_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvname);
            lb.Items.Add(dgvPhone);
            lb.Items.Add(dgvemail);
            lb.Items.Add(dgvAddress);


            string qry = @"Select * from Supplier
                where supName like '%" + txtSearch.Text + "%' order by supID desc";

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }
        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not in the header row
            if (e.RowIndex >= 0)
            {
                // Update
                if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvEdit")
                {
                    frmCustomerAdd frm = new frmCustomerAdd();

                    if (guna2DataGridView1.CurrentRow.Cells["dgvid"].Value != null)
                        frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);

                    frm.txtName.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvname"].Value);
                    frm.txtPhone.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvPhone"].Value);
                    frm.txtEmail.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvemail"].Value);
                    frm.txtAddress.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvAddress"].Value);

                    MainClass.BlurBackground(frm);
                    LoadData();
                }

                // Delete
                if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
                {
                    // Confirm Before delete
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

                    if (guna2MessageDialog1.Show("Are you sure you want to delete") == DialogResult.Yes)
                    {
                        if (guna2DataGridView1.CurrentRow.Cells["dgvid"].Value != null)
                        {
                            int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                            string qry = "Delete from Supplier where supID = " + id + "";
                            Hashtable ht = new Hashtable();

                            if (MainClass.SQl(qry, ht) > 0)
                            {
                                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                                guna2MessageDialog1.Show("Deleted successfully..");
                                LoadData();
                            }
                        }
                    }
                }
            }
        }
    }
}
