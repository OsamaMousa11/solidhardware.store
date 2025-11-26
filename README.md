# solidhardware.store


## 📘 Overview
E-Commerce Store API is a complete backend system built using **ASP.NET Core Web API**, designed to power a full e-commerce platform.  
The API includes user authentication, product management, categories, bundles, shopping cart, wishlist, and order processing.

The architecture follows **Clean Architecture**, **Onion Architecture**, and **N-Tier** principles to ensure maintainability, scalability, and clean separation of concerns.

---

# 🔧 Technologies Used

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Identity**
- **JWT + Refresh Tokens**
- **LINQ**
- **AutoMapper**
- **Serilog Logging**
- **Repository Pattern**
- **Unit of Work Pattern**
- **Dependency Injection**
- **Clean Architecture / Onion Architecture**

---

# 🧱 Architecture

### ✔ Clean Architecture  
- API (Controllers)  
- Application Layer (Services, DTOs)  
- Domain Layer (Entities)  
- Infrastructure Layer (EF Core, Repositories)

### ✔ Repository + Unit of Work  
- Centralized transaction management  
- Reusable data access  
- Cleaner services logic  

---

# ⭐ Features

### 🔐 Authentication & Authorization
- Register  
- Login  
- JWT authentication  
- Refresh tokens  
- Token revocation  
- Roles (ADMIN / USER)  
- Forgot / Reset password  
- User management  

---

### 🛍 Product Management
- Add / update / delete products (Admin)  
- Get product by ID  
- Get all products (pagination)  
- Search by name  
- Filter by category  

---

### 🗂 Category Management
- Public: get all / get by ID  
- Admin: create, update, delete  

---

### 🎁 Bundle Management (ADMIN ONLY)
- Create bundle  
- Edit bundle  
- Delete bundle  
- Bundle items (with product references)  
- Auto mapping with product data  

---

### 🛒 Shopping Cart
- Add item  
- Update quantity  
- Remove item  
- Clear cart  
- Check if product exists  
- Count items  
- Calculate subtotal  
- Sync product name + price automatically  

---

### ❤️ Wishlist
- Add  
- Remove  
- Clear  
- Get wishlist  
- Prevent duplicates  
- Includes product details via navigation property  

---

### 📦 Orders
- Create order  
- Update order  
- Delete order  
- Get order by ID  
- Get all orders for a user  
- **Total price auto-calculated**  
- OrderItems store **product price snapshot** for history integrity  

---

# 🔒 Role Permissions

| Role | Permissions |
|------|-------------|
| USER | Cart, wishlist, orders, view products |
| ADMIN | Manage products, categories, bundles, users |

---

# 📡 Real-Time Logging (Optional)
Supports **Serilog** for structured logs:
- Error logs  
- Request logs  
- Business events  

---

# 📌 API Endpoints

---

## 🔐 **Account**
```
POST   /api/Account/register
POST   /api/Account/login
GET    /api/Account/refresh-token
POST   /api/Account/revoke-token
POST   /api/Account/forgot-password
POST   /api/Account/reset-password
POST   /api/Account/add-role
GET    /api/Account/roles
DELETE /api/Account/roles/{roleName}
GET    /api/Account/users
GET    /api/Account/users/{id}
PUT    /api/Account/update/{id}
DELETE /api/Account/users/{id}
```

---

## 🗂 **Category**
```
GET    /api/Category/GetAllCategory
GET    /api/Category/GetCategoryById/{id}
POST   /api/Category/CreateCategory          (ADMIN)
PUT    /api/Category/UpdateCategory          (ADMIN)
DELETE /api/Category/DeleteCategory/{id}     (ADMIN)
```

---

## 🛍 **Product**
```
POST   /api/Product/CreateProduct            (ADMIN)
GET    /api/Product/GetProductById/{id}
GET    /api/Product/GetAllProducts
GET    /api/Product/GetProductsByCategory/{id}
GET    /api/Product/SearchProducts?searchTerm=
PUT    /api/Product/UpdateProduct            (ADMIN)
DELETE /api/Product/DeleteProduct/{id}       (ADMIN)
```

---

## 🎁 **Bundle**
```
POST   /api/Bundle/CreateBundle
PUT    /api/Bundle/UpdateBundle
DELETE /api/Bundle/DeleteBundle/{id}
GET    /api/Bundle/GetBundleById/{id}
GET    /api/Bundle/GetAllBundles
```

---

## 🛒 **Cart**
```
GET    /api/Cart/{userId}
POST   /api/Cart/add
POST   /api/Cart/update
DELETE /api/Cart/remove/{userId}/{productId}
DELETE /api/Cart/clear/{userId}
GET    /api/Cart/contains/{userId}/{productId}
GET    /api/Cart/count/{userId}
GET    /api/Cart/subtotal/{userId}
```

---

## ❤️ **Wishlist**
```
GET    /api/Wishlist/user/{userId}
POST   /api/Wishlist/add
DELETE /api/Wishlist/remove?userId=&productId=
DELETE /api/Wishlist/clear/{userId}
GET    /api/Wishlist/{userId}/items
```

---

## 📦 **Order**
```
POST   /api/Order/create
PUT    /api/Order/update
DELETE /api/Order/delete/{orderId}
GET    /api/Order/{orderId}
GET    /api/Order/user/{userId}
```

---

# 🧪 Testing & Tools

You can test all endpoints using:

- Swagger UI (built-in)
- Postman
- Thunder Client (VS Code)
- Curl

Authentication requires sending:

```
Authorization: Bearer <your_token_here>
```

---

# 🧑‍💻 Developer Notes

- AutoMapper is used to map entities → DTOs  
- UnitOfWork ensures safe DB transactions  
- All CRUD operations implemented using Generic Repository  
- Order totals & cart subtotal calculated in backend only  
- Wishlist prevents duplicate product entries  
- Roles enforced using `[Authorize(Roles = "ADMIN")]`  


وهظبطه لك 🔥🔥
