using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PointOfSaleSystem.Database;
using PointOfSaleSystem.UserControls;
using PointOfSaleSystem.ApiPost;
using System.Runtime.CompilerServices;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

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

            // Database initialization
            InitializeCategoriesFromDatabase();
            InitializeProductsFromDatabase();
            InitializeOrdersFromDatabase();
            InitializeOrderDetailsFromDatabase();
            InitializeSendData();
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

                    // Create table and insert data if the categories table did not yet exist
                    if (!DatabaseHelper.DoesTableExist(connection, tableName))
                    {
                        categoriesTableIsCreated = CategoriesTable.CreateCategoriesTable(connection);
                        if (categoriesTableIsCreated)
                        {
                            haveInsertedCategories = CategoriesTable.InsertCategoriesData(connection);
                        }
                    }

                    // Create category buttons if the table entries were created successfully or previously existed
                    if (haveInsertedCategories)
                    {
                        var categories = CategoriesTable.ReadCategoriesTable(connection);
                        if (categories.Count > 0)
                        {
                            CreateCategoryButtons(categories); // Pass the retrieved categories to CreateCategoryButtons
                        }
                    }
                }
            }
        }

        public List<Product> products;

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
                        products = ProductsTable.ReadProductsTable(connection);
                        if (products.Count > 0)
                        {
                            CreateProductButtons(products); // Pass the retrieved products to CreateProductButtons
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

        // Struct representing a product to create buttons from
        public struct Product
        {
            public Product(string productName, string productAutomationId, int productPrice, int foreignCategoryId, string foreignCategoryHexColor, bool productIsCommon)
            {
                name = productName;
                automationId = productAutomationId;
                price = productPrice;
                categoryId = foreignCategoryId;
                hexColor = foreignCategoryHexColor;
                isCommon = productIsCommon;
            }

            public string name { get; init; }
            public string automationId { get; init; }
            public int price { get; init; }
            public int categoryId { get; init; }
            public string hexColor { get; init; }
            public bool isCommon { get; init; }
        }

        // Struct representing a category to create buttons from
        public struct Category
        {
            public Category(int categoryId, string categoryName, string categoryAutomationId, string categoryHexColor)
            {
                id = categoryId;
                name = categoryName;
                automationId = categoryAutomationId;
                hexColor = categoryHexColor;
            }

            public int id { get; init; }
            public string name { get; init; }
            public string automationId { get; init; }
            public string hexColor { get; init; }
        }

        public Grid? lastCategoryButtonContainer = null;

        public void CreateProductButtons(List<Product> listOfProducts)
        {
            foreach (var product in listOfProducts)
            {
                // Create a button for each product
                Button button = new Button
                {
                    Name = product.automationId, // Used for x:Name value to create an id for the item
                    Margin = new Thickness(10, 10, 0, 0),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(product.hexColor)),
                    FontSize = 14,
                    Height = 65,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 91,
                    Tag = product.categoryId,
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

                // Hide button by default if the product is not common
                if (!product.isCommon)
                {
                    button.Visibility = Visibility.Collapsed;
                }

                ProductsWrapPanel.Children.Add(button); // Add each button as a child to ProductsStackPanel
            }
        }

        public void CreateCategoryButtons(List<Category> listOfCategories)
        {
            Button returnButton = new Button
            {
                Name = "categories_return",
                Margin = new Thickness(10, 10, 0, 0),
                Background = Brushes.White,
                FontSize = 14,
                Height = 65,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 91,
            };

            TextBlock returnTextBlock = new TextBlock
            {
                Text = "Return",
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.Black,
            };

            returnButton.Content = returnTextBlock;
            returnButton.Click += (sender, e) =>
            {
                // Make all common products from every category visible when pressing the return button
                foreach (UIElement item in ProductsWrapPanel.Children)
                {
                    if (item is FrameworkElement frameworkElement)
                    {
                        string itemName = frameworkElement.Name;
                        Product matchingProduct = products.Find(p => p.automationId == itemName);

                        if (matchingProduct.isCommon)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                }

                // If not null, make all the mirrored buttons in the last selected category button container visible
                if (lastCategoryButtonContainer != null)
                {
                    foreach (UIElement child in lastCategoryButtonContainer.Children)
                    {
                        child.Visibility = Visibility.Visible;
                    }
                    lastCategoryButtonContainer = null;
                }
            };
            CategoriesWrapPanel.Children.Add(returnButton);

            foreach (var category in listOfCategories)
            {
                // Use a Grid to stack buttons on top of each other
                Grid categoryButtonContainer = new Grid
                {
                    Margin = new Thickness(10, 10, 0, 0),
                };

                // Create mirrored buttons for a 3d effect
                Button mirrorButton1 = new Button
                {
                    Name = category.automationId + "_mirrored_button1",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(category.hexColor)),
                    Width = 91,
                    Height = 65,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(6, 6, 0, 0), // Slightly offset the button downwards and to the right
                    IsHitTestVisible = false // Make the button non-interactive
                };

                Button mirrorButton2 = new Button
                {
                    Name = category.automationId + "_mirrored_button2",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(category.hexColor)),
                    Width = 91,
                    Height = 65,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(3, 3, 0, 0),
                    IsHitTestVisible = false
                };

                // Create the main category button
                Button categoryButton = new Button
                {
                    Name = category.automationId,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(category.hexColor)),
                    FontSize = 14,
                    Width = 91,
                    Height = 65,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                TextBlock textBlock = new TextBlock
                {
                    Text = category.name,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.Black,
                };

                categoryButton.Content = textBlock;
                categoryButton.Click += (sender, e) =>
                {
                    if (lastCategoryButtonContainer != null && lastCategoryButtonContainer != categoryButtonContainer)
                    {
                        // Make the mirrored buttons visible again for the previously selected category
                        foreach (UIElement child in lastCategoryButtonContainer.Children)
                        {
                            child.Visibility = Visibility.Visible;
                        }
                    }

                    if (lastCategoryButtonContainer != categoryButtonContainer)
                    {
                        // Go through items in the current button container grid and hide the mirrored buttons
                        foreach (UIElement child in categoryButtonContainer.Children)
                        {
                            if (child is FrameworkElement frameworkElement)
                            {
                                // Hide the child if it is a mirrored button
                                if (frameworkElement.Name.EndsWith("mirrored_button1") || frameworkElement.Name.EndsWith("mirrored_button2"))
                                {
                                    child.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                        lastCategoryButtonContainer = categoryButtonContainer; // Store the related button container for the last clicked category
                    }

                    // Show only the products which belong to the selected category
                    foreach (UIElement item in ProductsWrapPanel.Children)
                    {
                        if (item.Visibility == Visibility.Collapsed)
                        {
                            item.Visibility = Visibility.Visible;
                        }

                        if (item is FrameworkElement frameworkElement)
                        {
                            var tag = frameworkElement.Tag;
                            int productCategoryId = int.Parse(tag.ToString());

                            if (productCategoryId != category.id)
                            {
                                item.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                };

                // Add the buttons to the container
                categoryButtonContainer.Children.Add(mirrorButton1);  // Add first mirrored button
                categoryButtonContainer.Children.Add(mirrorButton2);  // Add second mirrored button
                categoryButtonContainer.Children.Add(categoryButton); // Add main category button

                CategoriesWrapPanel.Children.Add(categoryButtonContainer); // Add the button container to the CategoriesWrapPanel
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
                        int productId;
                        // Insert the order details
                        foreach (var product in productWindow.Products)
                        {
                            productId = ProductsTable.GetProductId(connection, product.ProductName); // Get the product ID from the database
                            OrderDetailsTable.InsertOrderDetails(
                                connection,
                                productId,
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

        private void InitializeSendData()
        {
            // Ensure the date file exists
            SendData.EnsureDateFileExists();
            DateTime lastOrderDate = SendData.ReadDateFile();

            // Check if the last order date is not today
            if (lastOrderDate.Date < DateTime.Now.Date)
            {
                SendData.SendDataToDatabase();
                SendData.WriteDateToFile(DateTime.Now);
            }
        }
    }
}