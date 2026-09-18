# MYM Car Rental — Backend API

A production-ready RESTful backend API for a modern car rental platform, built with **ASP.NET Core Web API, C#, Entity Framework Core, and PostgreSQL**.

The backend provides the core services for vehicle management, categories, bookings, authentication, authorization, pricing, image management, customer communication, and administrative operations.

> **MYM Car Rental** is developed as a full-stack freelance software engineering project with a focus on clean architecture, security, scalability, maintainability, and real-world business workflows.

---

## 🚗 Project Overview

MYM Car Rental is a full-stack car rental platform designed to support both customers and administrative staff.

The backend exposes a secure REST API consumed by the Angular frontend and provides:

* Vehicle and category management
* Rental booking workflows
* Daily, weekly, and monthly rental plans
* Dynamic rental price calculation
* Booking availability and conflict validation
* Customer authentication
* Google authentication
* JWT access tokens
* Refresh token rotation
* Role-based authorization
* Customer, Employee, and Manager roles
* Cloudinary image management
* Arabic and English content
* Contact email delivery
* PostgreSQL persistence
* Entity Framework Core migrations
* Swagger / OpenAPI documentation
* API rate limiting

---

# 🏗️ Architecture

The backend follows a **layered Clean Architecture approach** that separates business rules, application contracts, infrastructure implementations, and HTTP concerns.

```text
MYMCarRental
│
├── MYMCarRental.API
│   ├── Controllers
│   ├── Program.cs
│   ├── Middleware / Configuration
│   └── Dependency Injection
│
├── MYMCarRental.Application
│   ├── DTOs
│   ├── Interfaces
│   └── Application Settings
│
├── MYMCarRental.Domain
│   ├── Entities
│   └── Enums
│
└── MYMCarRental.Infrastructure
    ├── Data
    ├── Configurations
    ├── Services
    ├── Authentication
    ├── Email
    ├── Cloudinary
    └── Migrations
```

### Layer Responsibilities

#### API

Responsible for:

* HTTP endpoints
* Controllers
* Authentication and authorization configuration
* Dependency injection
* Swagger/OpenAPI
* API middleware and infrastructure configuration

#### Application

Contains the application's contracts and DTOs:

* Data Transfer Objects
* Service interfaces
* JWT configuration models
* Application-level abstractions

The Application layer does not depend on infrastructure implementations.

#### Domain

Contains the core business model:

* Entities
* Enums
* Core business concepts

Main entities include:

* `Car`
* `CarCategory`
* `CarImage`
* `CarFeatures`
* `Booking`
* `User`

#### Infrastructure

Contains implementation details such as:

* Entity Framework Core
* PostgreSQL access
* Database configurations
* Authentication services
* JWT generation
* Refresh token management
* Booking services
* Cloudinary integration
* Email services
* Database migrations

---

# 🛠️ Technology Stack

| Technology                     | Purpose                         |
| ------------------------------ | ------------------------------- |
| **C#**                         | Backend programming language    |
| **ASP.NET Core Web API**       | REST API framework              |
| **.NET 10**                    | Target framework                |
| **Entity Framework Core**      | ORM and data access             |
| **PostgreSQL**                 | Relational database             |
| **Npgsql**                     | PostgreSQL provider for EF Core |
| **JWT**                        | Access token authentication     |
| **Refresh Tokens**             | Secure session renewal          |
| **Google.Apis.Auth**           | Google authentication           |
| **Cloudinary**                 | Image storage                   |
| **MailKit / MimeKit**          | Email delivery                  |
| **Swagger / OpenAPI**          | API documentation               |
| **ASP.NET Core Rate Limiting** | API protection                  |
| **Dependency Injection**       | Service composition             |
| **EF Core Migrations**         | Database schema versioning      |

---

# ✨ Core Features

## 🚘 Car Management

The API supports complete vehicle management.

Each car can contain:

* Brand
* Model
* Description
* Fuel type
* Transmission
* Seats
* Doors
* Luggage capacity
* Daily price
* Weekly price
* Monthly price
* Active status
* Featured status
* Multiple images
* Primary image
* Additional vehicle features

Administrative users can:

