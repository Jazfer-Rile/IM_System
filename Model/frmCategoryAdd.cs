using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace IM_System.Model
{
    public partial class frmCategoryAdd : Form
    {
        public frmCategoryAdd()
        {
            InitializeComponent();
            txtName.KeyPress += txtName_KeyPress;
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
                // Check for duplicate category name
                if (IsCategoryDuplicate(txtName.Text.Trim(), id))
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                    guna2MessageDialog1.Show("Category name already exists");
                    return;
                }

                string qry = "";
                if (id == 0)//Insert
                {
                    qry = @"Insert into Category values(@name)";
                }
                else //update
                {
                    qry = @"UPDATE Category SET catName = @name
                              WHERE catID = @id";
                }


                Hashtable ht = new Hashtable();
                ht.Add("@id", id);
                ht.Add("@name", txtName.Text);

                if (MainClass.SQl(qry, ht) > 0)
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Show("Data Save succesfully");
                    id = 0;
                    txtName.Text = "";
                    txtName.Focus();


                }


            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Convert the pressed key to uppercase
            e.KeyChar = char.ToUpper(e.KeyChar);
        }
        // Method to check if the category name already exists in the database
        private bool IsCategoryDuplicate(string categoryName, int categoryId)
        {
            string qry = "SELECT COUNT(*) FROM Category WHERE catName = @categoryName AND catID != @categoryId";
            int count = 0;

            using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@categoryName", categoryName);
                cmd.Parameters.AddWithValue("@categoryId", categoryId);

                MainClass.con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
                MainClass.con.Close();
            }

            return count > 0;
        }
    }
}
