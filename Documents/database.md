## Viewing the SQLite Database in DB Browser for SQLite

To view the database used in your application, follow these steps:

### 1. Download and Install DB Browser for SQLite

- Navigate to the [DB Browser for SQLite](https://sqlitebrowser.org/) website.
- Download the appropriate version for your operating system.
- Install the software by following the prompts in the installer.

### 3. Open DB Browser for SQLite

- Launch **DB Browser for SQLite** after installation.

### 4. Open the Database File

- In DB Browser, click on the **Open Database** button.
- Navigate to the folder where your database file is located:
  - Go to `C:\Users\[YourUsername]\AppData\Local\Restaurant Point Of Sale System\`.
  - Select the `database.db` file and click **Open**.

**Note:** The database file (`database.db`) is created after running the application for the first time. Ensure that the application has been run at least once before attempting to open the database file.

### 5. View the Tables

- Once the database is loaded, you will see a list of tables in the left sidebar.
- To view a specific table (e.g., `orders`, `order_details`, or `products`), click on the table name.

### 6. Browse the Data in the Table

- After selecting the table (e.g., `orders`, `order_details`, or `products`), you can browse the data in that table.
- Click on the **Browse Data** tab to see the records stored in the table.
- You can filter, sort, and view the data in a tabular format.

### 7. Perform Queries (Optional)

- To run a custom queries, go to the **Execute SQL** tab.
- Type your SQL query to retrieve or manipulate data in the table.

---

[Back to README](../README.md)