# 🚚 Delivery System Backend API

A scalable and modular **.NET Core Web API** for a delivery system, designed with layered architecture and JWT-based authentication.

---

## 📌 Overview

This project implements a complete backend for a delivery platform, including:

* User authentication & authorization
* Product & category management
* Cart operations
* Order processing workflow
* Address management

The architecture is designed for **clean separation of concerns** and easy scalability.

---

## 🧱 Project Structure

```text
DeliverySystem
│
├── DeliverySystem.API
│   ├── Controllers
│   ├── Middleware
│   ├── Extensions
│   └── Authentication
│
├── DeliverySystem.Data
│   ├── Context
│   ├── Services
│   ├── Infrastructure
│   ├── Entities
│   ├── ViewModels (DTOs)
│   └── Helpers
│
├── DeliverySystem.JWT
│   └── TokenBuilder (JWT generation logic)
```

---

## 🔐 Authentication

* JWT-based authentication
* Token generation handled via **TokenBuilder**
* Role-based access:

  * Admin
  * User

### 🔑 Usage

```text
Authorization: Bearer <JWT_TOKEN>
```

---

## 👤 User Features

* Register
* Login
* Forgot Password / Reset Password
* Address Management

  * Add / Update / Delete
  * Default Address logic

---

## 📦 Product Features

* Category Management (CRUD)
* Product Management (CRUD)
* Product Search & Filtering
* Pagination support

---

## 🛒 Cart Features

* Add to Cart
* Update Quantity
* Remove Item
* View Cart

---

## 📑 Order Features

* Checkout (Cart → Order)
* Payment (Mock)
* Order Status Management
* Cancel Order
* Order History
* Order Details (with items)

---

## 👑 Admin Features

* View all orders
* Update order status
* Manage products & categories

---

## 🌱 Database Seeding

On application startup:

* Default **Admin** and **User** are created automatically

| Role  | Email                                     | Password |
| ----- | ----------------------------------------- | -------- |
| Admin | [admin@gmail.com](mailto:admin@gmail.com) | 123456   |
| User  | [user@gmail.com](mailto:user@gmail.com)   | 123456   |

---

## 🛠️ Tech Stack

* .NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* BCrypt Password Hashing

---

## ⚙️ Setup Instructions

### 1️⃣ Clone Repository

```bash
git clone https://github.com/yourusername/delivery-system-api.git
cd delivery-system-api
```

### 2️⃣ Configure Database

Update connection string in `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=DeliveryDB;Trusted_Connection=True;"
}
```

### 3️⃣ Apply Migrations

```bash
dotnet ef database update
```

### 4️⃣ Run Application

```bash
dotnet run
```

---

## 📬 API Modules

### Auth

* Register / Login
* Forgot / Reset Password

### Product

* Categories
* Products (CRUD + Filter)

### Cart

* Add / Update / Remove / Get

### Order

* Checkout
* Payment
* Cancel
* History
* Details

---

## 🚀 Future Enhancements

* Real Payment Gateway Integration (Razorpay / Stripe)
* Redis Caching
* Docker Support
* Unit & Integration Testing
* API Rate Limiting

---

## 📄 License

MIT License

---

## 🙌 Author

**Yash Bhalodiya**

---

## ⭐ Show Your Support

If you found this project useful, consider giving it a ⭐ on GitHub!
