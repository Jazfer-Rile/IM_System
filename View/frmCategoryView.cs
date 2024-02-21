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
    public partial class frmCategoryView : Form
    {
        public frmCategoryView()
        {
            InitializeComponent();
            guna2MessageDialog1.Parent = frmMain.Instance;
        }

        private void frmCategoryView_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvname);

            string qry = @"Select * from Category
                where catName like '%" + txtSearch.Text + "%' order by catID desc";

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new frmCategoryAdd());
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Update
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvEdit")
            {
                frmCategoryAdd frm = new frmCategoryAdd();
                frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.txtName.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvname"].Value);
 
                MainClass.BlurBackground(frm);
            }
            //Delete
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
            {
                int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                string qry = "Delete from Category where catID = " + id + "";
                Hashtable ht = new Hashtable();
                if (MainClass.SQl(qry, ht) > 0)
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Show("Delled successfuly..");
                    LoadData();
                }
            }
        }
    }
}
