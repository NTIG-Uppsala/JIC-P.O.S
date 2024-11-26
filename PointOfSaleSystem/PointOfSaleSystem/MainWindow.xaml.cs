using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PointOfSaleSystem.Database;
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
            InitializeProductsFromDatabase();
            InitializeOrdersFromDatabase();
            InitializeOrderDetailsFromDatabase();
        }
        private void InitializeProductsFromDatabase()
        {

            using (var connection = DatabaseHelper.CreateConnection())
            {
                if (connection != null)
                {
                    bool haveInsertedProducts = true;
                    bool productsTableIsCreated = true;
                    const string tableName = "products";

                    // Create table and insert data if the products table did not yet exist
                    if (!DatabaseHelper.DoesTableExist(connection, tableName))
                    {
                        productsTableIsCreated = DatabaseHelper.CreateProductsTable(connection);

                        if (productsTableIsCreated)
                        {
                            haveInsertedProducts = DatabaseHelper.InsertProductsData(connection);
                        }
                    }

                    // Create product buttons if the table entries were created successfully or previously existed
                    if (haveInsertedProducts)
                    {
                        var products = DatabaseHelper.ReadProductsTable(connection);
                        if (products.Count > 0)
                        {
                            CreateProducts(products); // Pass the retrieved products to CreateProducts
                        }
                    }
                }
            }
        }

        // Initialize the orders table in the database
        private void InitializeOrdersFromDatabase()
        {
            using (var connection = DatabaseHelper.CreateConnection())
            {
                if (connection != null)
                {
                    bool ordersTableIsCreated = true;
                    const string tableName = "orders";

                    // Create table if the orders table did not yet exist
                    if (!DatabaseHelper.DoesTableExist(connection, tableName))
                    {
                        ordersTableIsCreated = DatabaseHelper.CreateOrdersTable(connection);
                    }
                }
            }
        }

        private void InitializeOrderDetailsFromDatabase()
        {
            using (var connection = DatabaseHelper.CreateConnection())
            {
                if (connection != null)
                {
                    bool orderDetailsTableIsCreated = true;
                    const string tableName = "order_details";

                    // Create table if the orders table did not yet exist
                    if (!DatabaseHelper.DoesTableExist(connection, tableName))
                    {
                        orderDetailsTableIsCreated = DatabaseHelper.CreateOrderDetailsTable(connection);
                    }
                }
            }
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
        public void CreateProducts(List<Product> listOfProducts)
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
                    OrderConfirmation.Visibility = Visibility.Hidden;
                };

                ProductsWrapPanel.Children.Add(button); // Add each button as a child to ProductsStackPanel
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
            productWindow.ClearProducts();
        }

        // Reset the total price and clear the product window
        private void PayButtonClick(object sender, RoutedEventArgs e)
        {
            string priceText = TotalPrice.Text.Replace("kr", "").Trim();
            int price = int.Parse(priceText);

            // If there are products in the product window
            if (price > 0)
            {
                // Gets the product names, price and amount
                string productInfo = "";
                foreach (var product in productWindow.Products)
                {
                    productInfo += $"Product: {product.ProductName}, Price: {product.ProductPrice}, Amount: {product.ProductAmount}\n";

                }

                // Gets the current date and time
                DateTime currentDateAndTime = DateTime.Now;

                // Reset the total price and displays the order confirmation
                OrderConfirmation.Visibility = Visibility.Visible;
                ResetTotalPrice();
            }
        }


        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            ResetTotalPrice();
            OrderConfirmation.Visibility = Visibility.Hidden;
        }
    }
}