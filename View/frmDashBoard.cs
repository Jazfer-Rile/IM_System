using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace IM_System.View
{
    public partial class frmDashBoard : Form
    {
        public frmDashBoard()
        {
            InitializeComponent();
        } 
        private void frmDashBoard_Load(object sender, EventArgs e)
        {
        
            lblTotalProduct.Text = GetTotalProductCount().ToString("#,##0");
            lblStockOnHand.Text = GetTotalStockOnHand().ToString("#,##0");

        }
        private int GetTotalProductCount()
        {
            try
            {
                // Open a connection using the connection string from MainClass
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();

                    // Create a command to execute the SQL query
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Product", con))
                    {
                        // Execute the query and return the result
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0; // Return 0 in case of an error
            }

        }
        private int GetTotalStockOnHand()
        {
            try
            {
                int totalStock = 0;

                // Open a connection using the connection string from MainClass
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();

                    // Create a command to execute the SQL query
                    using (SqlCommand cmd = new SqlCommand("SELECT p.proID, p.pName, p.pPrice, p.pCost, p.PImage, " +
                                                          "(ISNULL((SELECT SUM(d.qty) FROM tblDetails d " +
                                                          "INNER JOIN tblMain m ON m.MainID = d.dMainID " +
                                                          "WHERE m.mType = 'PUR' AND d.productID = p.proID), 0) - " +
                                                          "ISNULL((SELECT SUM(d.qty) FROM tblDetails d " +
                                                          "INNER JOIN tblMain m ON m.MainID = d.dMainID " +
                                                          "WHERE m.mType = 'SAL' AND d.productID = p.proID), 0)) AS Stock " +
                                                          "FROM Product p", con))
                    {
                        // Execute the query and read the result set
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Iterate through the result set and calculate the total stock
                            while (reader.Read())
                            {
                                int stock = Convert.ToInt32(reader["Stock"]);
                                totalStock += stock;
                            }
                        }
                    }
                }

                return totalStock;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0; // Return 0 in case of an error
            }
        }



    }

}
