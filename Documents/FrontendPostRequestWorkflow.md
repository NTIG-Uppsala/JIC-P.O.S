# Data Sending to Backend API - Documentation
 
## Overview
 
The application sends sales data to the backend API automatically when the app is launched, ensuring that the latest sales data is always sent once per day.
 
## `InitializeSendData` Method
 
- **Trigger**: The `InitializeSendData` method runs each time the application is launched.
- **Process**:
  - It checks the last order date stored in the `date.txt` file.
  - If the stored date is different from today's date, the application sends sales data to the backend API through the post route. For full documentation on all the API routes see [API endpoints](/Documents/ApiEndpoints.md).
  - After sending the data, the current date is written to the `date.txt` file to track the last data submission.
 
## Date File Location
 
The `date.txt` file is located in the following directory:
 
```
%LOCALAPPDATA%\Restaurant Point of Sale System\date.txt
```
 
This file is used to store the last successful data submission date.

---

[Back to README](../README.md)