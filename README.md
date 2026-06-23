# Dark Kitchen Manager

> **Academic Capstone Project** | Application Design 2 (Diseño de Aplicaciones 2)  
> **Universidad ORT Uruguay** | Semester: March 2026

Comprehensive management system for **Dark Kitchens** (ghost kitchens) designed to streamline operations for delivery-only restaurants. The platform provides centralized control over menus, orders, and multiple food brands from a single hub. It features a robust plugin-based architecture that allows for dynamic data importing (e.g., product catalogs via CSV and JSON) to easily integrate with external services and suppliers.

## Project Architecture

The system is divided into two main components:

### 1. Backend (`/DarkKitchen`)
Developed in **C# (.NET)**, following a layered architecture.
- **Web API**: Exposes the REST endpoints.
- **Business Logic & Domain**: Core business logic and entities.
- **Data Access**: Data persistence layer.
- **Plugin System**: Support for dynamic product/data import (includes implementations for `.csv` and `.json`).

### 2. Frontend (`/DarkKitchenUI`)
Developed in **Angular** and styled with **Tailwind CSS**.
- Modern graphical user interface for system management.
- Direct consumption of the Web API.

## Technologies Used
- **Backend:** C#, .NET, ASP.NET Core Web API.
- **Frontend:** Angular, TypeScript, Tailwind CSS.
- **Other:** Container support (see `Dockerfile` files) and Postman collection at the root.

## Installation and Setup

For detailed step-by-step instructions on how to set up the database, run the application using Docker, and configure the plugins, please refer to our official guide:

👉 **[Installation Guide (Guía de instalación)](./Datos/Guia%20instalación.MD)**

## Plugins Guide
For more details on how the plugin system works and how to implement new importers, refer to the [PLUGINS_GUIDE.md](./PLUGINS_GUIDE.md) file.

## Authors
Project developed by Software Engineering students: Mateo Muniz, Ana Barboza and Paula Brenlla.
