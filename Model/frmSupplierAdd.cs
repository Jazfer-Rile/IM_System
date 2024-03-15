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
using System.Data.SqlClient;

namespace IM_System.Model
{
    public partial class frmSupplierAdd : Form
    {
        public frmSupplierAdd()
        {
            InitializeComponent();
            txtPhone.KeyPress += new KeyPressEventHandler(txtPhone_KeyPress);
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
                // Check for duplicate supplier name
                if (IsSupplierNameDuplicate(txtName.Text.Trim(), id))
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                    guna2MessageDialog1.Show("Supplier name already exists");
                    return;
                }

                string qry = "";
                if (id == 0)//Insert
                {
                    qry = @"Insert into Supplier values(@name,@phone,@Email,@Address)";
                }
                else //update
                {
                    qry = @"UPDATE Supplier SET 
                        supName = @name,
                        supPhone = @phone,
                        supEmail = @email,
                        supAddress = @Address
                        WHERE supID = @id";



                }

                Hashtable ht = new Hashtable();
                ht.Add("@id", id);
                ht.Add("@name", txtName.Text);
                ht.Add("@phone", txtPhone.Text);
                ht.Add("@email", txtEmail.Text);
                ht.Add("@Address", txtAddress.Text);


                if (MainClass.SQl(qry, ht) > 0)
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Show("Data Save succesfully");
                    id = 0;
                    txtName.Text = "";
                    txtPhone.Text = "";
                    txtEmail.Text = "";
                    txtAddress.Text = "";
                    txtName.Focus();


                }


            }
        }

        // Method to check if the supplier name already exists in the database
        private bool IsSupplierNameDuplicate(string supplierName, int supplierId)
        {
            string qry = "SELECT COUNT(*) FROM Supplier WHERE supName = @supplierName AND supID != @supplierId";
            int count = 0;

            using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@supplierName", supplierName);
                cmd.Parameters.AddWithValue("@supplierId", supplierId);

                MainClass.con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
                MainClass.con.Close();
            }

            return count > 0;
        }
        public virtual void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
