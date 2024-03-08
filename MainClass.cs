using IM_System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Windows.Forms;
using System.Reflection;
using System.IO;


namespace IM_System
{
    class MainClass
    {
        public static readonly string con_string = "Data Source=RILEY\\SQLEXPRESS; Initial Catalog=IMS; Integrated Security=True; TrustServerCertificate=True;";
        public static SqlConnection con = new SqlConnection(con_string);


        public static bool IsValidUser(string user, string pass)
        {
            bool isValid = false;

            string qry = @"Select * from users where Uusername ='" + user + "' and uPass ='" + pass + "' ";
            SqlCommand cmd = new SqlCommand(qry, con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                isValid = true;
                USER = dt.Rows[0]["uName"].ToString();

                Byte[] imageArray = (byte[])dt.Rows[0]["uImage"];
                byte[] imageByteArry = imageArray;
                IMG = Image.FromStream(new MemoryStream(imageArray));
            }

            return isValid;
        }
        public static void StopBuffering(Panel ctr, bool doubleBuffer)
        {
            try
            {
                typeof(Control).InvokeMember("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, ctr, new object[] { doubleBuffer });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Create property for username
        private static string user;

        public static string USER
        {
            get { return user; }
            private set { user = value; }
        }
        // Property for User Image
        public static Image img;

        public static Image IMG
        {
            get { return img; }
            private set { img = value; }
        }

        // Method for CRUD Operation
        public static int SQl(string qry, Hashtable ht)
        {
            int res = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                foreach (DictionaryEntry item in ht)
                {
                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }

                if (con.State == ConnectionState.Closed) { con.Open(); }
                res = cmd.ExecuteNonQuery();
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                con.Close();
            }

            return res;
        }

        // for loading data from database
        public static void LoadData(string qry, DataGridView gv, ListBox lb)
        {
            // Serial No in Gridview
            gv.CellFormatting += new DataGridViewCellFormattingEventHandler(gv_CellFormatting);
            try
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < lb.Items.Count; i++)
                {
                    string colNam1 = ((DataGridViewColumn)lb.Items[i]).Name;
                    gv.Columns[colNam1].DataPropertyName = dt.Columns[i].ToString();
                }

                gv.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                con.Close();
            }
        }

        private static void gv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Guna.UI2.WinForms.Guna2DataGridView gv = (Guna.UI2.WinForms.Guna2DataGridView)sender;
            int count = 0;

            foreach (DataGridViewRow row in gv.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        //For Blur Screen 
        public static void BlurBackground(Form Model)
        {
            Form background = new Form();
            using (Model)
            {
                Form Background = new Form();
                using (Model)
                {
                    Background.StartPosition = FormStartPosition.Manual;
                    Background.FormBorderStyle = FormBorderStyle.None;
                    Background.Opacity = 0.5d;
                    Background.BackColor = Color.Black;
                    Background.Size = frmMain.Instance.Size;
                    Background.Location = frmMain.Instance.Location;
                    Background.ShowInTaskbar = false;
                    Background.Show();
                    Model.Owner = Background;
                    Model.ShowDialog(Background);
                    Background.Dispose();

                }
            }
        }

        // for ComboBox fill
        public static void CBFill(string qry, ComboBox cb)
        {
            SqlCommand cmd = new SqlCommand(qry, con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cb.DisplayMember = "name";
            cb.ValueMember = "id";
            cb.DataSource = dt;
            cb.SelectedIndex = -1;
        }
        public static bool Validation(Form F)
        {
            bool isValid = true;  

            foreach (Control c in F.Controls)
            {
                if (Convert.ToString(c.Tag) != "" && Convert.ToString(c.Tag) != null)
                {
                    // For Text Box
                    if (c is Guna.UI2.WinForms.Guna2TextBox)
                    {
                        Guna.UI2.WinForms.Guna2TextBox t = (Guna.UI2.WinForms.Guna2TextBox)c;
                        if (t.Text.Trim() == "")
                        {
                            t.BorderColor = Color.Red;
                            t.FocusedState.BorderColor = Color.Red;
                            t.HoverState.BorderColor = Color.Red;
                            isValid = false;  // Set form validation to false if any textbox is empty
                        }
                        else
                        {
                            t.BorderColor = Color.FromArgb(213, 218, 223);
                            t.FocusedState.BorderColor = Color.FromArgb(0, 122, 204);
                            t.HoverState.BorderColor = Color.FromArgb(0, 122, 204);
                        }
                    }
                    // For ComboBox
                    else if (c is Guna.UI2.WinForms.Guna2ComboBox)
                    {
                        Guna.UI2.WinForms.Guna2ComboBox cb = (Guna.UI2.WinForms.Guna2ComboBox)c;
                        if (cb.Text.Trim() == "")
                        {
                            cb.BorderColor = Color.Red;
                            cb.FocusedState.BorderColor = Color.Red;
                            cb.HoverState.BorderColor = Color.Red;
                            isValid = false;  // Set form validation to false if the combo box is empty
                        }
                        else
                        {
                            cb.BorderColor = Color.FromArgb(213, 218, 223);
                            cb.FocusedState.BorderColor = Color.FromArgb(0, 122, 204);
                            cb.HoverState.BorderColor = Color.FromArgb(0, 122, 204);
                        }
                    }
                }
            }

            return isValid;
        }

    }
}
