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
    public partial class frmProductView : Form
    {

        public frmProductView()
        {
            InitializeComponent();
            guna2MessageDialog1.Parent = frmMain.Instance;
            // Attach the reusable CellFormatting
            guna2DataGridView1.CellFormatting += (sender, e) => MainClass.ApplyPesoFormatting(sender, e, "dgvAmount", "dgvPrice", "dgvCost", "dgvSale");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvname);
            lb.Items.Add(dgvcatID);
            lb.Items.Add(dgvCategory);
            lb.Items.Add(dgvbarcode);
            lb.Items.Add(dgvCost);
            lb.Items.Add(dgvSale);
            lb.Items.Add(dgvStockBalance);
            lb.Items.Add(dgvReorder);
            string qry = @"SELECT p.proID, p.pName, p.pCatID, c.catName, p.pBarcode, p.pCost, p.pPrice, 
                          ISNULL((SELECT SUM(d.qty) FROM tblDetails d INNER JOIN tblMain m ON m.MainID = d.dMainID WHERE m.mType = 'PUR' AND d.productID = p.proID), 0) -
                          ISNULL((SELECT SUM(d.qty) FROM tblDetails d INNER JOIN tblMain m ON m.MainID = d.dMainID WHERE m.mType = 'SAL' AND d.productID = p.proID), 0) AS StockBalance, p.reorder
                   FROM Product p
                   INNER JOIN Category c ON p.pCatID = c.catID
                   WHERE p.pName LIKE '%" + txtSearch.Text + "%' ORDER BY p.pName ASC";

            //string qty_qry = @"SELECT
            //            ISNULL((SELECT SUM(qty) FROM tblDetails d INNER JOIN tblMain m ON m.MainID = d.dMainID WHERE m.mType = 'PUR' AND d.productID = proID), 0) -
            //            ISNULL((SELECT SUM(qty) FROM tblDetails d INNER JOIN tblMain m ON m.MainID = d.dMainID WHERE m.mType = 'SAL' AND d.productID = proID), 0) AS StockBalance
            //       FROM Product";

            MainClass.LoadData(qry, guna2DataGridView1, lb);
            //MainClass.LoadData(qty_qry, guna2DataGridView1, lb);

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new frmProductAdd());
            LoadData();
        }

        private void frmProductView_Load(object sender, EventArgs e)
        {
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
                    frmProductAdd frm = new frmProductAdd();
                    frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    frm.catID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvcatID"].Value);
                    frm.txtName.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvname"].Value);
                    frm.txtBarcode.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvbarcode"].Value);
                    frm.txtCost.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvCost"].Value);
                    frm.txtPrice.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvsale"].Value);
                    frm.UDReOrder.Value = Convert.ToDecimal(guna2DataGridView1.CurrentRow.Cells["dgvReorder"].Value);
                    //frm.qty.Value = Convert.ToDecimal(guna2DataGridView1.CurrentRow.Cells["dvgQuantity"].Value);

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
                        string qry = "Delete from Product where proID = " + id + "";
                        Hashtable ht = new Hashtable();
                        if (MainClass.SQl(qry, ht) > 0)
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
