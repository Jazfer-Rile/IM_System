using System;
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
using System.Configuration;

namespace IM_System.View
{
    public partial class frmDatabaseView : Form
    {
        // Connection string
        //public static readonly string con_string = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //public static SqlConnection con = new SqlConnection(con_string);
        private AutoBackupService autoBackupService; // Create an instance of AutoBackupService

        public frmDatabaseView()
        {
            InitializeComponent();
            LoadBackupFiles();
            guna2MessageDialog1.Parent = frmMain.Instance;
            autoBackupService = new AutoBackupService(); // Initialize the backup service
        }

        public class AutoBackupService
        {
            private string backupFolderPath = @"C:\SQLServerBackups"; // Changed path

            public AutoBackupService()
            {
                EnsureBackupDirectoryExists();
            }

            // Ensure the backup directory exists
            private void EnsureBackupDirectoryExists()
            {
                if (!Directory.Exists(backupFolderPath))
                {
                    Directory.CreateDirectory(backupFolderPath);
                }
            }

            // Method to perform manual backup
            public void PerformManualBackup()
            {
                string backupFilePath = Path.Combine(backupFolderPath, $"IMS_ManualBackup_{DateTime.Now:yyyyMMdd_HHmmss}.bak");

                string backupQuery = $"BACKUP DATABASE [IMS] TO DISK = '{backupFilePath}' WITH FORMAT, MEDIANAME = 'SQLServerBackups', NAME = 'Manual Backup of IMS';";

                try
                {
                    using (SqlConnection connection = new SqlConnection(MainClass.con_string)) // Use a new connection
                    {
                        SqlCommand command = new SqlCommand(backupQuery, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine($"Manual backup completed successfully at {DateTime.Now}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during manual backup: {ex.Message}");
                }
            }
        }

        private void LoadBackupFiles()
        {
            // Path where the backups are stored
            string backupFolderPath = @"C:\SQLServerBackups"; // Changed path

            // Clear existing rows before loading new data
            guna2DataGridView1.Rows.Clear();

            if (Directory.Exists(backupFolderPath))
            {
                var backupFiles = Directory.GetFiles(backupFolderPath, "*.bak");
                foreach (var backupFile in backupFiles)
                {
                    FileInfo fileInfo = new FileInfo(backupFile);
                    string fileName = fileInfo.Name;

                    // Change date format to 12-hour format
                    string fileDate = fileInfo.CreationTime.ToString("yyyy-MM-dd hh:mm tt");

                    // Convert size to MB and format to one decimal place
                    string fileSize = (fileInfo.Length / (1024.0 * 1024.0)).ToString("F1") + " MB"; // Convert to MB

                    string fileType = fileName.Contains("ManualBackup") ? "Manual" : "Auto";

                    // Add data to the grid
                    guna2DataGridView1.Rows.Add(fileName, fileDate, fileType, fileSize);
                }
            }
        }


        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not in the header row
            if (e.RowIndex >= 0)
            {
                string backupFolderPath = @"C:\SQLServerBackups"; // Changed path
                string fileName = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvname"].Value.ToString();
                string filePath = Path.Combine(backupFolderPath, fileName);

                // Delete Backup
                if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
                {
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

                    if (guna2MessageDialog1.Show("Are you sure you want to delete this backup?") == DialogResult.Yes)
                    {
                        DeleteBackup(filePath);
                        LoadBackupFiles(); // Refresh the grid after deletion
                    }
                }
            }
        }

        private void DeleteBackup(string backupFilePath)
        {
            try
            {
                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Show("Backup file deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                guna2MessageDialog1.Show($"Error deleting backup: {ex.Message}");
            }
        }

        private void btnManualBackup_Click(object sender, EventArgs e)
        {
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;

            DialogResult result = guna2MessageDialog1.Show("Do you want to perform a manual backup?");
            // Perform manual backup when the button is clicked
            if (result == DialogResult.Yes)
            {
                // Call the manual backup service
                autoBackupService.PerformManualBackup();

                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                guna2MessageDialog1.Show("Manual backup completed successfully.");
                LoadBackupFiles();
            }
        }
    }
}
