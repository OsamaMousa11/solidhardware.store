# 🚀 SolidHardware Store API

A full-featured **E-Commerce Web API** built using **ASP.NET Core**, implementing clean architecture principles, user authentication, authorization, product management, shopping cart, wishlist, orders, bundles, and category management.

This API supports both **guest users** (Wishlist only) and **authenticated users**, including admin-level operations.

---

## 🛠️ Technologies Used

- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  
- ASP.NET Identity (JWT + Refresh Token)  
- Fluent Validation + Validation Helper  
- Generic Repository Pattern  
- Unit of Work Pattern  
- LINQ  
- AutoMapper  
- Serilog Logging  

---

## 📐 Architecture

This project follows modern, scalable architectural patterns:

- **Clean Architecture**
- **Onion Architecture**
- **N-Tier Layering**
- **SOLID Principles**
- **Repository Pattern**
- **Unit of Work Pattern**
- **DTO Mapping Approach**

---

# 🔐 Authentication (Account Endpoints)

| Method | Endpoint | Description |
|-------|----------|-------------|
| POST | `/api/Account/register` | Register a new user |
| POST | `/api/Account/login` | Login + JWT + Refresh Token |
| GET | `/api/Account/refresh-token` | Issue a new access token |
| POST | `/api/Account/revoke-token` | Revoke a refresh token |
| POST | `/api/Account/forgot-password` | Send reset email |
| POST | `/api/Account/reset-password` | Reset password |
| POST | `/api/Account/add-role` | Add role to user |
| GET | `/api/Account/roles` | List all roles |
| DELETE | `/api/Account/roles/{roleName}` | Delete a role |
| GET | `/api/Account/users` | Get all users |
| GET | `/api/Account/users/{id}` | Get user by id |
| DELETE | `/api/Account/users/{id}` | Delete user |
| PUT | `/api/Account/update/{id}` | Update user |

---

# 📦 Bundle Endpoints (User Only)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Bundle` | Create a bundle |
| GET | `/api/Bundle` | Get all bundles (Public) |
| GET | `/api/Bundle/{id}` | Get a bundle by ID |
| PUT | `/api/Bundle` | Update bundle |
| DELETE | `/api/Bundle/{id}` | Delete bundle |

---

# 🛒 Cart Endpoints (User Only)


| GET | `/api/Cart` | Get logged-in user's cart |
| GET | `/api/Cart/user/{userId}` | (Admin) Get cart for any user |
| POST | `/api/Cart/add` | Add or update item |
| PUT | `/api/Cart/update` | Update quantity |
| DELETE | `/api/Cart/remove/{productId}` | Remove item |
| DELETE | `/api/Cart/clear` | Clear cart |
| GET | `/api/Cart/contains/{productId}` | Product exists in cart? |
| GET | `/api/Cart/count` | Cart item count |
| GET | `/api/Cart/subtotal` | Cart subtotal |

---

# 🗂️ Category Endpoints


| GET | `/api/Category/GetAllCategory` | Get all categories |
| GET | `/api/Category/GetCategoryById/{id}` | Get category |
| POST | `/api/Category/CreateCategory` | Create category (Admin) |
| DELETE | `/api/Category/DeleteCategory/{id}` | Delete category (Admin) |
| PUT | `/api/Category/UpdateCategory` | Update category (Admin) |

---

# 🧾 Order Endpoints


| POST | `/api/Order/create` | Create new order |
| PUT | `/api/Order/update` | Update order (Admin) |
| DELETE | `/api/Order/delete/{orderId}` | Delete order (Admin) |
| GET | `/api/Order/{orderId}` | Get order (Owner/Admin) |
| GET | `/api/Order/my-orders` | Get logged-in user's orders |
| GET | `/api/Order/user/{userId}` | (Admin) Get any user's orders |

---

# 🛍️ Product Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Product/CreateProduct` | Create product |
| GET | `/api/Product/GetProductById/{id}` | Get product |
| GET | `/api/Product/GetAllProducts` | List all products |
| DELETE | `/api/Product/DeleteProduct/{id}` | Delete product |
| GET | `/api/Product/GetProductsByCategory/{categoryId}` | Filter by category |
| GET | `/api/Product/SearchProducts` | Search products |
| PUT | `/api/Product/UpdateProduct` | Update product |

---

# ❤️ Wishlist Endpoints (Guest + User)

Supports **anonymous users via cookie** + **authenticated users**.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/wishlist` | Get wishlist (auto-create) |
| POST | `/api/wishlist/{productId}` | Add item |
| DELETE | `/api/wishlist/{productId}` | Remove item |
| DELETE | `/api/wishlist/clear` | Clear wishlist |
| GET | `/api/wishlist/in-wishlist/{productId}` | Exists in wishlist? |

---

# 📑 Schemas

The project includes all necessary DTOs:

- RegisterDTO  
- LoginDTO  
- OrderAddRequest  
- OrderUpdateRequest  
- ProductAddRequest  
- ProductUpdateRequest  
- CategoryAddRequest  
- CategoryUpdateRequest  
- CartAddRequest  
- CartUpdateRequest  
- BundleAddRequest  
- BundleUpdateRequest  
- WishListAddRequest  
- **ApiResponse (Global Response Wrapper)**

---

# 🧪 Testing

All endpoints are fully testable using **Swagger UI**, including:

- JWT Authentication  
- Role-based endpoints  
- Model validation  
- Anonymous wishlist behavior  


