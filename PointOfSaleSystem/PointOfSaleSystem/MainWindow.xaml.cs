using System.Data.SQLite;
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
            InitializeCategoriesFromDatabase();
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
                        productsTableIsCreated = ProductsTable.CreateProductsTable(connection);

                        if (productsTableIsCreated)
                        {
                            haveInsertedProducts = ProductsTable.InsertProductsData(connection);
                        }
                    }

                    // Create product buttons if the table entries were created successfully or previously existed
                    if (haveInsertedProducts)
                    {
                        var products = ProductsTable.ReadProductsTable(connection);
                        if (products.Count > 0)
                        {
                            CreateProducts(products); // Pass the retrieved products to CreateProducts
                        }
                    }
                }
            }
        }
        private void InitializeCategoriesFromDatabase()
        {
            using (var connection = DatabaseHelper.CreateConnection())
            {
                if (connection != null)
                {
                    bool categoriesTableIsCreated = true;
                    bool haveInsertedCategories = true;
                    const string tableName = "categories";

                    // Create table and insert data if the products table did not yet exist
                    if (!DatabaseHelper.DoesTableExist(connection, tableName))
                    {
                        categoriesTableIsCreated = CategoriesTable.CreateCategoriesTable(connection);
                        if (categoriesTableIsCreated)
                        {
                            haveInsertedCategories = CategoriesTable.InsertCategoriesData(connection);
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
                        ordersTableIsCreated = OrdersTable.CreateOrdersTable(connection);
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
                        orderDetailsTableIsCreated = OrderDetailsTable.CreateOrderDetailsTable(connection);
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
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffa500")),
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
            string priceText = TotalPrice.Text.Replace("SEK", "").Trim();
            int price = int.Parse(priceText);

            price += productPrice;
            TotalPrice.Text = price.ToString() + " SEK";
        }

        private void ResetTotalPrice()
        {
            TotalPrice.Text = "0 SEK";
            productWindow.ClearProducts();
        }

        private void PayButtonClick(object sender, RoutedEventArgs e)
        {
            string priceText = TotalPrice.Text.Replace("SEK", "").Trim();
            int totalPrice = int.Parse(priceText);

            if (totalPrice > 0)
            {
                // Gets the current date and time
                DateTime currentDateAndTime = DateTime.Now;

                // Establish a database connection
                using (var connection = DatabaseHelper.CreateConnection())
                {
                    // Insert the order
                    bool orderInserted = OrderDetailsTable.InsertOrders(connection, currentDateAndTime, totalPrice);

                    if (orderInserted)
                    {
                        // Retrieves last inserted row ID
                        int orderId = (int)connection.LastInsertRowId;
                        int productid;
                        // Insert the order details
                        foreach (var product in productWindow.Products)
                        {
                            productid = ProductsTable.GetProductID(connection, product.ProductName); // Get the product ID from the database
                            OrderDetailsTable.InsertOrderDetails(
                                connection,
                                productid,    
                                orderId,              
                                product.ProductAmount,
                                product.ProductPrice   
                            );
                        }
                    }
                }

                // Reset the total price and display the order confirmation
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