using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_System.Model
{
    public partial class frmSaleAdd : Form
    {
        public frmSaleAdd()
        {
            InitializeComponent();
        }
        public int id = 0;
        public int cusID = 0;

        private void frmSaleAdd_Load(object sender, EventArgs e)
        {
            string qry = @"Select cusID 'id' , cusName 'name' from Customer";
            MainClass.CBFill(qry, cbCustomer);

            if (cusID >0)
            {
                cbCustomer.SelectedValue = cusID;
            }
            loadProductsFromDatabase();
        }
        public void AddItems(string id, string name, string price, Image pimage, string cost)
        {
            var w = new ucProduct()
            {
                PName = name,
                Price = price,
                PImage = pimage,
                PCost = cost,
                id = Convert.ToInt32(id)
            };
            flowLayoutPanel1.Controls.Add(w);

            w.onSelect += (ss, ee) =>
            {
                var wdg = (ucProduct)ss;
                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    if (Convert.ToInt32(item.Cells["dgvproid"].Value) == wdg.id)
                    {
                        item.Cells["dgvqty"].Value = int.Parse(item.Cells["dgvqty"].Value.ToString()) + 1;
                        item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvqty"].Value.ToString()) * int.Parse(item.Cells["dgvPrice"].Value.ToString());
                        GrandTotal();
                        return;
                        
                    }
                }

                //if not find product in data gv
                guna2DataGridView1.Rows.Add(new object[] { 0, wdg.id, wdg.PName, 1, wdg.Price, wdg.Price, wdg.PCost });
                GrandTotal();
            };
        }
        private void GrandTotal()
        {
            double tot = 0;
            lblTotal.Text = "";
            foreach (DataGridViewRow item in  guna2DataGridView1.Rows) 
            {
                tot += double.Parse(item.Cells["dgvAmount"].Value.ToString());
            }
            lblTotal.Text = tot.ToString("N2");
        }
        private void loadProductsFromDatabase()
        {
            string qry = "Select * From Product";
            SqlCommand cmd = new SqlCommand(qry,MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Byte[] imageArray = (byte[])row["PImage"];
                    byte[] imageByteArray = imageArray;

                    AddItems(row["proID"].ToString(), row["pName"].ToString(), row["pPrice"].ToString(),
                            Image.FromStream(new MemoryStream(imageArray)), row["pCost"].ToString());
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            guna2DataGridView1.Rows.Clear();
            txtDate.Value = DateTime.Now;
            cbCustomer.SelectedIndex = 0;
            cbCustomer.SelectedIndex = -1;
            lblTotal.Text = "0.00";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach(var item in flowLayoutPanel1.Controls)
            {
                var pro =(ucProduct)item;
                pro.Visible = pro.PName.ToLower().Contains(txtSearch.Text.ToLower());
            }
        }
    }
}
