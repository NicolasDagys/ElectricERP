# Electric ERP

Electric ERP is a warehouse and logistics management system designed for **BIOS Logística S.R.L.**, a technology and electronics distribution company operating multiple warehouses.

The system focuses on **inventory management, warehouse operations, stock control, internal transfers, logistics, traceability, and operational reporting**.

The project was developed with **ASP.NET Core and .NET 8**, with a focus on backend development, relational data modeling, authentication and authorization, business rules, and maintainable system design.

---

## Project Overview

Electric ERP addresses the operational processes involved in managing products across multiple warehouses and sections.

The system provides functionality for:

* Product and supplier management
* Warehouse and section management
* Stock control and inventory
* Stock adjustments with approval workflows
* Internal warehouse transfers
* Vehicle and transporter management
* Transfer tracking and GPS location records
* Movement traceability and auditing
* Operational alerts
* Reports
* QR-based product and label identification
* Role-based access control

The system is intentionally designed as a **modular monolithic application**, keeping the architecture simple while maintaining clear separation between responsibilities.

---

## Main Objectives

The main objectives of the system are:

1. Centralize warehouse and inventory operations.
2. Maintain accurate stock levels across warehouses and sections.
3. Control stock modifications through approval workflows.
4. Provide traceability for inventory movements.
5. Manage internal product transfers between warehouses.
6. Track transportation operations and delivery status.
7. Provide role-based access to operational functionality.
8. Maintain an auditable history of important business operations.
9. Provide a maintainable foundation for future system evolution.

---

# Functional Scope

## Inventory Management

The system manages:

* Products
* Suppliers
* Warehouses
* Warehouse sections
* Stock
* Inventory counts
* Stock adjustments
* Inventory movements

Stock is associated with a specific product and warehouse section, allowing the system to represent the physical distribution of inventory.

---

## Stock Adjustments

Stock discrepancies are not modified directly by every user.

The system implements an approval workflow:

```text
Employee
   │
   ▼
Adjustment Request
   │
   ▼
Supervisor Review
   │
   ├── Rejected
   │
   └── Approved
          │
          ▼
      Stock Update
          │
          ▼
    Movement Record
```

This approach provides better control over inventory modifications and creates a traceable history of stock changes.

---

## Warehouse Transfers

The system supports transfers of products between warehouses.

A transfer follows a controlled workflow:

```text
Transfer Request
       │
       ▼
Administrator Assignment
       │
       ├── Vehicle
       └── Transporter
       │
       ▼
Transport Started
       │
       ▼
GPS Location Tracking
       │
       ▼
Transport Completed
       │
       ▼
Supervisor Reception
       │
       ▼
Inventory Updated
```

The transfer process separates **requesting, authorization, transportation, and reception responsibilities**.

This prevents a single user from controlling the complete lifecycle of an inventory transfer.

---

# User Roles

The application implements role-based authorization using ASP.NET Core Identity and JWT authentication.

The main roles are:

| Role          | Responsibility                                       |
| ------------- | ---------------------------------------------------- |
| Administrator | System administration and transfer authorization     |
| Supervisor    | Inventory approvals, transfer requests and reception |
| Employee      | Operational warehouse activities                     |
| Transporter   | Transportation and transfer execution                |

Authorization is enforced at the API level, allowing business operations to be restricted according to the authenticated user's role.

---

# Architecture

Electric ERP follows a **layered modular architecture** designed around separation of responsibilities.

The main logical components are:

```text
┌──────────────────────────────┐
│        Presentation          │
│       ASP.NET Core API       │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│          Application         │
│ Use Cases / Business Logic   │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│            Domain            │
│ Entities / Business Rules    │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│        Infrastructure        │
│ EF Core / SQL Server / Auth  │
└──────────────────────────────┘
```

The goal is not to introduce architectural complexity for its own sake.

The system uses a **pragmatic monolithic architecture**, appropriate for the project's scope while keeping responsibilities separated enough to support future evolution.

---

# Backend

The backend is implemented using **ASP.NET Core Web API on .NET 8**.

