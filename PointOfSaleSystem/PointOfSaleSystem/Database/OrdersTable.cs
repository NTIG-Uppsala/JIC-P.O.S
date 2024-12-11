using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PointOfSaleSystem.Database
{
    public class OrdersTable
    {
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
        
        public static List<int> GetOrderIdsAfterDate(SQLiteConnection connection, DateTime lastDate)
        {
            // Query to filter by lastDate
            string query = "SELECT id FROM orders WHERE date > @lastDate";

            SQLiteCommand sqlite_cmd = connection.CreateCommand();
            sqlite_cmd.CommandText = query;

            // Format the DateTime to match SQLite's date format
            sqlite_cmd.Parameters.AddWithValue("@lastDate", lastDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));


            // List to hold the IDs
            List<int> orderIdsAfterLastDate = new List<int>();

            try
            {
                SQLiteDataReader reader = sqlite_cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0); // Read the ID column
                    orderIdsAfterLastDate.Add(id); // Add it to the list
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve IDs: {ex.Message}");
            }
            return orderIdsAfterLastDate;
        }
    }
}
