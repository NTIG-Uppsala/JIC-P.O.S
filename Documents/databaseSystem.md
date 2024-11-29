## Database System
This program uses an SQLite-based database solution.

### Database Initialization
When the program is started for the first time, it creates the ``database.db`` file in the user's ``AppData\Local\Restaurant Point Of Sale System`` directory.

### Menu items
The database table "products" contains information about the menu items.
It initially gets created from [this](../PointOfSaleSystem/PointOfSaleSystem/InitialProductsData.txt) ``.txt`` file if the table does not yet exist (i.e. when [the program is first run](#database-initialization)).

Each entry in the file is placed under one of the following categories:

1. Starters
2. Main courses
3. Seafood
4. Pizza
5. Desserts
6. Sauces
7. Wines
8. Draft Beer
9. Bottled Beer
10. Cider
11. Coffee

Each item is added to the table with its corresponding category.

To add a new default menu item, add the name of the product and price under desired category in this format:

```
Chocolate mousse,90
```

To update the database with the new data the table then needs to be deleted and recreated.
This can be done either by deleting the whole database file at [its location](#database-initialization), or deleting the table through a tool like DB Browser (as mentioned in [this section](#deleting-a-table)) and then rerunning the program.

### Viewing and Changing the Database
Instead of directly changing the default data that the database uses during initialization, it is possible to directly interact with the database once it's created using DB Browser.

#### Downloading DB Browser
Download and install [DB Browser for SQLite](https://sqlitebrowser.org/dl/), a GUI program that will allow you to easily view and change the database.
Then, simply open or drag and drop the ``database.db`` file from [its location](#database-initialization) into Db Browser.

![DB Browser](images/dbBrowser.jpg)

#### Add to a Table
To add a new menu item, run the following inside of the "Execute SQL" tab after switching out the template entries to the desired data:

```SQL
INSERT INTO products (product_name, product_automation_id, price, category_id) VALUES ("Chocolate mousse", "chocolate_mousse_button", 90, 5);
```

Afterwards, press Ctrl+Return or click the playhead icon, and then click on "Write Changes" to update the database.

#### Deleting a Table
To delete a table in the database, while in the "Database Structure" tab, right click on the table name in the tree structure and select "Delete Table" and then click on "Write Changes".

---

[Back to README](../README.md)