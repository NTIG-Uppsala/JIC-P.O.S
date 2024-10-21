using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PointOfSaleSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateTotalPrice(float productPrice)
        {
            string priceText = TotalPrice.Text.Replace("kr", "").Trim();
            float price = float.Parse(priceText);

            price += productPrice;
            TotalPrice.Text = price.ToString("0.00") + " kr";
        }

        private void ResetTotalPrice()
        {
            TotalPrice.Text = "0 kr";
        }

        private void CoffeeButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());
            CalculateTotalPrice(25.99f);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetTotalPrice();
        }
    }
}