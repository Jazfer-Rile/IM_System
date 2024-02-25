namespace IM_System.Model
{
    partial class ucProduct
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblPName = new System.Windows.Forms.Label();
            this.txtPic = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2ShadowPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPic)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2ShadowPanel1
            // 
            this.guna2ShadowPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel1.Controls.Add(this.lblPrice);
            this.guna2ShadowPanel1.Controls.Add(this.lblPName);
            this.guna2ShadowPanel1.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel1.Location = new System.Drawing.Point(14, 53);
            this.guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            this.guna2ShadowPanel1.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel1.Size = new System.Drawing.Size(126, 140);
            this.guna2ShadowPanel1.TabIndex = 0;
            // 
            // lblPrice
            // 
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrice.Location = new System.Drawing.Point(8, 100);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(112, 40);
            this.lblPrice.TabIndex = 2;
            this.lblPrice.Text = "₱";
            this.lblPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPName
            // 
            this.lblPName.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPName.Location = new System.Drawing.Point(3, 48);
            this.lblPName.Name = "lblPName";
            this.lblPName.Size = new System.Drawing.Size(119, 52);
            this.lblPName.TabIndex = 1;
            this.lblPName.Text = "Product Name";
            this.lblPName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPic
            // 
            this.txtPic.BackColor = System.Drawing.Color.Transparent;
            this.txtPic.Image = global::IM_System.Properties.Resources.itemPic;
            this.txtPic.ImageRotate = 0F;
            this.txtPic.Location = new System.Drawing.Point(32, 8);
            this.txtPic.Name = "txtPic";
            this.txtPic.Size = new System.Drawing.Size(90, 90);
            this.txtPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.txtPic.TabIndex = 0;
            this.txtPic.TabStop = false;
            this.txtPic.UseTransparentBackground = true;
            this.txtPic.Click += new System.EventHandler(this.txtPic_Click);
            // 
            // ucProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.txtPic);
            this.Controls.Add(this.guna2ShadowPanel1);
            this.Name = "ucProduct";
            this.Size = new System.Drawing.Size(153, 198);
            this.guna2ShadowPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2PictureBox txtPic;
        private System.Windows.Forms.Label lblPName;
        private System.Windows.Forms.Label lblPrice;
    }
}