The API exposes HTTP endpoints for:

* Authentication
* Products
* Suppliers
* Warehouses
* Sections
* Stock
* Inventory
* Adjustments
* Transfers
* Vehicles
* GPS locations
* Alerts
* Reports

The API is documented and testable through **Swagger / OpenAPI**.

---

# Authentication and Authorization

Authentication is implemented using **ASP.NET Core Identity combined with JWT**.

ASP.NET Core Identity is responsible for:

* User management
* Password management
* Roles
* Security tokens
* Authenticator-based MFA support

The API continues to use **JWT tokens** for stateless authorization.

This separation allows Identity to manage user security without introducing unnecessary changes to the existing API authentication model.

### User model

The application uses a custom:

```csharp
ApplicationUser : IdentityUser
```

The database context is based on:

```csharp
IdentityDbContext<ApplicationUser, IdentityRole, string>
```

This integrates Identity with the existing Entity Framework Core data model.

---

# Multi-Factor Authentication

The system supports **TOTP-based authenticator applications** through ASP.NET Core Identity.

Users can configure an authenticator application and generate time-based verification codes.

This provides an additional authentication factor without introducing an external identity platform or Azure-specific authentication dependency.

---

# Data Access

The system uses:

* **Entity Framework Core**
* **SQL Server**
* Relational database modeling
* EF Core migrations

The database represents the main business concepts through entities such as:

* ApplicationUser
* Product
* Supplier
* Warehouse
* Section
* Stock
* Movement
* Adjustment Request
* Transfer
* Vehicle
* GPS Location
* Alert

Relationships are modeled to preserve the business rules and traceability requirements of the ERP.

---

# Domain and Business Rules

Business rules are implemented around real operational workflows rather than treating the application as simple CRUD.

Examples include:

### Stock

A stock record belongs to a product and a warehouse section.

### Adjustments

An employee can request an adjustment, but the modification requires supervisor approval.

### Transfers

A transfer requires authorization and assignment before transportation begins.

### Reception

The receiving warehouse confirms the transfer before the operation is considered completed.

### Traceability

Important inventory operations generate movement records that allow the system to reconstruct how stock changed over time.

These rules are enforced by the application rather than relying exclusively on the frontend.

---

# Inventory Traceability

One of the main design goals is maintaining an audit trail for inventory operations.

Instead of simply changing a stock quantity, important operations generate movement records containing information such as:

* Product
* User
* Section
* Movement type
* Quantity
* Date/time
* Related operation

This provides historical visibility into how inventory changed.

The design deliberately avoids creating unnecessary direct relationships between every business entity.

For example, a movement can describe the inventory operation while transfer and adjustment entities maintain their own workflow state.

---

# Database Design

The system uses a relational SQL Server database.

The data model represents the operational structure of the company:

```text
Company
   │
   ├── Warehouses
   │      │
   │      └── Sections
   │              │
   │              └── Stock
   │                     │
   │                     └── Products
   │
   ├── Suppliers
   │
   ├── Transfers
   │      │
   │      ├── Vehicle
   │      ├── Transporter
   │      └── GPS Locations
   │
   └── Inventory Movements
```

The model was designed around the actual business processes instead of forcing the domain into a generic CRUD structure.

---

# Security Design

Security considerations include:

* JWT-based API authentication
* Role-based authorization
* ASP.NET Core Identity
* Password hashing through Identity
* TOTP authenticator support
* Protected API endpoints
* Separation of authentication and business authorization
* Validation of user permissions for sensitive operations

Sensitive operations such as stock adjustments and transfer authorization are restricted according to the user's role.

---

# QR Code Integration

The system uses QR codes to facilitate product and label identification.

QR generation is implemented on the backend and can be used to associate physical labels with system entities.

This provides a bridge between the physical warehouse environment and the digital inventory system.

---

# Reporting and Alerts

The system includes operational reporting and alert capabilities for relevant inventory and logistics events.

Examples include:

* Stock levels
* Inventory discrepancies
* Stock movement history
* Transfer status
* Operational alerts

