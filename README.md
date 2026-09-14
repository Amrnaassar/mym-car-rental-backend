# MYM Car Rental API

A scalable and secure RESTful backend API for a car rental platform, built with **ASP.NET Core Web API**, **C#**, **Entity Framework Core**, and **PostgreSQL**.

The API provides the core backend services required to manage rental cars, categories, customers, bookings, authentication, authorization, pricing, and image storage.

---

## Overview

**MYM Car Rental API** is the backend service for a modern car rental web application.

The system is designed using a layered architecture to separate business logic, domain entities, application contracts, infrastructure services, and API endpoints.

It supports:

* Car and category management
* Rental booking workflows
* Daily, weekly, and monthly rental plans
* Rental price calculation
* Booking conflict validation
* JWT authentication
* Refresh token authentication flow
* Role-based authorization
* Google authentication support
* Cloudinary image storage
* Arabic and English content
* Swagger/OpenAPI documentation
* Entity Framework Core migrations

---

## Technology Stack

| Technology                       | Purpose                                        |
| -------------------------------- | ---------------------------------------------- |
| C#                               | Main programming language                      |
| ASP.NET Core Web API             | RESTful API development                        |
| .NET 10                          | Target framework                               |
| Entity Framework Core            | ORM and database access                        |
| PostgreSQL                       | Relational database                            |
| JWT Bearer Authentication        | Secure API authentication                      |
| Refresh Tokens                   | Session renewal                                |
| Google.Apis.Auth                 | Google authentication support                  |
| Cloudinary                       | Image upload and storage                       |
| Swagger / OpenAPI                | API documentation                              |
| Dependency Injection             | Service registration and dependency management |
| Entity Framework Core Migrations | Database schema versioning                     |

---

## Architecture

The project follows a layered architecture:

```text
MYMCarRental
│
├── MYMCarRental.API
│   ├── Controllers
│   ├── Program.cs
│   ├── appsettings.json
│   └── API configuration
│
├── MYMCarRental.Application
│   ├── DTOs
│   ├── Interfaces
│   └── Application settings
│
├── MYMCarRental.Domain
│   ├── Entities
│   └── Enums
│
└── MYMCarRental.Infrastructure
    ├── Data
    ├── Configurations
    ├── Services
    └── Migrations
```

### Layer Responsibilities

#### API Layer

Responsible for:

* HTTP endpoints
* Request handling
* Authentication and authorization
* Controller responses
* Swagger configuration
* Dependency injection configuration

#### Application Layer

Contains:

* DTOs
* Service interfaces
* JWT settings
* Application contracts

This layer defines how the application services communicate without depending directly on implementation details.

#### Domain Layer

Contains the core business entities and enums, including:

* `Car`
* `CarCategory`
* `CarImage`
* `CarFeatures`
* `Booking`
* `User`
* `RentalPlan`
* `BookingStatus`
* `FuelType`
* `Transmission`
* `UserRole`

#### Infrastructure Layer

Responsible for:

* Database access
* Entity Framework Core configurations
* Service implementations
* Authentication services
* JWT token generation
* Booking logic
* Image storage
* Database migrations

---

## Main Features

### Car Management

The API supports managing rental vehicles with information such as:

* Car name
* Brand and model information
* Vehicle specifications
* Fuel type
* Transmission type
* Number of seats
* Number of doors
* Luggage capacity
* Daily rental price
* Weekly rental price
* Monthly rental price
* Car images
* Active/inactive status
* Featured car status

### Category Management

Supports:

* Creating car categories
* Updating categories
* Retrieving categories
* Managing category images
* Arabic and English category names

### Booking Management

The booking system supports:

* Creating rental bookings
* Retrieving customer bookings
* Retrieving booking details
* Managing booking statuses
* Cancelling bookings
* Staff booking management
* Customer and staff booking permissions

Supported booking statuses include:

* `Pending`
* `Confirmed`
* `Cancelled`
* `Completed`

### Rental Plans and Pricing

The system supports multiple rental plans:

* Daily
* Weekly
* Monthly

The booking service calculates:

* Rental duration
* Base rental cost
* Insurance cost
* Discount amount
* Tax amount
* Final total price

The pricing information is calculated during the booking process and stored with the booking data.

### Booking Conflict Validation

The backend validates rental dates to help prevent overlapping bookings for the same vehicle.

This ensures that a car cannot be booked for conflicting rental periods.

### Authentication and Authorization

The API includes secure authentication features using:

