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
                "automation_id VARCHAR(255), " +
                "hex_color VARCHAR(7)" +
            ");";

            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            sqlite_cmd.CommandText = createsql;
            try
            {
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create database table \"categories\": {ex.Message}");
                return false;
            }
            return true;
        }

        public static bool InsertCategoriesData(SQLiteConnection connection)
        {
            bool returnValue;
            returnValue = DatabaseHelper.InsertTableDataFromTxt(connection, "InitialCategoriesData.txt", "categories", "name");
            return returnValue;
        }

        public static List<MainWindow.Category> ReadCategoriesTable(SQLiteConnection connection)
        {
            var categoriesList = new List<MainWindow.Category>(); // List containing categories created with the Category struct

            try
            {
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd = connection.CreateCommand();
                sqlite_cmd.CommandText = "SELECT id, name, automation_id, hex_color FROM categories";
                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    int categoryId = sqlite_datareader.GetInt32(0);
                    string categoryName = sqlite_datareader.GetString(1);
                    string categoryAutomationId = sqlite_datareader.GetString(2);
                    string categoryHexColor = sqlite_datareader.GetString(3);

                    categoriesList.Add(new MainWindow.Category(categoryId, categoryName, categoryAutomationId, categoryHexColor));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading from table \"categories\": {ex.Message}");
            }
            return categoriesList;
        }
    }
}
