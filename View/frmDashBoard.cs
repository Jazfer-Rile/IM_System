using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guna.Charts.WinForms;
using System.Windows.Forms;
using System.Data.SqlClient;



namespace IM_System.View
{
    public partial class frmDashBoard : Form
    {
        public frmDashBoard()
        {
            InitializeComponent();
            InitializeChart();
        }
        private void InitializeChart()
        {
            // Clear any existing datasets
            gunaChart1.Datasets.Clear();

            // Create a new line dataset
            GunaLineDataset dataset = new GunaLineDataset
            {
                Label = "Sales",
            };

            // Add the dataset to the chart
            gunaChart1.Datasets.Add(dataset);
        }

        private void frmDashBoard_Load(object sender, EventArgs e)
        {
            lblDailySales.Text = GetDailySalesDetails().ToString("#,##0");
            lblTotalProduct.Text = GetTotalProductCount().ToString("#,##0");
            lblStockOnHand.Text = GetTotalStockOnHand().ToString("#,##0");
            lblCriticalItems.Text = GetTotalCriticalItemsCount().ToString("#,##0");
            lblOutOfStockItems.Text = GetTotalOutOfStockItemsCount().ToString("#,##0");

            // Set up report sales combo box
            cmbReportSales.Items.Add("Daily");
            cmbReportSales.Items.Add("Weekly");
            cmbReportSales.Items.Add("Monthly");
            cmbReportSales.Items.Add("Yearly");

            // Set default selection
            cmbReportSales.SelectedIndex = 2;

            // Load the sales report based on the selected period
            cmbReportSales.SelectedIndexChanged += CmbReportSales_SelectedIndexChanged;
            LoadSalesData("Monthly"); // Load weekly data by default
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

        private void LoadSalesData(string period)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MainClass.con_string))
                {
                    con.Open();
                    string query = "";

                    // Adjust query based on selected period
                    if (period == "Daily")
                    {
                        query = @"
                        SELECT DAY(mDate) AS DayNumber, SUM(totalAmount) AS TotalSales
                        FROM vwSales
                        WHERE MONTH(mDate) = MONTH(GETDATE()) AND YEAR(mDate) = YEAR(GETDATE())
                        GROUP BY DAY(mDate)";
                    }
                    else if (period == "Weekly")
                    {
                        query = @"SELECT 
                            DATEPART(WEEK, mDate) - DATEPART(WEEK, DATEADD(MONTH, DATEDIFF(MONTH, 0, mDate), 0)) + 1 AS WeekOfMonth, 
                            SUM(totalAmount) AS TotalSales
                        FROM 
                            vwSales
                        WHERE 
                            MONTH(mDate) = MONTH(GETDATE()) 
                            AND YEAR(mDate) = YEAR(GETDATE())
                        GROUP BY 
                            DATEPART(WEEK, mDate) - DATEPART(WEEK, DATEADD(MONTH, DATEDIFF(MONTH, 0, mDate), 0)) + 1
                        ORDER BY 
                            WeekOfMonth;
                        ";
                    }
                    else if (period == "Monthly")
                    {
                        query = @"
                       SELECT DATEPART(MONTH, mDate) AS MonthNumber, SUM(totalAmount) AS TotalSales
                        FROM vwSales
                        WHERE YEAR(mDate) = YEAR(GETDATE())
                        GROUP BY DATEPART(MONTH, mDate)
                        ";
                    }
                    else if (period == "Yearly")
                    {
                        query = @"
                        SELECT DATEPART(YEAR, mDate) AS YearNumber, SUM(totalAmount) AS TotalSales
                        FROM vwSales
                        GROUP BY DATEPART(YEAR, mDate)";
                    }

                    // Execute the query and populate the chart
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            LoadChartData(reader, period);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                guna2MessageDialog1.Show("An error occurred: " + ex.Message);
            }
        }
        public class DataPoint
        {
            public string Label { get; set; }
            public double Value { get; set; }

            public DataPoint(string label, double value)
            {
                Label = label;
                Value = value;
            }
        }


        private void LoadChartData(SqlDataReader reader, string period)
        {
            // Clear existing data points
            GunaLineDataset cSales = (GunaLineDataset)gunaChart1.Datasets[0]; // Assuming the first dataset is the line dataset
            cSales.DataPoints.Clear(); // Clear existing data points

            while (reader.Read())
            {
                string label = "";
                double totalSales = Convert.ToDouble(reader["TotalSales"]);

                // Customize labels based on the period
                if (period == "Daily")
                {
                    label = "Day " + reader["DayNumber"].ToString();
                }
                else if (period == "Weekly")
                {
                    label = "Week " + reader["WeekOfMonth"].ToString(); 
                }

                else if (period == "Monthly")
                {
                    // Convert month number to month name
                    int monthNumber = Convert.ToInt32(reader["MonthNumber"]);
                    label = new DateTime(1, monthNumber, 1).ToString("MMMM"); // Get the full month name
                }
                else if (period == "Yearly")
                {
                    label = "Year " + reader["YearNumber"].ToString();
                }

                // Add data point to the chart dataset using LPoint
                cSales.DataPoints.Add(new Guna.Charts.WinForms.LPoint(label, totalSales));
            }

            // Redraw the chart to reflect changes
            gunaChart1.Update();
        }



        private void CmbReportSales_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPeriod = cmbReportSales.SelectedItem.ToString();
            LoadSalesData(selectedPeriod);
        }

    }

}
