using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PointOfSaleSystem.Database
{
    public class CategoriesTable
    {
        public static bool CreateCategoriesTable(SQLiteConnection connection)
        {
            string createsql = "CREATE TABLE categories (id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "name VARCHAR(255), " +
                "hex_color VARCHAR(11)" +
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
    }
}
