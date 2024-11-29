using System.IO;
using System.Windows;
using System.Data.SQLite;
using System.Security.Permissions;

namespace PointOfSaleSystem.Database
{
    public class DatabaseHelper
    {
        public static SQLiteConnection CreateConnection()
        {
            string dbLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                 "Restaurant Point Of Sale System"
            );

            CreateDatabaseFolder(dbLocation);

            string dbFileName = "database.db";

            string connectionString = $"Data Source={Path.Combine(dbLocation, dbFileName)}; Version=3; Compress=True;";

            SQLiteConnection sqlite_conn = new SQLiteConnection(connectionString);
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening database connection: {ex.Message}");
            }
            return sqlite_conn;
        }

        private static void CreateDatabaseFolder(string dbLocation)
        {
            if (!Directory.Exists(dbLocation))
            {
                Directory.CreateDirectory(dbLocation);
            }
        }

        public static bool CreateProductsTable(SQLiteConnection connection)
        {
            string createsql = "CREATE TABLE products (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "product_name VARCHAR(255), " +
                "product_automation_id VARCHAR(255), " +
                "price INT, " +
                "category_id INT" +
            ");";

            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            sqlite_cmd.CommandText = createsql;
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create database table \"products\": {ex.Message}");
                return false;
            }
            return true;
        }

        public static bool DoesTableExist(SQLiteConnection connection, string tableName)
        {
            using (var command = new SQLiteCommand($"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'", connection))
            {
                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }

        private static string[] ReadProductsFile()
        {
            const string filename = "InitialProductsData.txt";

            try
            {
                string[] fileLines = File.ReadAllLines(filename);
                return fileLines;
            }
            catch (IOException ex)
            {
                MessageBox.Show("The file could not be read:\n" + ex.Message);
                return Array.Empty<string>();
            }
        }

        // Get the product ID from the products table (used for inserting order details)
        public static int GetProductID(SQLiteConnection connection, string productName)
        {
            SQLiteCommand sqlite_cmd = connection.CreateCommand();
            sqlite_cmd.CommandText = "SELECT id FROM products WHERE product_name = @productName";
            sqlite_cmd.Parameters.AddWithValue("@productName", productName); // Add parameter safely

            using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
            {
                if (sqlite_datareader.Read())
                {
                    return sqlite_datareader.GetInt32(0); // Return the product ID
                }
            }
            return -1;
        }

        private static string CreateNameId(string productName)
        {
            string AutomationId = productName.ToLower().Replace(" ", "_")
                                                 .Replace("\'", "_")
                                                 .Replace("-", "_");

            return $"{AutomationId}_button";
        }

        public static bool InsertProductsData(SQLiteConnection connection)
        {
            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            string[] fileLines = ReadProductsFile();

            // Exit function if ReadProductsFile failed or returned nothing.
            if (fileLines == null || fileLines.Length == 0)
            {
                return false;
            }

            int currentCategoryId = 1; // Used to track which category each product is assigned to
            string currentLine;
            string[] lineSplit;
            bool isspace;
            int insertCount = 0; // Track the number of successful inserts

            // Loop through each line from the .txt file and add the entries to the "products" table
            for (int i = 0; i < fileLines.Length; i++)
            {
                currentLine = fileLines[i];

                isspace = currentLine.All(char.IsWhiteSpace);

                // Treat lines starting with '#' as comments
                if (currentLine.StartsWith('#'))
                {
                    continue;
                }
                // Skip if blank line is found
                else if (isspace)
                {
                    continue;
                }
                // Redefine currentCategoryId if the line only contains a number (indicating new category id) and then move to the next line
                else if (int.TryParse(currentLine.Trim(), out int categoryId))
                {
                    currentCategoryId = categoryId;
                    continue;
                }
                else
                {
                    lineSplit = currentLine.Split(',');

                    // Skip if not enough data
                    if (lineSplit.Length < 2)
                    {
                        continue;
                    }
                    string productName = lineSplit[0];
                    string productAutomationId = CreateNameId(productName); // Generate unique product name ID for UI automation
                    int productPrice = int.Parse(lineSplit[1]);

                    try
                    {
                        sqlite_cmd.CommandText = "INSERT INTO products (product_name, product_automation_id, price, category_id) VALUES (@product_name, @product_automation_id, @price, @category_id);";

                        sqlite_cmd.Parameters.Clear(); // Clear parameters before adding new ones
                        sqlite_cmd.Parameters.AddWithValue("@product_name", productName);
                        sqlite_cmd.Parameters.AddWithValue("@product_automation_id", productAutomationId);
                        sqlite_cmd.Parameters.AddWithValue("@price", productPrice);
                        sqlite_cmd.Parameters.AddWithValue("@category_id", currentCategoryId);

                        sqlite_cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    insertCount++; // Increment successful insert count
                }
            }

            if (insertCount > 0)
            {
                return true; // Products inserted successfully
            }
            else
            {
                MessageBox.Show($"Failed to create database entries in table \"products\"!");
                return false;
            }
        }

        public static List<MainWindow.Product> ReadProductsTable(SQLiteConnection connection)
        {
            var productsList = new List<MainWindow.Product>(); // List containing products created with the Product struct

            try
            {

                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd = connection.CreateCommand();
                sqlite_cmd.CommandText = "SELECT product_name, product_automation_id, price FROM products";
                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    string productName = sqlite_datareader.GetString(0);
                    string productAutomationId = sqlite_datareader.GetString(1);
                    int productPrice = sqlite_datareader.GetInt32(2);

                    productsList.Add(new MainWindow.Product(productName, productAutomationId, productPrice));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading from products table: {ex.Message}");
            }
            return productsList;
        }


        // order tables
        public static int GetOrderDetailsCount(SQLiteConnection connection)
        {
            int count = 0;

            try
            {
                SQLiteCommand sqlite_cmd = connection.CreateCommand();
                // Use COUNT(*) to get the total number of rows in the order_details table
                sqlite_cmd.CommandText = "SELECT COUNT(*) FROM order_details";

                // Execute the query and get the result
                count = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while counting the rows in the order_details table: {ex.Message}");
            }

            return count;
        }

        public static bool CreateOrdersTable(SQLiteConnection connection)
        {
            string createsql = "CREATE TABLE orders (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                               "date DATE, " +
                               "total_price SMALLINT);"; 

            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            sqlite_cmd.CommandText = createsql;
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create database table \"orders\": {ex.Message}");
                return false;
            }
            return true;
        }

        public static bool CreateOrderDetailsTable(SQLiteConnection connection)
        {
            string createsql = "CREATE TABLE order_details (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                               "product_id INTEGER, " +  // Foreign key to 'products'
                               "order_id INTEGER, " +    // Foreign key to 'orders'
                               "quantity SMALLINT, " +
                               "unit_price SMALLINT, " +
                               "FOREIGN KEY (product_id) REFERENCES products(id), " +  // Linking to products table
                               "FOREIGN KEY (order_id) REFERENCES orders(id));";  // Linking to orders table

            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            sqlite_cmd.CommandText = createsql;
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create database table \"order_details\": {ex.Message}");
                return false;
            }
            return true;
        }

        public static bool InsertOrders(SQLiteConnection connection, DateTime date, int total_price)
        {
            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            try
            {
                sqlite_cmd.CommandText = "INSERT INTO orders (date, total_price) VALUES (@date, @total_price);";

                sqlite_cmd.Parameters.Clear(); // Clear parameters before adding new ones
                sqlite_cmd.Parameters.AddWithValue("@date", date);
                sqlite_cmd.Parameters.AddWithValue("@total_price", total_price);

                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create database entries in table \"orders\": {ex.Message}");
                return false;
            }
            return true;
        }

        public static bool InsertOrderDetails(SQLiteConnection connection, int product_id, int order_id, int quantity, int unit_price)
        {
            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            try
            {
                sqlite_cmd.CommandText = "INSERT INTO order_details (product_id, order_id, quantity, unit_price) " +
                                         "VALUES (@product_id, @order_id, @quantity, @unit_price);";

                sqlite_cmd.Parameters.Clear(); // Clear parameters before adding new ones
                sqlite_cmd.Parameters.AddWithValue("@product_id", product_id);
                sqlite_cmd.Parameters.AddWithValue("@order_id", order_id);
                sqlite_cmd.Parameters.AddWithValue("@quantity", quantity);
                sqlite_cmd.Parameters.AddWithValue("@unit_price", unit_price);

                sqlite_cmd.ExecuteNonQuery(); // Execute the SQL command
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create database entries in table \"order_details\": {ex.Message}");
                return false;
            }
            return true;
        }
    }
}



