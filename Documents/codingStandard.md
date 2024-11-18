### Coding Standard

## C#
* Code and file names are written in English.
* All comments are written in English.
* File names are written in PascalCase.
* Class, struct and record names are written in PascalCase.
* Function names are written in PascalCase.
* Variable names are written in camelCase.
* Block openings are placed beneath expressions or declarations, such as:
    * ```bash
        public struct Data
        {

        }
        ```

Refer to the following guides for a more comprehensive C# coding standard:

* [Coding Convention](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
* [Naming Convention](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)

## SQL
* Table names:
   * Use plural names for tables (e.g., `users`, `roles`).
   * Use underscores to separate words in the table name (e.g., `users_name`, `users_role`).
     
* Column names:
   * For primary key columns, name them id.
   * Use underscores to separate words in the column name (e.g., `user_name`, `user_role`).
   * For data columns, use short descriptive names that represent the data (e.g., `country_name`, `country_code`, `customer_name`), but only use the table name prefix when necessary to avoid ambiguity.
   * For foreign key columns, use the related table’s name followed by id (e.g., `customer_id`, `employee_id`).
   
---

[Back to README](../README.md)
