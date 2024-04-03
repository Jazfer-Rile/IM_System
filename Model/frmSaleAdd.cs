using Guna.UI2.WinForms;
using System;
using System.Collections;
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
using static IM_System.frmProductAdd;

namespace IM_System.Model
{
    public partial class frmSaleAdd : Form
    {
        public frmSaleAdd()
        {
            InitializeComponent();
            IM_System.frmProductAdd instance = new IM_System.frmProductAdd();
            txtBarcode.KeyPress += instance.guna2barcodetextbox_KeyPress;
            txtDate.Value = DateTime.Now;
        }
        public int id = 0;
        public int cusID = 0;

        private void guna2barcodetextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the input is not a digit or control (like backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignore the character
            }
        }

        private void frmSaleAdd_Load(object sender, EventArgs e)
        {
            string qry = @"Select cusID 'id' , cusName 'name' from Customer";
            MainClass.CBFill(qry, cbCustomer);

            if (cusID > 0)
            {
                cbCustomer.SelectedValue = cusID;
                LoadForEdit();
                GrandTotal();
            }
            else
            {
                // Handle case where cusID is 0 or not set
                LoadForEditWithoutCustomer();
                GrandTotal();
            }
            loadProductsFromDatabase();
        }

        public void AddItems(string id, string name, string price, Image pimage, string cost, int stock)
        {
            var w = new ucProduct()
            {
                PName = name,
                Price = price,
                PImage = pimage,
                PCost = cost,
                id = Convert.ToInt32(id),
                Stock = stock,
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

            foreach (DataGridViewRow item in guna2DataGridView1.Rows)
            {
                // Check if the cell and its value are not null
                if (item.Cells["dgvAmount"].Value != null)
                {
                    tot += double.Parse(item.Cells["dgvAmount"].Value.ToString());
                }
            }

            lblTotal.Text = tot.ToString("N2");
        }
        private void RefreshProductPanel()
        {
            flowLayoutPanel1.Controls.Clear(); // Clear existing controls
            loadProductsFromDatabase(); // Reload products
        }

        private void loadProductsFromDatabase()
        {
            try
            {
                string qry = "SELECT p.proID, p.pName, p.pPrice, p.pCost, p.PImage, " +
               "(SELECT ISNULL(SUM(d.qty), 0) FROM tblDetails d " +
               "INNER JOIN tblMain m ON m.MainID = d.dMainID " +
               "WHERE m.mType = 'PUR' AND d.productID = p.proID) - " +
               "(SELECT ISNULL(SUM(d.qty), 0) FROM tblDetails d " +
               "INNER JOIN tblMain m ON m.MainID = d.dMainID " +
               "WHERE m.mType = 'SAL' AND d.productID = p.proID) AS Stock " +
               "FROM Product p";



                SqlCommand cmd = new SqlCommand(qry, MainClass.con);
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
                                Image.FromStream(new MemoryStream(imageArray)), row["pCost"].ToString(),
                                Convert.ToInt32(row["Stock"]));
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, show an error message, etc.)
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            foreach (Control ctrl in flowLayoutPanel1.Controls)
            {
                if (ctrl is ucProduct)
                {
                    ucProduct pro = (ucProduct)ctrl;
                    pro.Visible = pro.PName.ToLower().Contains(txtSearch.Text.ToLower());
                }
            }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string qry = "SELECT * FROM Product WHERE pBarcode LIKE '" + txtBarcode.Text + "'";
                SqlCommand cmd = new SqlCommand(qry, MainClass.con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                    {
                        if (Convert.ToInt32(item.Cells["dgvproid"].Value) == int.Parse(row["proID"].ToString()))
                        {
                            item.Cells["dgvqty"].Value = int.Parse(item.Cells["dgvqty"].Value.ToString()) + 1;
                            item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvqty"].Value.ToString()) * int.Parse(item.Cells["dgvPrice"].Value.ToString());
                            GrandTotal();
                            txtBarcode.Text = "";
                            return;
                        }
                    }

                    guna2DataGridView1.Rows.Add(new object[] {
                        0,
                        row["proID"].ToString(),
                        row["pName"].ToString(),
                        1,
                        row["pPrice"].ToString(),
                        row["pPrice"].ToString(),
                        row["pCost"].ToString()
                    });
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MainClass.Validation(this) == false)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("Please removed errors");
                return;
            }

            string qry1 = "";//for main Table
            string qry2 = "";//for details table
            int record = 0;

            if (id == 0) // insert
            {
                qry1 = @"INSERT INTO tblMain VALUES (@date, @type, @supID);
                 SELECT SCOPE_IDENTITY()";
            }
            else
            {
                qry1 = @"UPDATE tblMain SET mDate = @date, mType = @type, mSupCusID = @supID
                 WHERE MainID = @id";
            }

            SqlCommand cmd1 = new SqlCommand(qry1, MainClass.con);
            cmd1.Parameters.AddWithValue("@id", id);
            cmd1.Parameters.AddWithValue("@date", Convert.ToDateTime(txtDate.Value).Date);
            cmd1.Parameters.AddWithValue("@type", "SAL");
            cmd1.Parameters.AddWithValue("@supID", Convert.ToInt32(cbCustomer.SelectedValue));
            if (MainClass.con.State == ConnectionState.Closed)
            {
                MainClass.con.Open();
            }

