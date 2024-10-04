using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_System.Model
{
    public partial class ucProduct : UserControl
    {
        public EventHandler onSelect = null;
        public ucProduct()
        {
            InitializeComponent();
           
        }

        private void txtPic_Click(object sender, EventArgs e)
        {
            onSelect?.Invoke(this, e);
        }
       

        public int id { get; set; }
        public string PCost { get; set; }
        public string PName
        {
            get { return lblPName.Text;}
            set { lblPName.Text = value;}
        }
        public string Price
        {
            get { return lblPrice.Text; }
            set { lblPrice.Text = value; }
        }
        public Image PImage
        {
            get { return txtPic.Image; }
            set { txtPic.Image = value; }
        }
        
        private int stock;
        public int Stock
        {
            get { return stock; }
            set
            {
                stock = value;

                // Check if the stock is zero and update the label accordingly
                if (stock == 0)
                {
                    lblStock.Text = "Out of Stock"; // Change text to "Out of Stock"
                }
                else
                {
                    lblStock.Text = stock.ToString(); // Display the stock number
                }

                UpdateStockColor(); // Call method to update color based on stock
            }
        }
        private int reorder;
        public int Reorder
        {
            get { return reorder; }
            set
            {
                reorder = value;
                lblReorder.Text = reorder.ToString();
                UpdateStockColor(); // Call method to update color based on reorder level
            }
        }
        private void UpdateStockColor()
        {
            if (stock == 0)
            {
                // Out of stock - Red color
                lblStock.ForeColor = Color.Red;
            }
            else if (stock < reorder)
            {
                // Critical stock (less than or equal to reorder level) - Yellow color
                lblStock.ForeColor = Color.Orange;
            }
            else
            {
                // Sufficient stock - Green color
                lblStock.ForeColor = ColorTranslator.FromHtml("#00e80c");
            }
        }

    }
}
