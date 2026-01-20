# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction

## Code Style
- Use specific formatting rules
- Follow naming conventions

## Database Design
- Store `TransactionType` as a non-identity integer foreign key (ValueGeneratedNever) to a new `TransactionTypes` lookup table to facilitate enum mapping with explicit values.
- The `TransactionTypes` table should have the following columns: `TransactionTypeId` (Primary Key), `Name`.
- Ensure that `Transactions.TransactionType` is a foreign key referencing the `TransactionTypes` table.
- Drop the development database and recreate migrations for schema changes as needed (development DB may be destroyed).

## Project-Specific Rules
- Add `Microsoft.Extensions.Configuration` packages to the Infrastructure project when `DesignTimeDbContextFactory` requires `ConfigurationBuilder`.
- Prefer returning DTOs from controllers and using AutoMapper to map EF entities to DTOs instead of returning EF entities directly to avoid JSON cycles and expose stable API shapes. Update `CustomersController` to map `Customer` entity to `CustomerResponse` DTO and `CreateCustomerRequest` to `Customer` for POST requests. When returning data from controllers, map EF entities to DTOs using AutoMapper and return DTOs instead of returning EF entities directly to avoid JSON cycles and expose stable API shapes. Include related portfolios, holdings, transactions, and assets when querying to expose stable API shapes. Update `CustomersController` to return `CustomerResponse` DTOs and eager-load related data.
- Tests should deserialize API responses into DTO types (e.g., `CustomerResponse`) instead of domain entities; update `CustomersControllerTests` accordingly.
- Prefer JWT configuration to read the key from user-secrets and validate the issuer and audience when generating and validating JWTs.