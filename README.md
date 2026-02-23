# Booking Manage System
A backend booking management API built with ASP.NET Core, featuring JWT authentication, Hangfire jobs and SQL Server support for reservation workflows and related services.

## Features
- **JWT Authentication**: Secure user authentication and authorization.
- **Hangfire Integration**: Background job processing for tasks like expiring reservations.
- **SQL Server Support**: Robust database integration for managing reservations, resources and users.
- **Modular Architecture**: Organized into controllers, services, repositories and DTOs for maintainability.

## Folder Structure
- **Controllers/**: Handles API endpoints (e.g., `AuthController`, `ReservationController`, `ResourceController`).
- **Data/**: Contains the `AppDbContext` for database interactions.
- **DTOs/**: Data Transfer Objects for API requests and responses.
- **Helpers/**: Utility classes like `ExceptionHelper` and `ModelsDtoMapper`.
- **Jobs/**: Background jobs managed by Hangfire.
- **Middleware/**: Custom middleware like `ErrorHandlingMiddleware`.
- **Migrations/**: Entity Framework migrations for database schema.
- **Models/**: Entity models like `Reservation`, `Resource` and `User`.
- **Repositories/**: Data access layer with interfaces and implementations.
- **Security/**: Security utilities like `PasswordHasher` and `TokenGenerater`.
- **Services/**: Business logic layer with interfaces and implementations.

## Setup Instructions

### Prerequisites
- .NET SDK 8.0 or later
- SQL Server
- Git Bash (for generating secret keys)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/tamarques13/booking-manage-system.git
   cd BookingSystem
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Configure environment variables in a `.env` file:
   ```env
   DB_CONNECTION_STRING=your-database-connection-string
   SECRET_KEY=your-secret-key (see below for generating one)
   ISSUER=your-jwt-issuer
   AUDIENCE=your-jwt-audience
   ```
   - `DB_CONNECTION_STRING`: Connection string for your SQL Server database.
   - `SECRET_KEY`: A secure key for JWT signing (see the "Generate Environment Secret Key" section below).
   - `ISSUER`: The issuer of the JWT (e.g., your API name).
   - `AUDIENCE`: The audience for the JWT (e.g., your client application).
4. Apply migrations to the database:
   ```bash
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

## Usage
- Use tools like Postman or Swagger to test the API endpoints.
- Example endpoints:
  - `POST /api/auth/login`: Authenticate and retrieve a JWT.
  - `GET /api/reservations`: Fetch all reservations for User.
  - `POST /api/reservations`: Create a new reservation for User.

## Generate Environment Secret Key (Git Bash)

Copy and run the command below in Git Bash to generate a secure secret key:

```
openssl rand -base64 64
```
The output will be a random base64 string (64 characters).

## License
This project is licensed under the MIT License. See the LICENSE file for details.