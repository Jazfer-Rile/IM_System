using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;


namespace IM_System
{
    public partial class frmLogin : Form
    {
        private int maxLoginAttempts = 3; // Maximum number of login attempts allowed
        private int remainingLoginAttempts; // Remaining login attempts
        private int[] cooldownDurations = { 10, 60, 180, 300 }; // Cooldown durations in seconds for each attempt
        private int currentCooldownIndex = 0; // Index to track the current cooldown duration
        private DateTime cooldownEndTime; // Time when cooldown ends
        public frmLogin()
        {
            InitializeComponent();
            remainingLoginAttempts = maxLoginAttempts;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (remainingLoginAttempts <= 0)
            {
                TimeSpan remainingCooldown = cooldownEndTime - DateTime.Now;
                ShowCooldownMessage($"Please wait {remainingCooldown.TotalSeconds:F0} seconds before trying again.", "Cooldown");
                return;
            }

            if (MainClass.IsValidUser(txtUser.Text, txtPass.Text))
            {
                this.Hide();
                frmMain frm = new frmMain();
                frm.Show();
            }
            else
            {
                remainingLoginAttempts--;

                if (remainingLoginAttempts > 0)
                {
                    ShowErrorMessage($"Invalid username or password. {remainingLoginAttempts} attempts remaining.");
                }
                else
                {
                    currentCooldownIndex++; // Increase cooldown index after every set of 3 failed attempts
                    if (currentCooldownIndex >= cooldownDurations.Length)
                    {
                        currentCooldownIndex = cooldownDurations.Length - 1; 
                    }
                    cooldownEndTime = DateTime.Now.AddSeconds(cooldownDurations[currentCooldownIndex]); // Update cooldown duration
                    ShowCooldownMessage($"Maximum login attempts reached. Please try again later. Cooldown: {cooldownDurations[currentCooldownIndex]} seconds", "Cooldown");

                    // Start cooldown countdown asynchronously
                    await Task.Delay(cooldownDurations[currentCooldownIndex] * 1000); 
                    remainingLoginAttempts = maxLoginAttempts;
                }
            }
        }



        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (txtPass.PasswordChar == '\0')
            {
                guna2Button2.BringToFront();
                txtPass.PasswordChar = '●';
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (txtPass.PasswordChar == '●')
            {
                guna2Button3.BringToFront();
                txtPass.PasswordChar = '\0';
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                // Close any open forms explicitly
                foreach (Form form in Application.OpenForms)
                {
                    form.Close();
                }

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowErrorMessage(string message)
        {
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
            guna2MessageDialog1.Show(message, "Error");
        }

        private void ShowCooldownMessage(string message, string caption)
        {
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
            guna2MessageDialog1.Show(message, caption);
        }
    }
}
