# Expense Tracker Backend

A .NET 8 Web API backend for the Expense Tracker application following Clean Architecture principles.

## Tech Stack

- .NET 8 Web API
- SQL Server
- Entity Framework Core
- JWT Authentication with Identity

## Project Structure

- **ExpenseTracker.Core**: Contains domain entities, interfaces, and business logic
- **ExpenseTracker.Application**: Contains application services, DTOs, and CQRS handlers
- **ExpenseTracker.Infrastructure**: Contains implementations of interfaces defined in Core
- **ExpenseTracker.API**: Contains API controllers and configuration

## Getting Started

1. Ensure you have .NET 8 SDK installed
2. Update the connection string in `appsettings.json`
3. Run the following commands:

```
dotnet restore
dotnet build
dotnet run --project ExpenseTracker.API
```

## Architecture

The backend follows Clean Architecture principles with:
- Domain-driven design
- CQRS pattern for queries and commands
- Repository pattern for data access
- JWT-based authentication
- Entity Framework Code First approach
