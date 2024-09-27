﻿using System;
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
            lblDailySales.Text = GetDailySalesDetails().ToString("#,##0");
            lblTotalProduct.Text = GetTotalProductCount().ToString("#,##0");
            lblStockOnHand.Text = GetTotalStockOnHand().ToString("#,##0");
            lblCriticalItems.Text = GetTotalCriticalItemsCount().ToString("#,##0");
            lblOutOfStockItems.Text = GetTotalOutOfStockItemsCount().ToString("#,##0");

        }
        private int GetDailySalesDetails()
        {
            try
            {
                // Open a connection using the connection string from MainClass
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();

                    // Create a SqlCommand to execute the SQL query for out-of-stock items
                    using (SqlCommand cmd = new SqlCommand(@"SELECT COALESCE(SUM(d.amount), 0) AS totalDailySales
               FROM tblMain m
               LEFT JOIN tblDetails d ON d.dMainID = m.MainID
               WHERE m.mType = 'SAL'
               AND CAST(m.mDate AS DATE) = CAST(GETDATE() AS DATE)", con))
                    {
                        // Execute the query and return the result
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error message using Guna2MessageDialog
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("An error occurred: " + ex.Message);
                return 0; // Return 0 in case of an error
            }
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
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("An error occurred: " + ex.Message);
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
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("An error occurred: " + ex.Message);
                return 0; // Return 0 in case of an error
            }

        }
        private int GetTotalCriticalItemsCount()
        {
            try
            {
                // Open a connection using the connection string from MainClass
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();

                    // Create a SqlCommand to execute the SQL query
                    using (SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) 
                                             FROM vwCriticalItems 
                                             WHERE Stock > 0", con))
                    {
                        // Execute the query and return the result
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error message using Guna2MessageDialog
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("An error occurred: " + ex.Message);
                return 0; // Return 0 in case of an error
            }
        }
        private int GetTotalOutOfStockItemsCount()
        {
            try
            {
                // Open a connection using the connection string from MainClass
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();

                    // Create a SqlCommand to execute the SQL query for out-of-stock items
                    using (SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) 
                                                     FROM vwCriticalItems 
                                                     WHERE Stock = 0", con))
                    {
                        // Execute the query and return the result
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error message using Guna2MessageDialog
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show("An error occurred: " + ex.Message);
                return 0; // Return 0 in case of an error
            }
        }


    }

}
