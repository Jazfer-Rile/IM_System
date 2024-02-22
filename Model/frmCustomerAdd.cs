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

namespace IM_System.Model
{
    public partial class frmCustomerAdd : Form
    {
        public frmCustomerAdd()
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
                    qry = @"Insert into Customer values(@name,@phone,@Email,@Address)";
                }
                else //update
                {
                    qry = @"UPDATE Customer SET 
                        cusName = @name,
                        cusPhone = @phone,
                        cusEmail = @email,
                        cusAddress = @Address
                        WHERE cusID = @id";



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
