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
                string qry = "";
                if (id == 0)//Insert
                {
                    qry = @"Insert into Product values(@name,@pCatID,@barcode,@cost,@saleprice,@image)";
                }
                else //update
                {
                    qry = @"UPDATE Product SET pName = @name,
                          pCatID = @pCatID,
                          pBarcode = @barcode,
                          pCost = @cost,
                          pPrice = @saleprice,
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
                    txtPic.Image = IM_System.Properties.Resources.userPic;
                    txtName.Focus();


                }


            }
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