            if (id == 0)
            {
                id = Convert.ToInt32(cmd1.ExecuteScalar());
            }
            else
            {
                cmd1.ExecuteNonQuery();
            }
            //Insert Details Table
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                int did = Convert.ToInt32(row.Cells["dgvid"].Value);

                if (did == 0) // insert
                {
                    qry2 = @"INSERT INTO tblDetails VALUES (@mID, @proID, @qty, @price, @amount, @cost)";
                }
                else
                {
                    qry2 = @"UPDATE tblDetails SET dMainID = @mID, productID = @proID,
                     qty = @qty, price = @price, amount = @amount, cost = @cost
                     WHERE detailID = @id";
                }
                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@id", did);
                cmd2.Parameters.AddWithValue("@mID", id);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["dgvproid"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvqty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToInt32(row.Cells["dgvCost"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToInt32(row.Cells["dgvAmount"].Value));
                cmd2.Parameters.AddWithValue("@cost", Convert.ToInt32(row.Cells["dgvCost"].Value));
                record += cmd2.ExecuteNonQuery();

            }
            if (record > 0)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                guna2MessageDialog1.Show("Saved Successfully");
                RefreshProductPanel();

                id = 0;
                cusID = 0;
                txtDate.Value = DateTime.Now;
                cbCustomer.SelectedIndex = 0;
                cbCustomer.SelectedIndex = -1;
                guna2DataGridView1.Rows.Clear();
                lblTotal.Text = "0.00";
            }
        }
        private void LoadForEdit()
        {
            string qry = "SELECT * FROM tblDetails INNER JOIN Product ON proID = productID WHERE dMainID = " + id;
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                string did;
                string pid;
                string pname;
                string qty;
                string cost;
                string amt;

                did = row["detailID"].ToString();
                pname = row["pName"].ToString();
                pid = row["proID"].ToString();
                qty = row["qty"].ToString();
                cost = row["price"].ToString();
                amt = row["amount"].ToString();
                cost = row["cost"].ToString();

                // 0 For Serial and Id
                guna2DataGridView1.Rows.Add(did, pid, pname, qty, cost, amt, cost);
            }
        }

        private void LoadForEditWithoutCustomer()
        {
            string qry = @"SELECT d.detailID, d.productID AS proID, p.pName, d.qty, d.price, d.amount, d.cost
                   FROM tblDetails d
                   INNER JOIN Product p ON d.productID = p.proID
                   WHERE d.dMainID = " + id;

            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                string did;
                string pid;
                string pname;
                string qty;
                string cost;
                string amt;

                did = row["detailID"].ToString();
                pname = row["pName"].ToString();
                pid = row["proID"].ToString();
                qty = row["qty"].ToString();
                cost = row["price"].ToString();
                amt = row["amount"].ToString();
                cost = row["cost"].ToString();

                // Add data to DataGridView
                guna2DataGridView1.Rows.Add(did, pid, pname, qty, cost, amt, cost);
            }
        }



        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not in the header row
            if (e.RowIndex >= 0)
            {
                if (guna2DataGridView1.Columns.Contains("colAdd") && guna2DataGridView1.Columns.Contains("colReduce"))
                {
                    DataGridViewColumn colAdd = guna2DataGridView1.Columns["colAdd"];
                    DataGridViewColumn colReduce = guna2DataGridView1.Columns["colReduce"];

                    // Delete
                    if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
                    {
                        // Confirm Before delete
                        guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                        guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

                        if (guna2MessageDialog1.Show("Are you sure you want to delete") == DialogResult.Yes)
                        {
                            int rowindex = guna2DataGridView1.CurrentCell.RowIndex;
                            DataGridViewRow selectedRow = guna2DataGridView1.Rows[rowindex];

                            int id = Convert.ToInt32(selectedRow.Cells["dgvid"].Value);

                            guna2DataGridView1.Rows.RemoveAt(rowindex);

                            string qry = "DELETE FROM tblMain WHERE MainID = " + id + "";
                            string qry2 = "DELETE FROM tblDetails WHERE dMainID = " + id + "";
                            Hashtable ht = new Hashtable();
                            MainClass.SQl(qry, ht);

                            if (MainClass.SQl(qry2, ht) > 0)
                            {
                                // Success message if needed
                            }

                            GrandTotal();
                        }
                    }
                    else if (guna2DataGridView1.CurrentCell.OwningColumn == colAdd)
                    {
                        // Increment quantity
                        int currentQuantity = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvqty"].Value);
                        guna2DataGridView1.Rows[e.RowIndex].Cells["dgvqty"].Value = currentQuantity + 1;

                        // Recalculate amount and update GrandTotal
                        guna2DataGridView1.Rows[e.RowIndex].Cells["dgvAmount"].Value =
                            Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvqty"].Value) *
                            Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvPrice"].Value);

                        GrandTotal();
                    }
                    else if (guna2DataGridView1.CurrentCell.OwningColumn == colReduce)
                    {
                        // Decrement quantity (ensure it doesn't go below zero)
                        int currentQuantity = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvqty"].Value);
                        if (currentQuantity > 0)
                        {
                            guna2DataGridView1.Rows[e.RowIndex].Cells["dgvqty"].Value = currentQuantity - 1;

                            // Recalculate amount and update GrandTotal
                            guna2DataGridView1.Rows[e.RowIndex].Cells["dgvAmount"].Value =
                                Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvqty"].Value) *
                                Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvPrice"].Value);

                            GrandTotal();
                        }
                    }
                }
            }
        }


    }
}
