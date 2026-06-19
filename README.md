# MyRestaurant - Management & Ordering System 🍕🍔

## 📖 Overview
MyRestaurant is a comprehensive desktop application built with C# and **Windows Presentation Foundation (WPF)**. It simulates a complete restaurant ordering system, featuring user authentication, an interactive menu with visual assets, shopping cart management, and detailed order tracking.

## 🏛️ Architecture & Project Structure
This project is built using a highly scalable **3-Tier Architecture** combined with the **MVVM (Model-View-ViewModel)** design pattern for the presentation layer. This ensures a strict separation of concerns, making the codebase maintainable and extensible.

### 1. Presentation Layer (UI & MVVM)
Handles user interaction and visual state, completely decoupled from the business logic.
*   **Views (`.xaml`)**: Includes modern interfaces like `SignInWindow`, `RestaurantMenuWindow`, `CartWindow`, and `OrderDetailsWindow`.
*   **ViewModels (`*VM.cs`)**: `MainWindowVM`, `CartWindowVM`, and others handle the presentation logic and data binding.
*   **Commands (`RelayCommand.cs`)**: Ensures UI actions (like adding to cart or submitting orders) are handled cleanly without relying on code-behind.

### 2. Business Logic Layer (BLL)
Acts as the intermediary, containing the core business rules of the restaurant.
*   **`UserBLL.cs`**: Manages user authentication, roles (`UserType`), and session validation.
*   **`OrderBLL.cs`**: Processes shopping cart data (`CartItem`), calculates totals, and manages the lifecycle of an `Order` and its `PreparatComandat` (ordered items).

### 3. Data Access Layer (DAL)
Dedicated strictly to database communication and data retrieval.
*   **`DBHelper.cs`**: A centralized utility for managing database connections and executing queries securely.
*   **Data Access Classes (`*DataAccess.cs`)**: `MenuDataAccess`, `OrderDataAccess`, and `UserDataAccess` abstract the CRUD operations, ensuring the rest of the application doesn't need to know about the underlying database implementation.

## 📦 Core Features
*   **Role-Based Access:** Secure sign-in system distinguishing between different user types.
*   **Interactive Menu:** Dynamically loaded catalog of items (`Preparat.cs`) complete with images (e.g., pizzas, pastas, desserts, beverages).
*   **Cart & Checkout:** Real-time cart management system allowing users to review and modify their selections before placing an order.
*   **Order History:** Detailed views of past and current orders via `OrderDetailsModel`.

## 🛠️ Technologies
*   **Language:** C#
*   **Framework:** .NET / WPF
*   **Architecture:** 3-Tier (Presentation, BLL, DAL), MVVM
*   **Concepts:** Data Binding, Separation of Concerns, Relational Database Integration.
