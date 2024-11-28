using System.IO;
using System.Windows;
using System.Data.SQLite;

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
                "product_name VARCHAR(20), " +
                "identifier_name VARCHAR(255), " +
                "price VARCHAR(255), " +
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
            const string filename = "Products.txt";

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

        private static string CreateNameId(string productName)
        {
            string nameId = productName.ToLower().Replace(" ", "_")
                                                 .Replace("\'", "_")
                                                 .Replace("-", "_");

            return $"{nameId}_button";
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

            int current_category = int.Parse(fileLines[0]);
            string currentLine;
            string[] lineSplit;
            bool isspace;
            int insertCount = 0; // Track the number of successful inserts

            // Start loop on the first item in the initial category
            for (int i = 1; i < fileLines.Length; i++)
            {
                currentLine = fileLines[i];

                isspace = currentLine.All(char.IsWhiteSpace);

                // Continue with the next category of products when a blank line is found
                if (isspace)
                {
                    // Next line after a blank line will be the next category ID
                    i++;
                    current_category = int.Parse(fileLines[i]);
                    continue;
                }

                lineSplit = currentLine.Split(',');
                string productName = lineSplit[0];
                string productAutomationId = CreateNameId(productName); // Generate unique product name ID for UI automation
                string productPrice = lineSplit[1];

                try
                {
                    sqlite_cmd.CommandText = "INSERT INTO products (product_name, identifier_name, price, category_id) VALUES (@product_name, @identifier_name, @price, @category_id);";

                    sqlite_cmd.Parameters.Clear(); // Clear parameters before adding new ones
                    sqlite_cmd.Parameters.AddWithValue("@product_name", productName);
                    sqlite_cmd.Parameters.AddWithValue("@identifier_name", productAutomationId);
                    sqlite_cmd.Parameters.AddWithValue("@price", productPrice);
                    sqlite_cmd.Parameters.AddWithValue("@category_id", current_category);

                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    continue;
                }
                insertCount++; // Increment successful insert count
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
                sqlite_cmd.CommandText = "SELECT product_name, identifier_name, price FROM products";
                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    string productName = sqlite_datareader.GetString(0);
                    string productAutomationId = sqlite_datareader.GetString(1);
                    int productPrice = int.Parse(sqlite_datareader.GetString(2));

                    productsList.Add(new MainWindow.Product(productName, productAutomationId, productPrice));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading from products table: {ex.Message}");
            }
            return productsList;
        }
    }
}
