using System.IO;
using System.Windows;
using System.Data.SQLite;
using System.Security.Permissions;
using static PointOfSaleSystem.MainWindow;

namespace PointOfSaleSystem.Database
{
    public class ProductsTable
    {
        public static bool CreateProductsTable(SQLiteConnection connection)
        {
            string createsql = "CREATE TABLE products (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "name VARCHAR(255), " +
                "automation_id VARCHAR(255), " +
                "price INT, " +
                "category_id INT, " +
                "is_common INT, " +
                "FOREIGN KEY (category_id) REFERENCES categories(id)" +  // Linking to categories table
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
        public static int GetProductId(SQLiteConnection connection, string productName)
        {
            SQLiteCommand sqlite_cmd = connection.CreateCommand();
            sqlite_cmd.CommandText = "SELECT id FROM products WHERE name = @productName";
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

        public static bool InsertProductsData(SQLiteConnection connection)
        {
            bool returnValue;
            returnValue = DatabaseHelper.InsertTableDataFromTxt(connection, "InitialProductsData.txt", "products", "name");
            return returnValue;
        }

        public static List<MainWindow.Product> ReadProductsTable(SQLiteConnection connection)
        {
            var productsList = new List<MainWindow.Product>(); // List containing products created with the Product struct

            try
            {
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd = connection.CreateCommand();

                // Select entries from the products table and utilize category_id to retrieve the hex_color entry from the categories table
                sqlite_cmd.CommandText = @"
                    SELECT product.name, product.automation_id, product.price, product.category_id, category.hex_color, product.is_common
                    FROM products product
                    LEFT JOIN categories category ON product.category_id = category.id";

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    string productName = sqlite_datareader.GetString(0);
                    string productAutomationId = sqlite_datareader.GetString(1);
                    int productPrice = sqlite_datareader.GetInt32(2);
                    int foreignCategoryId = sqlite_datareader.GetInt32(3);
                    string foreignCategoryHexColor = sqlite_datareader.GetString(4);
                    int isCommonInt = sqlite_datareader.GetInt32(5);

                    bool isCommon = isCommonInt == 1; // Turn the int to a boolean

                    productsList.Add(new MainWindow.Product(productName, productAutomationId, productPrice, foreignCategoryId, foreignCategoryHexColor, isCommon));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading from products table: {ex.Message}");
            }
            return productsList;
        }

        public static string GetProductNameById(SQLiteConnection connection, int productId)
        {
            string product = null; // Declare it outside the using block to make it accessible later

            try
            {
                SQLiteCommand sqlite_cmd = connection.CreateCommand();
                sqlite_cmd.CommandText = "SELECT product_name FROM products WHERE id = @productId";
                sqlite_cmd.Parameters.AddWithValue("@productId", productId);

                using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                {
                    if (sqlite_datareader.Read())
                    {
                        product = sqlite_datareader.GetString(0); // Assign value to product
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading from products table: {ex.Message}");
            }

            return product; // Return the product name
        }
    }
}