The reporting model is designed to consume the transactional data generated by the system while preserving the original movement history.

---

# Design Decisions

Several architectural decisions were made deliberately.

### Modular Monolith instead of Microservices

The system is implemented as a modular monolith.

Microservices were intentionally avoided because the project's scale does not justify the additional operational complexity of:

* Distributed deployments
* Service-to-service communication
* Distributed tracing
* Independent data stores
* Message brokers
* Service discovery

The goal is to demonstrate good system design without adding infrastructure complexity that does not provide business value.

### Simple Layered Architecture

The architecture separates presentation, application logic, domain concepts, and infrastructure responsibilities.

This provides maintainability while keeping the system understandable.

### ASP.NET Core Identity + JWT

ASP.NET Core Identity was introduced specifically for:

* User management
* Roles
* Password security
* Authenticator-based MFA

JWT remains the mechanism used by the API for authenticated requests.

This avoids replacing the existing authentication model unnecessarily.

### SQL Server + Entity Framework Core

SQL Server was selected because the ERP is highly relational and contains clear relationships between:

* Products
* Stock
* Warehouses
* Users
* Transfers
* Movements
* Suppliers

EF Core provides a strong integration with the .NET ecosystem while keeping persistence code maintainable.

### Workflow-based Business Logic

The system models operations such as adjustments and transfers as workflows rather than exposing direct database modifications.

This makes authorization, validation, and traceability part of the domain behavior.

---

# Technology Stack

### Backend

* C#
* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* ASP.NET Core Identity
* JWT Authentication
* TOTP / Authenticator MFA
* Swagger / OpenAPI

### Database

* Microsoft SQL Server
* Entity Framework Core
* Relational data modeling
* EF Core migrations

### Development

* Visual Studio
* Git
* GitHub
* Swagger UI
* SQL Server Management Studio

### Additional Technologies

* QR Code generation
* GPS/location tracking
* Reporting
* Alerts and notifications

---

# Project Structure

A simplified representation of the backend structure is:

```text
ElectricERP
│
├── ElectricERP.Api
│   ├── Controllers
│   ├── Middleware
│   └── Configuration
│
├── ElectricERP.Application
│   ├── Services
│   ├── DTOs
│   └── Business Logic
│
├── ElectricERP.Domain
│   ├── Entities
│   ├── Enums
│   └── Domain Rules
│
├── ElectricERP.Infrastructure
│   ├── Persistence
│   ├── Identity
│   └── External Services
│
└── ElectricERP.Tests
```

The exact implementation may evolve as the system grows, but the main objective remains maintaining clear boundaries between responsibilities.

---

# Engineering Focus

Electric ERP was designed as a practical demonstration of backend and system design skills using the Microsoft ecosystem.

The project demonstrates experience with:

* Object-oriented programming
* C# and .NET
* ASP.NET Core Web APIs
* RESTful API design
* Relational database design
* Entity Framework Core
* Authentication and authorization
* JWT
* ASP.NET Core Identity
* MFA/TOTP
* Role-based access control
* Business workflows
* Inventory domain modeling
* Transactional operations
* Auditability and traceability
* Error handling and validation
* System modularization
* API documentation
* QR-based identification
* Logistics and GPS workflows

---

# Future Evolution

The current architecture provides a foundation for future improvements without requiring a complete redesign.

Potential future extensions include:

* Additional warehouse automation
* Advanced reporting
* More detailed observability
* Additional integrations
* Mobile application enhancements
* More sophisticated notification mechanisms
* Cloud deployment
* CI/CD automation

The architecture intentionally keeps these possibilities open without prematurely introducing unnecessary infrastructure or distributed-system patterns.

---

# Project Goal

Electric ERP was built to solve a concrete business problem while demonstrating practical software engineering principles.

The primary focus is not simply implementing CRUD operations, but designing a system that can represent **real business workflows, enforce authorization, maintain data consistency, and provide traceability across inventory and logistics operations**.

The project combines **.NET backend development, relational database design, authentication, authorization, business logic, and system architecture** into a single end-to-end ERP solution.
