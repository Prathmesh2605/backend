# Expense Tracker

A full-stack expense tracking application built with .NET 8 and Angular 17.

## Project Structure

### Backend (.NET 8)
- `ExpenseTracker.API` - Web API project
- `ExpenseTracker.Core` - Domain entities and interfaces
- `ExpenseTracker.Application` - Application logic and DTOs
- `ExpenseTracker.Infrastructure` - Data access and external services

### Frontend (Angular 17)
- `expense-tracker-client` - Angular application with Material UI

## Features

- User authentication and authorization
- Expense management (CRUD operations)
- Category management
- File attachments for receipts
- Reporting and visualization
- Export capabilities
- Budget planning and tracking
- Recurring expenses

## Prerequisites

- .NET 8 SDK
- Node.js and npm
- SQL Server
- Angular CLI

## Getting Started

1. Clone the repository
2. Backend Setup:
   ```bash
   cd ExpenseTracker
   dotnet restore
   dotnet build
   cd src/ExpenseTracker.API
   dotnet run
   ```

3. Frontend Setup:
   ```bash
   cd src/expense-tracker-client
   npm install
   ng serve
   ```

4. Navigate to `http://localhost:4200` in your browser

## Technology Stack

- Backend:
  - .NET 8 Web API
  - Entity Framework Core
  - SQL Server
  - JWT Authentication
  - Clean Architecture

- Frontend:
  - Angular 17
  - Angular Material
  - NGRX for state management
  - Chart.js for visualizations
  - Angular Reactive Forms

## API Documentation

The API documentation is available through Swagger UI when running the application in development mode.
- Access the Swagger UI at: `https://localhost:5001/swagger`
- API endpoints are versioned (v1)
- Authentication is required for most endpoints

## Deployment

### Backend Deployment
1. Publish the API:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
2. Deploy to your hosting environment (Azure App Service, IIS, Docker, etc.)
3. Configure environment variables for production settings

### Frontend Deployment
1. Build the Angular application:
   ```bash
   ng build --prod
   ```
2. Deploy the contents of the `dist` folder to your web server

## Contributing

1. Fork the repository
2. Create a feature branch
3. Submit a pull request

## License

This project is licensed under the MIT License