* JWT access tokens
* Refresh tokens
* JWT bearer authentication
* Protected endpoints
* Role-based authorization
* Google authentication support

The system supports different user roles, including:

* Customer
* Employee
* Manager

Access to administrative operations is restricted according to the authenticated user's role.

### Image Storage

Car and category images are managed through **Cloudinary**.

The image storage service supports:

* Uploading images
* Storing image URLs
* Managing public image identifiers
* Removing stored images

### Multilingual Support

The backend supports Arabic and English content through bilingual properties such as:

```text
NameAr
NameEn
```

This allows the frontend to display localized content based on the selected language.

---

## API Controllers

The project currently includes the following controllers:

| Controller             | Responsibility                                        |
| ---------------------- | ----------------------------------------------------- |
| `AuthController`       | Authentication, login, registration, token operations |
| `CarsController`       | Car management                                        |
| `CategoriesController` | Car category management                               |
| `BookingsController`   | Booking creation and management                       |
| `UsersController`      | User management                                       |

---

## Example API Routes

### Authentication

```http
POST /api/auth
```

Authentication endpoints support login and token-related operations.

### Cars

```http
GET    /api/cars
GET    /api/cars/{id}
POST   /api/cars
PUT    /api/cars/{id}
DELETE /api/cars/{id}
```

### Categories

```http
GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

### Bookings

```http
POST   /api/bookings
GET    /api/bookings/my
GET    /api/bookings
GET    /api/bookings/{id}
PUT    /api/bookings/{id}/status
DELETE /api/bookings/{id}/cancel
```

> Exact endpoint availability and authorization requirements should be checked in the corresponding controller before integrating with the frontend.

---

## Database

The project uses:

* **PostgreSQL**
* **Entity Framework Core**
* **Code First approach**
* **Entity configurations**
* **EF Core migrations**

The database context is located in:

```text
MYMCarRental.Infrastructure/Data/AppDbContext.cs
```

Entity configurations are organized under:

```text
MYMCarRental.Infrastructure/Configurations
```

Database migrations are located under:

```text
MYMCarRental.Infrastructure/Migrations
```

---

## Getting Started

### Prerequisites

Make sure you have the following installed:

* .NET 10 SDK
* PostgreSQL
* Git
* A code editor such as Visual Studio or Visual Studio Code
* Cloudinary account for image storage

### Clone the Repository

```bash
git clone https://github.com/Amrnaassar/mym-car-rental-backend.git
```

Navigate to the project folder:

```bash
cd mym-car-rental-backend
```

### Configure the Application

Update the configuration values in:

```text
MYMCarRental.API/appsettings.json
```

Required configuration areas include:

* PostgreSQL connection string
* JWT settings
* Cloudinary credentials
* Google authentication settings, if enabled

Do not commit real passwords, API keys, JWT secrets, or private credentials to GitHub.

### Apply Database Migrations

From the solution directory, run:

```bash
dotnet ef database update \
  --project MYMCarRental.Infrastructure \
  --startup-project MYMCarRental.API
```

### Run the API

```bash
dotnet run --project MYMCarRental.API
```

The API can then be accessed through the configured local URL.

### Swagger Documentation

When the application is running, open the Swagger endpoint available in the API environment to explore and test the endpoints.

Swagger provides documentation for:

* Available endpoints
* Request models
* Response models
* Authentication requirements
* API testing

---

## Security Considerations

The API includes:

* JWT token validation
* Issuer and audience validation
* Token lifetime validation
* Signing key validation
* Role-based authorization
* Protected booking endpoints
* Configuration-based secrets

For production deployment, make sure to:

* Use environment variables or secure secret storage
* Use a strong JWT signing key
* Configure production CORS origins
* Enable HTTPS
* Protect Cloudinary credentials
* Use a secure PostgreSQL connection
* Avoid committing sensitive configuration files

---

## Project Status

The backend is currently under active development as part of the **MYM Car Rental** platform.

The project is being developed as a freelance full-stack software engineering project, with ongoing improvements to the rental workflows, frontend integration, and platform functionality.

---

## Related Repository

Frontend repository:

[MYM Car Rental Frontend](https://github.com/Amrnaassar/mym-car-rental-frontend)

---

## Author

**Omar Fathi Salah**

Full-Stack Software Engineer

Specialized in:

* ASP.NET Core
* C#
* Angular
* RESTful APIs
* Entity Framework Core
* PostgreSQL
* SQL Server
* TypeScript
* Full-Stack Web Development

---

## License

This project is private and intended for the MYM Car Rental platform.
