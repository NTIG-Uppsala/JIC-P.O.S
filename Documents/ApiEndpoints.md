## API Endpoints Documentation

### Base URL
The main API endpoint is as follows:  
Replace `<ipaddress>` with the IP address of the machine where the API is running.

```
<ipaddress>:3000/api/
```

---

### **GET Endpoints**

#### 1. **Get All Products**
**URL:** `/api/products`  
**Description:** Retrieves a list of all products from the database.  
**Response Format:**  
```json
[
    {
        "id": 1,
        "product_name": "Coffee",
        "price": 25
    },
    {
        "id": 2,
        "product_name": "Pasta Carbonara",
        "price": 170
    }
]
```

---

#### 2. **Get All Sales**
**URL:** `/api/sales`  
**Description:** Retrieves a list of all sales from the database.  
**Response Format:**  
```json
[
    {
        "id": 1,
        "restaurant_name": "Restaurant A",
        "product_id": 1,
        "quantity": 10
    },
    {
        "id": 2,
        "restaurant_name": "Restaurant B",
        "product_id": 2,
        "quantity": 5
    }
]
```

---

#### 3. **Get Sales for a Specific Product**
**URL:** `/api/sales/:product_id`  
**Description:** Retrieves sales data for a specific product by its `product_id`.  
**Example Request:** `/api/sales/1`  
**Response Format:**  
```json
{
    "products": {
        "product_name": "Coffee",
        "price": 25
    },
    "sales": [
        {
            "restaurant_name": "Restaurant A",
            "quantity": 10
        }
    ]
}
```

---

#### 4. **Get Total Sales**
**URL:** `/api/total_sales`  
**Description:** Calculates and retrieves the total sales quantity and total income across all products.  
**Response Format:**  
```json
{
    "total_sales": 15,
    "total_income": 625
}
```

---

#### 5. **Get Total Sales for a Specific Restaurant**
**URL:** `/api/total_sales/:restaurant_name`  
**Description:** Retrieves total sales and total income for a specific restaurant.  
**Example Request:** `/api/total_sales/Restaurant%20A`  
**Response Format:**  
```json
{
    "total_sales": 10,
    "total_income": 250
}
```

---

### **POST Endpoint**

#### **Add Sales Data**
**URL:** `/api/sales/:password`  
**Description:** Adds sales data for a restaurant. The request must include a valid password and the sales data in the request body.  
**Authentication:** The `:password` parameter must match the value of `POST_PASSWORD` in the environment variables.  

**Example Request:**  
**URL:** `/api/sales/securepassword`  
**Request Body:**  
```json
[
    {
        "restaurant_name": "Restaurant A",
        "product_name": "Coffee",
        "price": 25,
        "quantity": 5
    },
    {
        "restaurant_name": "Restaurant A",
        "product_name": "Pasta Carbonara",
        "price": 170,
        "quantity": 2
    }
]
```

**Response:**  
  ```
  Sales data processed successfully!
  ```

---

### **Notes**
- Ensure the database connection details and environment variables are correctly configured in the `.env` file.  
- The POST endpoint is password-protected to prevent unauthorized access. 