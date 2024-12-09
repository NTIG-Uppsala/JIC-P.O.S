using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PointOfSaleSystem.Database;
using System.IO;
using System.Windows;

// Description: This class sends data to the API:s post route.
namespace PointOfSaleSystem.ApiPost
{
    public class SendData
    {
        public static async Task SendDataToDatabase()
        {
            var data = new List<object>();

            // Create a connection to the database
            using (var connection = DatabaseHelper.CreateConnection())
            {
                // Get the order details after a specified date from the database
                var orderDetails = GetOrderDetails(connection);

                foreach (var orderDetail in orderDetails)
                {
                    // Get the product name from the products table
                    var product = ProductsTable.GetProductNameById(connection, orderDetail.ProductId);

                    // Add all the order information to the data list
                    data.Add(new
                    {
                        restaurant_name = "Downtown Bistro",
                        product_name = product,
                        price = orderDetail.UnitPrice,
                        quantity = orderDetail.Quantity
                    });
                }
            }

            // Serialize the data list to JSON
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                // Send the JSON data to the API
                var response = await client.PostAsync("http://localhost:3000/api/sales/password123", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data sent successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to send data. Status code: {response.StatusCode}");
                }
            }
        }

        public static List<OrderDetail> GetOrderDetails(SQLiteConnection connection)
        {
            var orderDetails = new List<OrderDetail>();
            // Read the last date from the date.txt file
            DateTime lastDate = ReadDateFile();

            try
            {
                // Get the order IDs after the last date
                List<int> orderId = OrdersTable.GetOrderIdsAfterDate(connection, lastDate);

                SQLiteCommand sqlite_cmd = connection.CreateCommand();

                // Create placeholders for each ID in the orderId list in the format "@id0, @id1, @id2, ..."
                string placeholders = string.Join(",", orderId.Select((_, index) => $"@id{index}"));

                // Update the query with the placeholders
                sqlite_cmd.CommandText = $"SELECT product_id, quantity, unit_price FROM order_details WHERE order_id IN ({placeholders})";

                // Add parameters dynamically
                for (int i = 0; i < orderId.Count; i++)
                {
                    sqlite_cmd.Parameters.AddWithValue($"@id{i}", orderId[i]);
                }

                // Read the data from the order_details table and add them to the orderDetails list
                using (SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader())
                {

                    while (sqlite_datareader.Read())
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            ProductId = sqlite_datareader.GetInt32(0),
                            Quantity = sqlite_datareader.GetInt32(1),
                            UnitPrice = sqlite_datareader.GetInt32(2)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading from order_details table: {ex.Message}");
            }

            return orderDetails;
        }

        // Read the date from the date.txt file
        private static DateTime ReadDateFile()
        {
            const string filename = "date.txt";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            try
            {
                string[] fileLines = File.ReadAllLines(filePath);
                return DateTime.Parse(fileLines[0]);
            }
            catch (IOException ex)
            {
                MessageBox.Show("The file could not be read:\n" + ex.Message);
                return DateTime.MinValue;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The date format in the file is invalid:\n" + ex.Message);
                return DateTime.MinValue;
            }
        }
    }

    // Class to hold the order details
    public class OrderDetail
    {
        public int order_id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}