* Create cars
* Update cars
* Delete cars
* Upload images
* Delete images
* Set primary images
* Manage vehicle features

---

## 🏷️ Category Management

Car categories support:

* Create
* Read
* Update
* Delete
* Category images
* Arabic names
* English names
* Slug-based retrieval

---

# 📅 Booking System

The booking service implements the core rental workflow.

Customers can create rental bookings by selecting:

* Vehicle
* Pickup location
* Return location
* Pickup date
* Return date
* Rental plan
* Optional notes

The backend validates the booking before creating it.

### Booking Validation

The system validates:

* Pickup date must be before return date
* Valid pickup location
* Vehicle availability
* Existing active bookings
* Overlapping rental periods

Overlapping bookings are detected using the rental interval:

```text
Existing Pickup < Requested Return
AND
Existing Return > Requested Pickup
```

This prevents the same vehicle from being booked for conflicting rental periods.

---

# 💰 Rental Pricing

The backend supports three rental plans:

```text
Daily
Weekly
Monthly
```

Rental pricing is calculated server-side.

The booking calculation includes:

* Rental duration
* Selected rental plan
* Base rental cost
* Insurance
* Discount
* Tax
* Grand total

The calculated pricing information is stored with the booking so the booking retains the pricing snapshot used at creation time.

---

# 🔐 Authentication

The API uses secure token-based authentication.

Supported authentication mechanisms include:

* JWT access tokens
* Refresh tokens
* Google authentication
* Protected API endpoints
* Role-based authorization

### Access Token

JWT access tokens contain claims such as:

```text
User ID
Name
Email
Role
JWT ID
```

### Refresh Token

Refresh tokens are:

* Cryptographically generated
* Stored as hashes
* Rotated during refresh
* Expiration controlled through configuration

This prevents storing raw refresh tokens in the database.

---

# 👥 Authorization & Roles

The system supports three primary roles:

```text
Customer
Employee
Manager
```

Administrative functionality is protected using role-based authorization.

For example:

```text
Customer
   │
   ├── Browse cars
   ├── View categories
   └── Create/manage own bookings

Employee
   │
   └── Operational booking access

Manager
   │
   ├── Manage cars
   ├── Manage categories
   ├── Manage users
   └── Administrative operations
```

---

# 🔑 Google Authentication

The API supports Google authentication through Google's identity token validation.

The backend:

1. Receives the Google credential.
2. Validates the token.
3. Verifies the Google account information.
4. Finds or creates the application user.
5. Generates application authentication tokens.

New Google users are assigned the default customer role.

---

# 🖼️ Cloudinary Image Management

Vehicle and category images are stored using **Cloudinary**.

The backend manages:

* Image uploads
* Image URLs
* Public IDs
* Primary image selection
* Image deletion

This keeps binary image storage outside the application server and database.

---

# 📧 Email Integration

The backend includes email functionality using:

* MailKit
* MimeKit
* SMTP

The contact service supports:

* Customer name
* Email
* Phone
* Message
* Reply-to customer email

User-provided content is HTML encoded before being included in outgoing messages.

---

# 🌍 Multilingual Content

The backend supports bilingual content for the frontend.

Examples include:

```text
NameAr
NameEn
```

This allows the Angular frontend to display Arabic or English content without duplicating database records.

---

# 🛡️ Security

Security is considered throughout the API architecture.

Implemented security mechanisms include:

* JWT authentication
* Refresh token hashing
* Refresh token rotation
* Issuer validation
* Audience validation
* Token lifetime validation
* Signing key validation
* Role-based authorization
* Protected administrative endpoints
* API rate limiting
* Secure authentication cookies
* HTTPS support
* Configuration-based secrets

### Production Security

Production secrets should be provided through environment variables or the hosting platform's secure configuration.

Sensitive values should never be committed to Git.

Examples include:

```text
Database connection strings
JWT signing keys
Google credentials
Cloudinary credentials
SMTP credentials
```

---

# 🚦 API Rate Limiting

The API uses rate limiting to reduce abuse and protect sensitive endpoints.

Rate limiting is applied to areas such as:

* Authentication
* Token refresh
* Contact requests
* General API requests

This provides an additional layer of protection against excessive requests.

---
