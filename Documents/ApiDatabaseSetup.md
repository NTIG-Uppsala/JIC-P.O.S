# How to Set Up the API Database

This document guides you through setting up the database for the API. It assumes that you have MySQL installed on your system.

---

## Database Setup

### Database Structure

The API uses two tables: `products` and `sales`. Below is the database schema:

![Database Schema](images/ApiDB.png)

### Create the `.env` File

The API requires specific environment variables to run correctly. You can find the required structure in the [`.env.example`](../API/.env.example) file.

To create the `.env` file, follow these steps:

1. Navigate to the root directory of the project:
   ```bash
   cd <project_directory>
   ```

2. Copy the `.env.example` file to `.env`:
   ```bash
   cp .env.example .env
   ```

3. Open the `.env` file in your preferred text editor (e.g., `nano`):
   ```bash
   nano .env
   ```

4. Modify the values as needed and save the changes.

### Creating the Tables

To set up the database with the necessary structure, you need to have **Node.js** and **Sequelize** installed. Follow the steps below to install the required dependencies and create the tables:

1. **Install dependencies**:  
   Navigate to the API directory and run the following command to install all required dependencies, including Sequelize:

   ```bash
   npm install
   ```

2. **Set up the database**:  
   Once the dependencies are installed, simply run the Node application. Sequelize will automatically create all the necessary tables. Run the following command from the API directory:

   > This assumes you already have an empty database ready and a `.env` file with the required information.

   ```bash
   node app.js
   ```

This process will set up your database structure with Sequelize.


### Notes

- The `createdAt` and `updatedAt` fields are managed automatically using MySQL’s timestamp functionality.

---

[Back to README](../README.md)
