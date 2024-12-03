using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PointOfSaleSystem.Database
{
    public class DatabaseHelper
    {
        public static SQLiteConnection CreateConnection()
        {
            string dbLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                 "Restaurant Point of Sale System"
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

        public static bool DoesTableExist(SQLiteConnection connection, string tableName)
        {
            using (var command = new SQLiteCommand($"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'", connection))
            {
                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
