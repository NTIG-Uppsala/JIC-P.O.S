using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PointOfSaleSystem.UserControls;

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
            CreateProducts();
        }

        // Struct representing a product to create buttons
        public struct Product
        {
            public Product(string productName, string productNameId, int productPrice)
            {
                name = productName;
                nameId = productNameId;
                price = productPrice;
            }

            public string name { get; init; }
            public string nameId { get; init; }
            public int price { get; init; }
        }

        // Create a list contaning products that are created with the Product struct
        public static List<Product> listOfProducts = new List<Product>
        {
            new Product("Coffee", "CoffeeButton", 25), // product name, nameId, price
            new Product("Sushi 12 pieces", "Sushi12Button", 150),
            new Product("Pasta carbonara", "PastaCarbonaraButton", 170),
            new Product("Chicken nuggets 9 pieces", "ChickenNuggets9Button", 70),
            new Product("Meatballs with mashed potatoes", "MeatballsMashedPotatoesButton", 100)
        };

        private void CreateProducts()
        {
            foreach (var product in listOfProducts)
            {
                // Create a button for each product
                Button button = new Button
                {
                    Name = product.nameId, // Used for x:Name value to create an id for the item
                    Margin = new Thickness(10, 10, 0, 0),
                    Background = Brushes.Orange,
                    FontSize = 14,
                    Height = 65,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 91,
                };

                // Create a TextBlock that supports text wrapping for the Button content
                TextBlock textBlock = new TextBlock
                {
                    Text = product.name,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.Black,
                };

                button.Content = textBlock;
                button.Click += (sender, e) =>
                {
                    ChangeTotalPrice(product.price);
                    productWindow.AddProduct(product.name, product.price); // Call method in ProductWindow to add a product to the the product window
                };

                ProductsStackPanel.Children.Add(button); // Add each button as a child to ProductsStackPanel
            }
        }

        public void ChangeTotalPrice(int productPrice)
        {
            // Get the current price as an integer from TotalPrice
            string priceText = TotalPrice.Text.Replace("kr", "").Trim();
            int price = int.Parse(priceText);

            price += productPrice;
            TotalPrice.Text = price.ToString() + " kr";
        }

        private void ResetTotalPrice()
        {
            TotalPrice.Text = "0 kr";
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            ResetTotalPrice();
            productWindow.ClearProducts();
        }
    }
}