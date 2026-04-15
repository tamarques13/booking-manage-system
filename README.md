# Booking Manage System
![CI](https://github.com/tamarques13/booking-manage-system/actions/workflows/dotnet-ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/license-MIT-blue.svg)

A backend booking management API built with ASP.NET Core, featuring JWT authentication, Hangfire jobs and SQL Server support for reservation workflows and related services.

## Features
- **JWT Authentication**: Secure user authentication and authorization.
- **Refresh Tokens**: Extend user sessions securely with refresh token support.
- **Hangfire Integration**: Background job processing for tasks like expiring reservations.
- **SQL Server Support**: Robust database integration for managing reservations, resources and users.
- **Reservation Capacity Management**: Logic to handle reservation limits and availability.
- **Security Utilities**: Includes password hashing and token generation utilities.
- **Modular Architecture**: Organized into controllers, services, repositories and DTOs for maintainability.

## Folder Structure
This project follows a layered architecture approach, separating responsibilities into:

- **Infrastructure/Persistence/**: Contains the `AppDbContext` for database interactions.
- **Infrastructure/Security/**: Security utilities like `PasswordHasher` and `TokenGenerater`.
- **Infrastructure/Migrations/**: Entity Framework migrations for database schema.
- **Application/DTOs/**: Data Transfer Objects for API requests and responses.
- **Application/Services/**: Business logic layer with interfaces and implementations, including `AuthService`, `ReservationService`, and `AdminReservationService`.
- **Application/Services/Auth/Tokens/**: Handles token related logic, such as `AuthToken`.
- **Application/Services/Reservations/Capacity/**: Manages reservation capacity logic.
- **Application/Jobs/**: Background jobs managed by Hangfire.
- **Domain/Models/**: Entity models like `Reservation`, `Resource`, `User`, and `RefreshToken`.
- **API/Middleware/**: Custom middleware like `ErrorHandlingMiddleware`.
- **API/Controllers/**: Handles API endpoints (e.g., `AuthController`, `ReservationController`, `ResourceController`).

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
  - `POST /api/v1/auth/login`: Authenticate and retrieve a JWT.
  - `GET /api/v1/reservations`: Fetch all reservations for User.
  - `POST /api/v1/reservations`: Create a new reservation for User.
  - `GET /api/v1/admin/users`: Retrieves all users in the system.
  - `GET /api/v1/admin/reservations`: Fetch all reservations.
  - `POST /api/v1/admin/reservations`: Creates a new reservation on behalf of an admin user.

## Testing

### Unit Tests
This project includes unit tests for:
- Domain models
   - Resource
   - Reservation
   - Auth
- Service layer business logic 
   - ResourceService
   - ReservationService
   - AdminReservationService
   - AuthService
   - UserService

Run tests locally using:

```bash
dotnet test
```

### Continuous Integration
This project uses **GitHub Actions** to automatically build and test the application.

The CI pipeline runs on:
- Pull requests  

The workflow performs:
- Restore dependencies  
- Build the project  
- Run unit tests  

## Generate Environment Secret Key (Git Bash)

Copy and run the command below in Git Bash to generate a secure secret key:

```
openssl rand -base64 64
```
The output will be a random base64 string (64 characters).

## License
This project is licensed under the MIT License. See the LICENSE file for details.

## Database Schema
The database schema includes the following entities:

- **Users**:
  - Fields: `Id`, `Email`, `Password`, `FirstName`, `LastName`, `Role`.
  - Relationships: One-to-Many with `Reservations` and `RefreshTokens`.
- **Reservations**:
  - Fields: `Id`, `StartDate`, `EndDate`, `NumberOfPeople`, `Status`, `ResourceId`, `UserId`.
  - Relationships: Many-to-One with `Users` and `Resources`.
- **Resources**:
  - Fields: `Id`, `Name`, `Capacity`, `OpeningTime`, `ClosingTime`, `Type`, `Status`, `Weekends`.
  - Relationships: One-to-Many with `Reservations`.
- **RefreshTokens**:
  - Fields: `Id`, `UserId`, `Token`, `ExpireDate`, `CreatedAt`, `RevokedAt`, `ReplacedByToken`, `IsRevoked`, `IpAddress`.
  - Relationships: Many-to-One with `Users`.

## API Documentation (Swagger)
Swagger is integrated into the project:

- **Setup**:
  - Swagger is configured in `Program.cs` with security definitions for JWT.
  - Swagger UI is accessible at `/swagger` in development mode.
- **Usage**:
  - Provides API documentation for all endpoints.
  - Includes JWT authentication details for secured endpoints.

## Advanced Features

- **Refresh Tokens**:
  - Tokens are generated during login and stored in the database.
  - Tokens are validated for expiration and reuse detection.
  - Token rotation is implemented to enhance security.
- **Reservation Capacity**:
  - Temporary holds reservations until confirmation, preventing overbooking.
  - Validates that the number of people in reservations does not exceed the resource's capacity.
  - Throws an exception if the capacity is exceeded.
- **Hangfire Jobs**:
  - Jobs are scheduled to expire reservations after a set time that have not been confirmed.

## Contributing Guidelines

- **Code Style**:
  - Follow C# conventions and use meaningful variable names.
- **Pull Requests**:
  - Create a new branch for each feature or bug fix.
  - Ensure all tests pass before submitting a pull request.
- **Testing**:
  - Write unit tests for new features.
  - Run `dotnet test` to verify changes.