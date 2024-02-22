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



namespace IM_System.Model
{
    public partial class frmUserAdd : Form
    {
        public frmUserAdd()
        {
            InitializeComponent();
            txtPhone.KeyPress += new KeyPressEventHandler(txtPhone_KeyPress);

        }

        public virtual void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public int id = 0;
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
                    qry = @"Insert into Users values(@name,@username,@pass,@phone,@role,@image)";
                }
                else //update
                {
                    qry = @"UPDATE Users SET uName = @name,
                          Uusername = @username,
                          uPass = @pass,
                          uPhone = @phone,
                          uRole = @role,
                          uImage = @image
                          WHERE userID = @id";


                }
                Image temp = new Bitmap(txtPic.Image);
                MemoryStream ms = new MemoryStream();
                temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageByteArray = ms.ToArray();

                Hashtable ht = new Hashtable();
                ht.Add("@id", id);
                ht.Add("@name", txtName.Text);
                ht.Add("@username", txtUser.Text);
                ht.Add("@pass", txtPass.Text);
                ht.Add("@phone", txtPhone.Text);
                ht.Add("@role", cmbRole.Text);
                ht.Add("@image", imageByteArray);

                if (MainClass.SQl(qry, ht) > 0)
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Show("Data Save succesfully");
                    id = 0;
                    txtName.Text = "";
                    txtUser.Text = "";
                    txtPass.Text = "";
                    txtPhone.Text = "";
                    cmbRole.Text = "";
                    txtPic.Image = IM_System.Properties.Resources.userPic;
                    txtName.Focus();


                }


            }
        }

        public string filePath = "";
        Byte[] imageByteArray;
        private void btnBrowse_Click(object sender, EventArgs e)
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
            string qry = @"Select uImage from Users where userID = " + id + "";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                Byte[] imageArray = (byte[])dt.Rows[0]["uImage"];
                byte[] imageByteArry = imageArray;
                txtPic.Image = Image.FromStream(new MemoryStream(imageArray));
            }
        }

        private void frmUserAdd_Load(object sender, EventArgs e)
        {
            if (id > 0)
            {
                LoadImage();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (txtPass.PasswordChar == '●')
            {
                guna2Button3.BringToFront();
                txtPass.PasswordChar = '\0';
            }
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (txtPass.PasswordChar == '\0')
            {
                guna2Button2.BringToFront();
                txtPass.PasswordChar = '●';
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {

            // Check if the pressed key is a digit or the backspace key
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                // If not a digit or backspace, suppress the keypress
                e.Handled = true;
            }
        }
    }
}
