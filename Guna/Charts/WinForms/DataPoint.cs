namespace Guna.Charts.WinForms
{
    internal class DataPoint
    {
        private string label;
        private double totalSales;

        public DataPoint(string label, double totalSales)
        {
            this.label = label;
            this.totalSales = totalSales;
        }
    }
}