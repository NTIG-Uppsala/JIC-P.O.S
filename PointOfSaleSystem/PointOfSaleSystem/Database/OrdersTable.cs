using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
    }
}
