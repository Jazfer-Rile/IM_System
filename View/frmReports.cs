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

        
    }
}
