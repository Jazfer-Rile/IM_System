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
using IM_System.Reports;

namespace IM_System.View
{
    public partial class frmReports : Form
    {
        public frmReports()
        {
            InitializeComponent();
            // Set DateTimePickers to today's date on form load
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Today;
        }

        private  DataTable dTable(string qry)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand(qry,MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);


            return dt;
        }
        private void btnProductList_Click(object sender, EventArgs e)
        {
            string qry = @"Select * from Product p inner join Category c on p.pCatID = c.catID";
            DataTable dt = dTable(qry);

            crystalReportViewer1.ReportSource = null;
            rptProducts cr = new rptProducts();
            cr.SetDataSource(dt);
            crystalReportViewer1.ReportSource = cr;
            crystalReportViewer1.Refresh();
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            string qry = @"SELECT proID, pName,
                        ISNULL((SELECT SUM(qty) FROM tblDetails d INNER JOIN tblMain m ON m.MainID = d.dMainID WHERE m.mType = 'PUR' AND d.productID = proID), 0) -
                        ISNULL((SELECT SUM(qty) FROM tblDetails d INNER JOIN tblMain m ON m.MainID = d.dMainID WHERE m.mType = 'SAL' AND d.productID = proID), 0) AS StockBalance
                   FROM Product";

            crystalReportViewer1.ReportSource = null;
            rptStock cr = new rptStock();
            cr.SetDataSource(dTable(qry));
            crystalReportViewer1.ReportSource = cr;
            crystalReportViewer1.Refresh();
        }

        private void btnCriticalItem_Click(object sender, EventArgs e)
        {
            string qry = @"SELECT 
                            [pName],
                            [Stock],
                            [reorder]
                        FROM 
                            [IMS].[dbo].[vwCriticalItems]
                        WHERE 
                            [Stock] > 0
                        ";

            crystalReportViewer1.ReportSource = null;
            rptCriticalItem cr = new rptCriticalItem();
            cr.SetDataSource(dTable(qry));
            crystalReportViewer1.ReportSource = cr;
            crystalReportViewer1.Refresh();
        }

        private void btnOutofStock_Click(object sender, EventArgs e)
        {
            string qry = @"SELECT 
                          [proID],
                          [pName],
                          [pPrice],
                          [pCost],
                          [pImage],
                          [reorder],
                          [Stock]
                    FROM 
                          [IMS].[dbo].[vwCriticalItems]
                    WHERE 
                          [Stock] = 0;

                        ";

            crystalReportViewer1.ReportSource = null;
            rptOutofStocks cr = new rptOutofStocks();
            cr.SetDataSource(dTable(qry));
            crystalReportViewer1.ReportSource = cr;
            crystalReportViewer1.Refresh();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            // Get the selected start date from the DateTimePickers
            DateTime startDate = dtpStartDate.Value.Date;

            // Set the end date to the end of the selected day (23:59:59)
            DateTime endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1);

            // Adjust the query to filter sales records by date range
            string qry = @"SELECT m.MainID, m.mDate, m.mSupCusID, c.cusName, 
                   COALESCE(SUM(d.qty), 0) AS totalQuantity, 
                   COALESCE(SUM(d.amount), 0) AS totalAmount
                   FROM tblMain m
                   LEFT JOIN tblDetails d ON d.dMainID = m.MainID
                   LEFT JOIN Customer c ON c.cusID = m.mSupCusID
                   WHERE m.mType = 'SAL' 
                   AND m.mDate BETWEEN @startDate AND @endDate
                   GROUP BY m.MainID, m.mDate, m.mSupCusID, c.cusName
                   ORDER BY m.mDate DESC";

            // Set the report source to null before loading the new data
            crystalReportViewer1.ReportSource = null;

            // Create a new instance of the report
            rptSales cr = new rptSales();

            // Create a DataTable with the filtered data
            DataTable salesData = new DataTable();
            using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            {
                // Add the parameters to the SQL query
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);

                // Use SqlDataAdapter to fill the DataTable
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(salesData);
            }

            // Set the data source for the report
            cr.SetDataSource(salesData);

            // Display the report in the CrystalReportViewer
            crystalReportViewer1.ReportSource = cr;
            crystalReportViewer1.Refresh();
        }


    }
}
