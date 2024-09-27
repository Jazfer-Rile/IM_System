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

namespace IM_System
{
    public partial class frmProductAdd : Form
    {
       
        public frmProductAdd()
        {
            InitializeComponent();
            txtBarcode.KeyPress += new KeyPressEventHandler(guna2barcodetextbox_KeyPress);
            txtPrice.KeyPress += new KeyPressEventHandler(guna2pricetextbox_KeyPress);
            txtCost.KeyPress += new KeyPressEventHandler(guna2costtextbox_KeyPress);
        }
        public int id = 0;
        public int catID = 0;
        private void frmProductAdd_Load(object sender, EventArgs e)
        {
            string qry = "Select catID 'id' , catName 'name' from Category";
            MainClass.CBFill(qry, cbCategory);
            if (id > 0) 
            {
                cbCategory.SelectedValue = catID;
                LoadImage();
            }
           
        }


        public void guna2barcodetextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the input is not a digit or control (like backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignore the character
            }
        }
        public void guna2pricetextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the input is not a digit or control (like backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignore the character
            }
        }
        public void guna2costtextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the input is not a digit or control (like backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignore the character
            }
        }


        public virtual void btnSave_Click(object sender, EventArgs e)
        {
            if (MainClass.Validation(this) == false)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("Please remove errors");
                return;
            }
            else
            {
                // Check for product duplication
                if (IsProductDuplicate(txtName.Text.Trim(), id))
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                    guna2MessageDialog1.Show("Product name already exists");
                    return;
                }
                // Check for barcode duplication
                if (IsBarcodeDuplicate(txtBarcode.Text.Trim(), id))
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                    guna2MessageDialog1.Show("Barcode already exists");
                    return;
                }

                // Continue with saving data if no duplication
                string qry = "";
                if (id == 0)//Insert
                {

                    qry = @"INSERT INTO Product (pName, pCatID, pBarcode, pCost, pPrice, reorder, pImage) VALUES (@name, @pCatID, @barcode, @cost, @saleprice, @reorder, @image)";
                }
                else //update
                {
                    qry = @"UPDATE Product SET pName = @name,
                      pCatID = @pCatID,
                      pBarcode = @barcode,
                      pCost = @cost,
                      pPrice = @saleprice,
                      reorder = @reorder,
                      pImage = @image
                      WHERE proID = @id";
                }
                Image temp = new Bitmap(txtPic.Image);
                MemoryStream ms = new MemoryStream();
                temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageByteArray = ms.ToArray();

                Hashtable ht = new Hashtable();
                ht.Add("@id", id);
                ht.Add("@name", txtName.Text);
                ht.Add("@pCatID", Convert.ToInt32(cbCategory.SelectedValue));
                ht.Add("@barcode", txtBarcode.Text);
                ht.Add("@cost", Convert.ToDouble(txtCost.Text));
                ht.Add("@saleprice", Convert.ToInt32(txtPrice.Text));
                ht.Add("@reorder", UDReOrder.Value);
                ht.Add("@image", imageByteArray);

                if (MainClass.SQl(qry, ht) > 0)
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Show("Data Save succesfully");
                    id = 0;
                    txtName.Text = "";
                    txtBarcode.Text = "";
                    cbCategory.SelectedIndex = 0;
                    cbCategory.SelectedIndex = -1;
                    txtCost.Text = "";
                    txtPrice.Text = "";
                    UDReOrder.Value = 0;
                    txtPic.Image = IM_System.Properties.Resources.itemPic;
                    txtName.Focus();


                }


            }
        }
        // Method to check if the product name already exists in the database
        private bool IsProductDuplicate(string productName, int productId)
        {
            string qry = "SELECT COUNT(*) FROM Product WHERE pName = @productName AND proID != @productId";
            int count = 0;

            using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@productName", productName);
                cmd.Parameters.AddWithValue("@productId", productId);

                MainClass.con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
                MainClass.con.Close();
            }

            return count > 0;
        }
        // Method to check if the barcode already exists in the database
        private bool IsBarcodeDuplicate(string barcode, int productId)
        {
            string qry = "SELECT COUNT(*) FROM Product WHERE pBarcode = @barcode AND proID != @productId";
            int count = 0;

            using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@barcode", barcode);
                cmd.Parameters.AddWithValue("@productId", productId);

                MainClass.con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
                MainClass.con.Close();
            }

            return count > 0;
        }

        public string filePath = "";
        Byte[] imageByteArray;

        public virtual void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public virtual void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images(.jpg, .png)|*.png; *jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                txtPic.Image = new Bitmap(filePath);
            }
        }
        private void LoadImage()
        {
            string qry = @"Select pImage from Product where proID = " + id + "";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                Byte[] imageArray = (byte[])dt.Rows[0]["pImage"];
                byte[] imageByteArry = imageArray;
                txtPic.Image = Image.FromStream(new MemoryStream(imageArray));
            }
        }

        
    }
}
