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

        public static string CreateAutomationId(string sourceString)
        {
            string AutomationId = sourceString.ToLower().Replace(" ", "_")
                                                 .Replace("\'", "_")
                                                 .Replace("-", "_");

            return AutomationId;
        }

        public static string[] ReadFileLines(string filename)
        {
            try
            {
                string[] fileLines = File.ReadAllLines(filename);
                return fileLines;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"The file \"{filename}\" could not be read:\n {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public static bool InsertTableDataFromTxt(SQLiteConnection connection, string filename, string tableName, string generateAutomationIdFrom = "none")
        {
            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            string[] fileLines = ReadFileLines(filename);

            // Exit if ReadFileLines failed or returned no data.
            if (fileLines == null || fileLines.Length == 0)
            {
                return false;
            }

            string currentLine;
            string[] currentValues;
            bool isspace;
            int insertCount = 0; // Track the number of successful inserts
            string formatErrors = ""; // Insert all possible errors regarding the format for the values in the fileLines
            bool formatStringParsed = false;

            string[] tableEntries = Array.Empty<string>();
            string dbFullCommand = "";
            string dbDataNames = "";
            string dbDataValues = "";
            bool generateAutomationId = false;

            if (generateAutomationIdFrom != "none")
            {
                generateAutomationId = true;
            }

            // Loop through each line from the .txt file and add the entries to the table
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
                // Parse the format specifier to interpret the db values
                else if (currentLine.StartsWith("FORMAT:"))
                {
                    tableEntries = currentLine.Split(':');
                    tableEntries = tableEntries[1].Split(',');

                    if (generateAutomationId)
                    {
                        dbDataNames += "automation_id, ";
                        dbDataValues += "@automation_id, ";
                    }

                    for (int j = 0; j < tableEntries.Length; j++)
                    {
                        string data = tableEntries[j];

                        dbDataNames += data;
                        dbDataValues += "@" + data;

                        if (j != tableEntries.Length - 1)
                        {
                            dbDataNames += ", ";
                            dbDataValues += ", ";
                        }
                    }

                    dbFullCommand = $"({dbDataNames}) VALUES ({dbDataValues})";

                    formatStringParsed = true;
                }
                // Don't parse the current line if the line containing the format specifier has not yet been parsed.
                else if (!formatStringParsed)
                {
                    continue;
                }
                else
                {
                    currentValues = currentLine.Split(',');

                    // Add a format error if the current line does not possess the right amount of data relative to the format specifier
                    if (currentValues.Length < tableEntries.Length)
                    {
                        formatErrors += $"Line {i + 1}: ";
                        for (int j = 0; j < currentValues.Length; j++)
                        {
                            formatErrors += currentValues[j];

                            if (j != currentValues.Length - 1)
                            {
                                formatErrors += ",";
                            }
                        }

                        formatErrors += "\n";

                        continue;
                    }

                    try
                    {
                        sqlite_cmd.CommandText = $"INSERT INTO {tableName} {dbFullCommand}";

                        sqlite_cmd.Parameters.Clear(); // Clear parameters before adding new ones

                        for (int j = 0; j < tableEntries.Length; j++)
                        {
                            string tableEntry = tableEntries[j];
                            string tableEntryValue = currentValues[j];

                            sqlite_cmd.Parameters.AddWithValue("@" + tableEntry.ToString(), tableEntryValue);

                            if (generateAutomationId && tableEntry == generateAutomationIdFrom)
                            {
                                sqlite_cmd.Parameters.AddWithValue("@automation_id", CreateAutomationId(tableEntryValue));
                            }
                        }

                        sqlite_cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    insertCount++; // Increment successful insert count
                }
            }

            if (!formatStringParsed)
            {
                MessageBox.Show($"Failed to find a format specifier when parsing file \"{filename}\".");
                return false;
            }

            // Nothing was inserted successfully
            if (insertCount == 0)
            {
                MessageBox.Show($"Failed to create database entries in table \"{tableName}\"!");
                return false;
            }
            // Some format errors were encountered when parsing the .txt file lines
            else if (formatErrors != "")
            {
                MessageBox.Show($"The following format errors were encountered when parsing file \"{filename}\":\n{formatErrors}");
                return true; // Some entries were inserted successfully
            }
            else
            {
                return true; // All entries were inserted successfully
            }
        }
    }
}
