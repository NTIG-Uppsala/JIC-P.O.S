// Description: This file contains the code for the Node.js server that serves the API endpoints for the sales data.
// The server uses the Sequelize ORM to interact with the MySQL database and fetch the data.The server listens on port 3000 by default.

const express = require('express');
const fs = require('fs');
const path = require('path');
const app = express();
require('dotenv').config();
const { Sequelize, DataTypes } = require('sequelize');

app.use(express.json());

const sequelize = new Sequelize(process.env.DB_NAME, process.env.DB_USER, process.env.DB_PASSWORD, {
    host: process.env.DB_HOST,
    dialect: 'mysql',  // Sequelize uses mysql2 automatically with this setting
    dialectModule: require('mysql2'),  // Explicitly use mysql2
    port: process.env.DB_PORT,
});

// Define Sales model
let sale = sequelize.define('sales', {
    restaurant_id: {
        type: DataTypes.INTEGER,
        allowNull: false,
    },
    total_amount: {
        type: DataTypes.INTEGER,
        allowNull: false,
    }
}, {
    timestamps: true,
});

// Define Products model
let product = sequelize.define('products', {
    product_name: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    price: {
        type: DataTypes.SMALLINT,
        allowNull: false,
    }
}, {
    timestamps: false,
});

// Define Sale_Products junction table
let sale_product = sequelize.define('sale_products', {
    sale_id: {
        type: DataTypes.INTEGER,
        allowNull: false,
        references: {
            model: sale,
            key: 'id',
        },
    },
    product_id: {
        type: DataTypes.INTEGER,
        allowNull: false,
        references: {
            model: product,
            key: 'id',
        },
    },
    quantity: {
        type: DataTypes.SMALLINT,
        allowNull: false,
    }
}, {
    timestamps: false,
});

let restaurant = sequelize.define('restaurants', {
    name: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    address: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    email: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    total_sales: {
        type: DataTypes.INTEGER,
    }
}, {
    timestamps: false,
});


// Sale <-> Product (Many-to-Many through SaleProduct)
sale.belongsToMany(product, { through: sale_product, foreignKey: 'sale_id' });
product.belongsToMany(sale, { through: sale_product, foreignKey: 'product_id' });

// Define the association between Sale and Restaurant
restaurant.hasMany(sale, { foreignKey: 'restaurant_id' });  // A restaurant has many sales
sale.belongsTo(restaurant, { foreignKey: 'restaurant_id' });  // A sale belongs to a restaurant


(async () => {
    try {
        await sequelize.authenticate();
        console.log('Connection has been established successfully.');
    } catch (error) {
        console.error('Unable to connect to the database:', error);
        process.exit(1);  // Exit if database connection fails
    }
})();

// Sync the models with the database
(async () => {
    try {
        await sequelize.sync();
        console.log('Models synced successfully.');
    } catch (error) {
        console.error('Unable to sync models with the database:', error);
        process.exit(1);  // Exit if model sync fails
    }
})();

// GET request to get all products data
app.get('/api/products', async (req, res) => {
    try {
        // Fetch all products from the database
        const products = await product.findAll();

        // Return the products as JSON
        res.json(products);
    } catch (error) {
        console.error('Error fetching products:', error);
        res.status(500).send('Unable to fetch products');
    }
});

// GET request to get all sales data
app.get('/api/sales', async (req, res) => {
    try {
        // Fetch all sales from the database
        const sales = await sale.findAll();

        // Return the sales as JSON
        res.json(sales);
    } catch (error) {
        console.error('Error fetching sales:', error);
        res.status(500).send('Unable to fetch sales');
    }
});

// GET request to get sales data for a specific product
app.get('/api/sales/:product_id', async (req, res) => {
    try {
        // Fetch the sale with the given ID
        const product_id = req.params.product_id;
        const sales = await sale.findAll({
            where: { product_id: product_id },
            attributes: ['restaurant_name', 'quantity'],
        });

        // Fetch the product with the given ID
        const products = await product.findByPk(product_id, {
            attributes: ['product_name', 'price'],
        });

        // Return the product and sale as JSON
        res.status(200).json({ products, sales });
    } catch (error) {
        console.error('Error fetching sale:', error);
        res.status(500).send('Unable to fetch sale');
    }
});

// GET request to get the total sales data
app.get('/api/total_sales', async (req, res) => {
    try {
        // Fetch all sales from the database
        const sales = await sale.findAll();
        const products = await product.findAll();

        // Calculate the total sales and total income
        let totalSales = 0;
        let totalIncome = 0;
        sales.forEach(sale => {
            totalSales += sale.quantity;
            totalIncome += sale.quantity * products.find(product => product.id === sale.product_id).price;
        });

        // Return the total sales as JSON
        res.json({ total_sales: totalSales, total_income: totalIncome });
    } catch (error) {
        console.error('Error fetching total sales:', error);
        res.status(500).send('Unable to fetch total sales');
    }
});

// GET request to get the sales data for a specific restaurant
app.get('/api/total_sales/:restaurant_name', async (req, res) => {
    try {
        const restaurant_name = req.params.restaurant_name;
        const products = await product.findAll();

        const sales = await sale.findAll({
            where: { restaurant_name: restaurant_name },
        });

        // Calculate the total sales for the restaurant
        let totalSales = 0;
        let totalIncome = 0;
        sales.forEach(sale => {
            totalSales += sale.quantity;
            totalIncome += sale.quantity * products.find(product => product.id === sale.product_id).price;
        });

        res.json({ total_sales: totalSales, total_income: totalIncome });
    }
    catch (error) {
        console.error('Error fetching total sales:', error);
        res.status(500).send('Unable to fetch total sales for {restaurant_name}');
    }
});

// Post request to add restaurant sales data (restaurant will send restaurant_name, product_name, price, and quantity)
app.post('/api/sales/:password', async (req, res) => {
    try {
        // Check if the password is correct
        if (req.params.password !== process.env.POST_PASSWORD) {
            return res.status(403).send('Unauthorized');
        }

        for (const saleData of req.body) {
            // Find or create the product with the correct price
            const [productData, productCreated] = await product.findOrCreate({
                where: {
                    product_name: saleData.product_name,
                    price: saleData.price,
                }
            });

            // Find or create the sale record for the restaurant
            const [restaurantSaleData, created] = await sale.findOrCreate({
                where: {
                    restaurant_name: saleData.restaurant_name,
                    product_id: productData.id,
                },
                defaults: {
                    quantity: saleData.quantity,  // Sets the quantity if the record is created
                }
            });

            // If the record already existed, increment the quantity
            if (!created) {
                restaurantSaleData.quantity += saleData.quantity;
                await restaurantSaleData.save();
            }
        }
        res.status(200).send('Sales data processed successfully!');
    } catch (error) {
        console.error('Error processing sales data:', error);
        res.status(500).send('Internal server error');
    }
});


const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
