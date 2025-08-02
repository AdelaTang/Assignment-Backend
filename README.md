# Order API

A clean, maintainable Orders API built with C#, .NET 8 Web API, and Entity Framework Core .

## Features

- RESTful API endpoint for creating orders
- Entity Framework Core with SQLite database
- Proper error handling and logging
- Dependency injection
- Async/await patterns throughout
- Unit tests for Services and Repositories

## Project Structure

```
OrdersAPI/
├── OrdersAPI.Domain/              # Domain Layer (Entities, Interfaces)
├── OrdersAPI.Application/         # Application Layer (Services, DTOs)
├── OrdersAPI.Infrastructure/      # Infrastructure Layer (Data Access, Repository Implementations)
├── OrdersAPI.WebAPI/              # Presentation Layer (Controllers, API)
├── OrdersAPI.UnitTests/           # Unit Tests
```

## Prerequisites

- .NET 8 SDK or later
- Entity Framework Core tools (optional, for migrations)

## Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd OrdersAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run database migrations** (Optional)
- SQLite database, orders.db already exsit in project, the data user posted will be stored in it
- Migration file also include in project, you can use the migration file to update the database

## Running the API

1. **Navigate to the API project directory**
   ```bash
   cd OrdersAPI.WebAPI
   ```

2. **Run the application**
   ```bash
   dotnet run
   ```

3. **Access the API**
   - API Base URL: `http://localhost:5044/` 
   - Swagger UI: `http://localhost:5044/swagger/index.html`

## Running Tests

1. **Run all tests**
   ```bash
   cd OrdersAPI.UnitTests
   dotnet test
   ```

## API Endpoints

### Create Order
- **Endpoint**: `POST /api/Orders`
- **Request Body**:
  ```json
  {
    "orderId": "3fa85f64-5717-4562-b3fc-2c963f66af88",
    "customerName": "CustomerA",
    "items": [
      {
        "productId": "3fa85f64-5717-4562-b3fc-2c963f66af99",
        "quantity": 2147483647
      }
    ],
    "createdAt": "2025-08-02T09:56:25.054Z"
  }
  ```
- **Success Response**: `201 Created`
  ```json
  {
    "orderId": "3fa85f64-5717-4562-b3fc-2c963f66af88",
    "message": "Order created successfully",
    "createdAt": "2025-08-02T09:56:25.054Z"
  }
  ```
- **Error Responses**:
  - `400 Bad Request` - Invalid request data
  - `409 Conflict` - Order with same ID already exists
  - `500 Internal Server Error` - Unexpected error

## Assumptions

1. No authentication/authorization required
2. Order IDs are provided by the user
3. No validation on ProductIds (assume they exist in a separate product table)

## Troubleshooting

1. **Port already in use**: Change the port in `launchSettings.json`
2. **Build errors**: Ensure .NET 8 SDK and related packages installed
