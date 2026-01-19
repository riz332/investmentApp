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