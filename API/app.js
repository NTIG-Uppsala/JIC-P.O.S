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
    total_price: {
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

// Define Restaurants model
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

// GET request to list all restaurants
app.get('/api/restaurants', async (req, res) => {
    try {
        // Fetch all restaurants from the database
        const restaurants = await restaurant.findAll();

        res.json(restaurants);
    } catch (error) {
        console.error('Error fetching restaurants:', error);
        res.status(500).send('Unable to fetch restaurants');
    }
});

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
        // Fetch all the sales with product_name and quantity
        const sales = await sale_product.findAll({
            include: {
                model: product,
                attributes: ['product_name'],  // Include product_name from the product model
            },
            attributes: ['sale_id', 'quantity'],  // Attributes from sale_product
        });

        // Transform the data to replace product_id with product_name
        const salesWithProductName = sales.map(saleEntry => ({
            sale_id: saleEntry.sale_id,
            product_name: saleEntry.product.product_name,  // Access the product_name from the included product
            quantity: saleEntry.quantity,
        }));

        // Return the sales data as JSON
        res.status(200).json(salesWithProductName);
    } catch (error) {
        console.error('Error fetching sales:', error);
        res.status(500).send('Unable to fetch sales');
    }
});

// GET request to get the total sales data
app.get('/api/sales/income', async (req, res) => {
    try {
        // Fetch all sales from the database
        const income = await sale.findAll({
            attributes: ['total_price'],
        });

        // Calculate the total income
        let totalIncome = 0;
        income.forEach(sale => {
            totalIncome += sale.total_price;
        });

        // Return the total sales as JSON
        res.json({ total_income: totalIncome + ' SEK' });
    } catch (error) {
        console.error('Error fetching total sales:', error);
        res.status(500).send('Unable to fetch total sales');
    }
});

// GET request to get the sales data for a specific restaurant
app.get('/api/sales/income/:restaurant_id', async (req, res) => {
    try {
        const restaurant_id = req.params.restaurant_id;
        const restaurant_name = await restaurant.findByPk(restaurant_id);
        // Fetch all sales from the database
        const income = await sale.findAll({
            where: { restaurant_id: restaurant_id },
            attributes: ['total_price'],
        });

        let totalIncome = 0;
        // Calculate the total income
        income.forEach(sale => {
            totalIncome += sale.total_price;
        });

        // Return the total sales as JSON
        res.json({ total_income: totalIncome + ' SEK for restaurant ' + restaurant_name.name });
    } catch (error) {
        console.error('Error fetching total sales:', error);
        res.status(500).send('Unable to fetch total sales');
    }
});

// GET request to get sales data for a specific product
app.get('/api/sales/:product_id', async (req, res) => {
    try {
        // Fetch all the sales for the product with the given ID
        const product_id = req.params.product_id;
        // Fetch all the sales with product_name and quantity
        const sales = await sale_product.findAll({
            where: { product_id: product_id },
            include: {
                model: product,
                attributes: ['product_name'],  // Include product_name from the product model
            },
            attributes: ['sale_id', 'quantity'],  // Attributes from sale_product
        });

        // Transform the data to replace product_id with product_name
        const salesWithProductName = sales.map(saleEntry => ({
            sale_id: saleEntry.sale_id,
            product_name: saleEntry.product.product_name,  // Access the product_name from the included product
            quantity: saleEntry.quantity,
        }));

        // Return the product and sale as JSON
        res.status(200).json({ salesWithProductName });
    } catch (error) {
        console.error('Error fetching sale:', error);
        res.status(500).send('Unable to fetch sale');
    }
});

// Post request to add restaurant sales data (restaurant will send restaurant_name, address, email, product_name, price, and quantity)
app.post('/api/post/sales/:password', async (req, res) => {
    try {
        // Check if the password is correct
        if (req.params.password !== process.env.POST_PASSWORD) {
            return res.status(403).send('Unauthorized');
        }

        for (const saleData of req.body) {
            // Find or create the restaurant and get the data
            const restaurantData = await findOrCreateRestaurant(saleData);

            // Find or create the product with the correct price
            const [productData, productCreated] = await product.findOrCreate({
                where: {
                    product_name: saleData.product_name,
                    price: saleData.price,
                }
            });

            // Create the sale record for the restaurant
            const restaurantSaleData = await sale.create({
                restaurant_id: restaurantData.id,
                total_price: saleData.price * saleData.quantity,
            });

            // Create the sale_product record for the sale
            await sale_product.create({
                sale_id: restaurantSaleData.id,
                product_id: productData.id,
                quantity: saleData.quantity,
            });

            // Update the total sales for the restaurant
            restaurantData.total_sales += saleData.price * saleData.quantity;
            await restaurantData.save();
        }

        res.status(200).send('Sales data processed successfully!');
    } catch (error) {
        console.error('Error processing sales data:', error);
        res.status(500).send('Internal server error');
    }
});

async function findOrCreateRestaurant(saleData) {
    // Find or create the restaurant with the given name
    const [restaurantData, restaurantCreated] = await restaurant.findOrCreate({
        where: {
            name: saleData.restaurant_name,
        },
        defaults: {
            address: saleData.address,
            email: saleData.email,
            total_sales: saleData.total_sales || 0,  // Initialize total_sales if not provided
        }
    });

    // If the restaurant already existed, update the address and email if they have changed
    if (!restaurantCreated) {
        let needsUpdate = false;

        if (restaurantData.address !== saleData.address) {
            restaurantData.address = saleData.address;
            needsUpdate = true;
        }

        if (restaurantData.email !== saleData.email) {
            restaurantData.email = saleData.email;
            needsUpdate = true;
        }

        // Save updates if any fields were changed
        if (needsUpdate) {
            await restaurantData.save();
        }
    }

    return restaurantData;
}

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
