# EventSourcing Test Project

This is a test project demonstrating an Event Sourcing architecture for a simple bank account domain, implemented in ASP.NET Core with C# and Entity Framework Core (PostgreSQL).

## Overview

The project exposes a REST API for managing bank accounts, supporting operations such as account creation, deposit, withdrawal, and projection saving. All state changes are persisted as events, and the current state is reconstructed by replaying these events.

## Key Patterns and Concepts

- **Event Sourcing**: All changes to application state are stored as a sequence of events. The current state of an entity (e.g., Account) is rebuilt by replaying these events.
- **CQRS (Command Query Responsibility Segregation)**: The project separates commands (state-changing operations) from queries (read operations), using the `CQRSMediatr` library for mediation.
- **Domain-Driven Design (DDD)**: The codebase uses aggregates (e.g., `AccountAggregate`), value objects, and domain events to model business logic.
- **Repository Pattern**: Entity Framework Core is used as the data access layer, abstracting persistence concerns.
- **Mediator Pattern**: The `CQRSMediatr` library is used to decouple request handling from controllers.
- **Exception Handling Middleware**: Custom middleware is used for consistent API error responses.
- **Automapper Pattern**: Mapper classes (e.g., `AccountMapper`, `EventsMapper`) are used to convert between domain models and DTOs.

## Technologies Used

- ASP.NET Core (.NET 8)
- Entity Framework Core (PostgreSQL)
- Serilog (logging)
- Swashbuckle (Swagger/OpenAPI)
- CQRSMediatr (CQRS and Mediator implementation)

## Project Structure

- `Aggregates/` - Domain aggregates and root logic
- `Controllers/` - API controllers
- `Data/` - Entity models, DbContext, and migrations
- `Events/` - Domain events
- `Exceptions/` - Custom exception types
- `Extensions/` - Extension methods (e.g., for exception handling, DB migration)
- `Features/` - Command and query handlers (CQRS)
- `Mappers/` - Mapping between domain models and DTOs
- `Models/` - DTOs

## Running the Project

1. Ensure PostgreSQL is running and the connection string in `appsettings.Development.json` is correct.
2. Run database migrations (applied automatically on startup).
3. Start the application. Swagger UI will be available at `/swagger`.

## API Endpoints

- `POST /api/account` - Create a new account
- `GET /api/account/{id}` - Get account by ID (optionally by version)
- `PUT /api/account/{id}/deposit` - Deposit money
- `PUT /api/account/{id}/withdraw` - Withdraw money
- `POST /api/account/{id}/save` - Save account projection

## License
Copyright (c) 2025 Serhiy Krasovskyy xhunter74@gmail.com  

This project is for demonstration and testing purposes only.
