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
    public partial class frmSaleView : Form
    {
        public frmSaleView()
        {
            InitializeComponent();
            guna2MessageDialog1.Parent = frmMain.Instance;
        }

        public virtual void frmSaleView_Load(object sender, EventArgs e)
        {
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
            lb.Items.Add(dgvdate);
            lb.Items.Add(dgvCusID);
            lb.Items.Add(dgvCustomer);
            lb.Items.Add(dgvAmount);

            string qry = @"SELECT m.MainID, m.mDate, m.mSupCusID, c.cusName, COALESCE(SUM(d.amount), 0) AS totalAmount
                            FROM tblMain m
                            LEFT JOIN tblDetails d ON d.dMainID = m.MainID
                            LEFT JOIN Customer c ON c.cusID = m.mSupCusID
                            WHERE m.mType = 'SAL'
                            GROUP BY m.MainID, m.mDate, m.mSupCusID, c.cusName
                            ";


            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }
        public virtual void btnAdd_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new frmSaleAdd());
            LoadData();
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not in the header row
            if (e.RowIndex >= 0)
            {
                //Update
                if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvEdit")
                {
                    frmSaleAdd frm = new frmSaleAdd();
                    frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    frm.cusID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvCusID"].Value);

                    MainClass.BlurBackground(frm);
                    LoadData();
                }
                //Delete
                if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
                {

                    //Confirm Before delete
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

                    if (guna2MessageDialog1.Show("Are you sure you want to delete") == DialogResult.Yes)
                    {
                        int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                        string qry = "Delete from tblMain where MainID = " + id + "";
                        string qry2 = "Delete from tblDetails where dMainID = " + id + "";
                        Hashtable ht = new Hashtable();
                        MainClass.SQl(qry, ht);
                        if (MainClass.SQl(qry2, ht) > 0)
                        {
                            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                            guna2MessageDialog1.Show("Deleted successfuly..");
                            LoadData();
                        }
                    }
                }
            }
        }
    }
}
