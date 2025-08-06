# WarehouseApp

This repository contains my final project for the ASP.NET Advanced course at SoftUni. It showcases my learning and development throughout the course via a full-featured web application built with ASP.NET Core MVC.

## 📄 Documentation

- 📘 [Course Introduction Project Assignment](./Docs/00.Course-Introduction-Project-Assignment.docx)  
- 💬 [Discord Instructions Guide](./Docs/Discord_Instructions_Guide.docx)

## 📦 Application Overview

**WarehouseApp** is designed to help users manage and track goods across one or more warehouses. It supports importing and exporting products and automatically calculates available stock in real time.

### 🔑 Key Features:

- ✅ **Import Invoices** - Add products to a warehouse through import invoices, each with item quantity and unit price  
- ✅ **Export Invoices** - Remove goods from inventory using export invoices  
- ✅ **Product Structure** - Products are uniquely identified by name and category  
- ✅ **Stock Availability** - Real-time stock calculation by computing the difference between total imports and total exports  
- ✅ **Multi-Warehouse Support** - Each user can manage multiple warehouses with completely isolated inventories and invoices  
- ✅ **User Roles and Security** - Role seeding and secure authentication (e.g., Admin role)

### ⚡ AJAX and Dynamic Data Loading

Products and invoices are retrieved asynchronously using AJAX calls to improve user experience during export creations.

## 🧰 Tech Stack

- **ASP.NET Core MVC** - Main web framework  
- **Entity Framework Core** - ORM for database operations  
- **SQL Server** - Database in development and production  
- **Azure App Service** - Hosting and deployment  
- **Razor Views** - Server-side rendering of HTML  
- **Bootstrap 5** - UI and responsive layout  
- **Microsoft Identity** - Authentication and authorization  
- **NUnit** - Unit testing framework

## 🔐 Security, Admin User & Data Seeding

The application seeds:

- An **admin user** with full permissions  
- Sample **warehouses**, **products**, and **invoices**  

This ensures a complete demonstration of functionality out-of-the-box. You can create new users and manage roles through the admin interface.

## 🧱 Architecture and Patterns

The project follows the **Repository Pattern**, allowing for separation of concerns between business logic and data access. This improves testability, scalability, and maintainability.

## 🚀🌐 Live Demo

The app is deployed and live at:  
🔗 [https://warehouseapp-hxddb5cehqd2cpgv.francecentral-01.azurewebsites.net](https://warehouseapp-hxddb5cehqd2cpgv.francecentral-01.azurewebsites.net)