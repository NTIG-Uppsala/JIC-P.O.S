using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PointOfSaleSystem.Database
{
    public class OrderDetailsTable
    {
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
    }
}
