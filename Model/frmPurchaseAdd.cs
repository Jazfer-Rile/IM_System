using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_System.Model
{
    public partial class frmPurchaseAdd : Form
    {
        public frmPurchaseAdd()
        {
            InitializeComponent();
        }
        public int mainID = 0;
        public int supID = 0;
        private void frmPurchaseAdd_Load(object sender, EventArgs e)
        {
            cbProduct.SelectedIndexChanged -= new EventHandler(cbProduct_SelectedIndexChanged);
            string qry = "Select proID 'id' , pName 'name' from Product";
            string qry2 = "Select supID 'id' , supName 'name' from Supplier";

            MainClass.CBFill(qry, cbProduct);
            MainClass.CBFill(qry2, cbSupplier);

            if (supID > 0)
            {
                cbSupplier.SelectedValue = supID;
                LoadForEdit();
            }

            cbProduct.SelectedIndexChanged += new EventHandler(cbProduct_SelectedIndexChanged);
        }
        private void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProduct.SelectedIndex != -1)
            {
                txtQty.Text = "";
                GetDetails();
            }
        }

        private void GetDetails()
        {
            string qry = "Select * from Product where proID = " + Convert.ToInt32(cbProduct.SelectedValue) + "";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtCost.Text = dt.Rows[0]["pCost"].ToString();
                Calculate();
            }
        }

        private void Calculate()
        {
            double qty = 0;
            double cost = 0;
            double amt = 0;

            double.TryParse(txtQty.Text, out qty);
            double.TryParse(txtCost.Text, out cost);

            amt = qty * cost;
            txtAmount.Text = amt.ToString();

        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string qry = "Select * from Product where pBarcode like '" + txtBarcode.Text + "'";
                SqlCommand cmd = new SqlCommand(qry, MainClass.con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cbProduct.SelectedValue = Convert.ToInt32(dt.Rows[0]["proID"].ToString());
                    Calculate();
                    txtBarcode.Text = "";
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string pid;
            string pname;
            string qty;
            string cost;
            string amt;

            pname = cbProduct.Text;
            pid = cbProduct.SelectedValue.ToString();
            qty = txtQty.Text;
            cost = txtCost.Text;
            amt = txtAmount.Text;
            // 0 For Serial and Id
            guna2DataGridView1.Rows.Add(0, 0, pid, pname, qty, cost, amt);
            cbProduct.SelectedIndex = 0;
            cbProduct.SelectedIndex = -1;
            txtQty.Text = "";
            txtCost.Text = "";
            txtAmount.Text = "";

        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int count = 0;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        public virtual void btnSave_Click(object sender, EventArgs e)
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

            if (mainID == 0) // insert
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
            cmd1.Parameters.AddWithValue("@id", mainID);
            cmd1.Parameters.AddWithValue("@date", Convert.ToDateTime(txtDate.Value).Date);
            cmd1.Parameters.AddWithValue("@type", "PUR");
            cmd1.Parameters.AddWithValue("@supID", Convert.ToInt32(cbSupplier.SelectedValue));
            if (MainClass.con.State == ConnectionState.Closed)
            {
                MainClass.con.Open();
            }

            if (mainID == 0)
            {
                mainID = Convert.ToInt32(cmd1.ExecuteScalar());
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
                cmd2.Parameters.AddWithValue("@mID", mainID);
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

                mainID = 0;
                supID = 0;
                txtDate.Value = DateTime.Now;
                cbSupplier.SelectedIndex = 0;
                cbSupplier.SelectedIndex = -1; 
                guna2DataGridView1.Rows.Clear();
            }


        }
        private void LoadForEdit()
        {
            string qry = "Select * from tblDetails inner join product on proID= productID where dMainID = " + mainID + " ";
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
                pid = row["productID"].ToString();
                qty = row["qty"].ToString();
                cost = row["price"].ToString();
                amt = row["amount"].ToString();

                // 0 For Serial and Id
                guna2DataGridView1.Rows.Add(0, did, pid, pname, qty, cost, amt);
            }
        }
    }
 }
